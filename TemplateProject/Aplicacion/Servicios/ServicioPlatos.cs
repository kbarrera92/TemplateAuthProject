using Microsoft.EntityFrameworkCore;
using TemplateProject.Datos;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using TemplateProject.DTOs.Platos;

namespace TemplateProject.Aplicacion.Servicios
{
    public class ServicioPlatos : IServicioPlatos
    {
        private readonly ApplicationDbContext contexto;
        private readonly TimeProvider tiempo;
        private readonly IProveedorUsuario proveedorUsuario;

        public ServicioPlatos(ApplicationDbContext contexto, TimeProvider tiempo, IProveedorUsuario proveedorUsuario)
        {
            this.contexto = contexto;
            this.tiempo = tiempo;
            this.proveedorUsuario = proveedorUsuario;
        }

        public async Task<List<PlatoDTO>> Listar(bool? activo)
        {
            var consulta = CargarPlatosConDetalle();

            if (activo is not null)
                consulta = consulta.Where(x => x.Activo == activo);

            var platos = await consulta.OrderBy(x => x.Nombre).ToListAsync();
            return platos.Select(MapearADTO).ToList();
        }

        public async Task<PlatoDTO?> ObtenerPorId(Guid id)
        {
            var plato = await CargarPlatosConDetalle().FirstOrDefaultAsync(x => x.Id == id);
            return plato is null ? null : MapearADTO(plato);
        }

        public async Task<PlatoDTO> Crear(CrearPlatoDTO dto)
        {
            await ValidarNombreDisponible(dto.Nombre, idAExcluir: null);

            var usuarioId = proveedorUsuario.ObtenerUsuarioId();
            var plato = Plato.Crear(dto.Nombre, dto.Descripcion, dto.PrecioVenta, usuarioId, tiempo);

            contexto.Platos.Add(plato);
            await contexto.SaveChangesAsync();

            return MapearADTO(plato);
        }

        public async Task<PlatoDTO?> Actualizar(Guid id, ActualizarPlatoDTO dto)
        {
            var plato = await CargarPlatosConDetalle().FirstOrDefaultAsync(x => x.Id == id);
            if (plato is null)
                return null;

            await ValidarNombreDisponible(dto.Nombre, idAExcluir: id);

            plato.ActualizarNombre(dto.Nombre, tiempo);
            plato.ActualizarDescripcion(dto.Descripcion, tiempo);
            plato.ActualizarPrecioVenta(dto.PrecioVenta, tiempo);

            await contexto.SaveChangesAsync();
            return MapearADTO(plato);
        }

        public async Task<bool> Desactivar(Guid id)
        {
            var plato = await contexto.Platos.FindAsync(id);
            if (plato is null)
                return false;

            plato.Desactivar(tiempo);
            await contexto.SaveChangesAsync();
            return true;
        }

        public async Task<PlatoDTO?> AgregarIngrediente(Guid platoId, AgregarItemRecetaDTO dto)
        {
            var plato = await CargarPlatosConDetalle().FirstOrDefaultAsync(x => x.Id == platoId);
            if (plato is null)
                return null;

            await ObtenerIngredienteActivoOFallar(dto.IngredienteId);

            plato.AgregarIngrediente(dto.IngredienteId, dto.Cantidad, tiempo);

            var itemNuevo = plato.ItemsReceta.Single(x => x.IngredienteId == dto.IngredienteId);
            contexto.Entry(itemNuevo).State = EntityState.Added;

            await contexto.SaveChangesAsync();

            plato = await CargarPlatosConDetalle().FirstAsync(x => x.Id == platoId);
            return MapearADTO(plato);
        }

        public async Task<PlatoDTO?> ActualizarCantidadIngrediente(Guid platoId, Guid ingredienteId, ActualizarItemRecetaDTO dto)
        {
            var plato = await CargarPlatosConDetalle().FirstOrDefaultAsync(x => x.Id == platoId);
            if (plato is null)
                return null;

            plato.ActualizarCantidadIngrediente(ingredienteId, dto.Cantidad, tiempo);
            await contexto.SaveChangesAsync();

            return MapearADTO(plato);
        }

        public async Task<PlatoDTO?> QuitarIngrediente(Guid platoId, Guid ingredienteId)
        {
            var plato = await CargarPlatosConDetalle().FirstOrDefaultAsync(x => x.Id == platoId);
            if (plato is null)
                return null;

            plato.QuitarIngrediente(ingredienteId, tiempo);
            await contexto.SaveChangesAsync();

            return MapearADTO(plato);
        }

        private IQueryable<Plato> CargarPlatosConDetalle() =>
            contexto.Platos
                .Include(p => p.ItemsReceta)
                .ThenInclude(i => i.Ingrediente!)
                .ThenInclude(i => i!.UnidadMedida);

        private async Task<Ingrediente> ObtenerIngredienteActivoOFallar(Guid ingredienteId)
        {
            var ingrediente = await contexto.Ingredientes.FindAsync(ingredienteId);
            if (ingrediente is null || !ingrediente.Activo)
                throw new ExcepcionDominio("El ingrediente indicado no existe o está inactivo.");

            return ingrediente;
        }

        private async Task ValidarNombreDisponible(string nombre, Guid? idAExcluir)
        {
            var nombreNormalizado = nombre.Trim();
            var existe = await contexto.Platos
                .AnyAsync(x => x.Nombre == nombreNormalizado && x.Id != (idAExcluir ?? Guid.Empty));

            if (existe)
                throw new ExcepcionDominio($"Ya existe un plato con el nombre '{nombreNormalizado}'.");
        }

        private static PlatoDTO MapearADTO(Plato plato)
        {
            var items = plato.ItemsReceta
                .OrderBy(x => x.Ingrediente!.Nombre)
                .Select(x => new ItemRecetaDTO
                {
                    IngredienteId = x.IngredienteId,
                    IngredienteNombre = x.Ingrediente!.Nombre,
                    Cantidad = x.Cantidad,
                    UnidadMedidaAbreviatura = x.Ingrediente.UnidadMedida!.Abreviatura,
                    PrecioUnitario = x.Ingrediente.PrecioUnitario,
                    Subtotal = x.Cantidad * x.Ingrediente.PrecioUnitario
                })
                .ToList();

            var costoReceta = items.Sum(x => x.Subtotal);

            return new PlatoDTO
            {
                Id = plato.Id,
                Nombre = plato.Nombre,
                Descripcion = plato.Descripcion,
                Activo = plato.Activo,
                PrecioVenta = plato.PrecioVenta,
                CostoReceta = costoReceta,
                MargenGanancia = plato.PrecioVenta - costoReceta,
                ItemsReceta = items,
                FechaCreacion = plato.FechaCreacion,
                FechaModificacion = plato.FechaModificacion,
                UsuarioCreacionId = plato.UsuarioCreacionId
            };
        }
    }
}
