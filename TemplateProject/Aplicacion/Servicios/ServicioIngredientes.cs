using Microsoft.EntityFrameworkCore;
using TemplateProject.Datos;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using TemplateProject.DTOs.Ingredientes;

namespace TemplateProject.Aplicacion.Servicios
{
    public class ServicioIngredientes : IServicioIngredientes
    {
        private readonly ApplicationDbContext contexto;
        private readonly TimeProvider tiempo;
        private readonly IProveedorUsuario proveedorUsuario;

        public ServicioIngredientes(ApplicationDbContext contexto, TimeProvider tiempo, IProveedorUsuario proveedorUsuario)
        {
            this.contexto = contexto;
            this.tiempo = tiempo;
            this.proveedorUsuario = proveedorUsuario;
        }

        public async Task<List<IngredienteDTO>> Listar(bool? activo)
        {
            var consulta = contexto.Ingredientes
                .Include(x => x.CategoriaIngrediente)
                .Include(x => x.UnidadMedida)
                .AsQueryable();

            if (activo is not null)
                consulta = consulta.Where(x => x.Activo == activo);

            var ingredientes = await consulta.OrderBy(x => x.Nombre).ToListAsync();
            return ingredientes
                .Select(x => MapearADTO(x, x.CategoriaIngrediente!.Nombre, x.UnidadMedida!.Nombre))
                .ToList();
        }

        public async Task<IngredienteDTO?> ObtenerPorId(Guid id)
        {
            var ingrediente = await contexto.Ingredientes
                .Include(x => x.CategoriaIngrediente)
                .Include(x => x.UnidadMedida)
                .FirstOrDefaultAsync(x => x.Id == id);

            return ingrediente is null
                ? null
                : MapearADTO(ingrediente, ingrediente.CategoriaIngrediente!.Nombre, ingrediente.UnidadMedida!.Nombre);
        }

        public async Task<IngredienteDTO> Crear(CrearIngredienteDTO dto)
        {
            await ValidarNombreDisponible(dto.Nombre, idAExcluir: null);

            var categoria = await ObtenerCategoriaActivaOFallar(dto.CategoriaIngredienteId);
            var unidad = await ObtenerUnidadActivaOFallar(dto.UnidadMedidaId);

            var usuarioId = proveedorUsuario.ObtenerUsuarioId();
            var ingrediente = Ingrediente.Crear(
                dto.Nombre,
                dto.Descripcion,
                dto.PrecioUnitario,
                dto.StockMinimo,
                categoria.Id,
                unidad.Id,
                usuarioId,
                tiempo);

            contexto.Ingredientes.Add(ingrediente);
            await contexto.SaveChangesAsync();

            return MapearADTO(ingrediente, categoria.Nombre, unidad.Nombre);
        }

        public async Task<IngredienteDTO?> Actualizar(Guid id, ActualizarIngredienteDTO dto)
        {
            var ingrediente = await contexto.Ingredientes.FindAsync(id);
            if (ingrediente is null)
                return null;

            await ValidarNombreDisponible(dto.Nombre, idAExcluir: id);

            var categoria = await ObtenerCategoriaActivaOFallar(dto.CategoriaIngredienteId);
            var unidad = await ObtenerUnidadActivaOFallar(dto.UnidadMedidaId);

            ingrediente.ActualizarNombre(dto.Nombre, tiempo);
            ingrediente.ActualizarDescripcion(dto.Descripcion, tiempo);
            ingrediente.ActualizarPrecio(dto.PrecioUnitario, tiempo);
            ingrediente.ActualizarStockMinimo(dto.StockMinimo, tiempo);
            ingrediente.CambiarCategoria(categoria.Id, tiempo);
            ingrediente.CambiarUnidadMedida(unidad.Id, tiempo);

            await contexto.SaveChangesAsync();
            return MapearADTO(ingrediente, categoria.Nombre, unidad.Nombre);
        }

        public async Task<bool> Desactivar(Guid id)
        {
            var ingrediente = await contexto.Ingredientes.FindAsync(id);
            if (ingrediente is null)
                return false;

            ingrediente.Desactivar(tiempo);
            await contexto.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Activar(Guid id)
        {
            var ingrediente = await contexto.Ingredientes.FindAsync(id);
            if (ingrediente is null)
                return false;

            ingrediente.Activar(tiempo);
            await contexto.SaveChangesAsync();
            return true;
        }

        public async Task<IngredienteDTO?> AjustarStock(Guid id, decimal cantidad)
        {
            var ingrediente = await contexto.Ingredientes
                .Include(x => x.CategoriaIngrediente)
                .Include(x => x.UnidadMedida)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ingrediente is null)
                return null;

            ingrediente.AjustarStock(cantidad, tiempo);
            await contexto.SaveChangesAsync();

            return MapearADTO(ingrediente, ingrediente.CategoriaIngrediente!.Nombre, ingrediente.UnidadMedida!.Nombre);
        }

        private async Task<CategoriaIngrediente> ObtenerCategoriaActivaOFallar(Guid categoriaId)
        {
            var categoria = await contexto.CategoriasIngredientes.FindAsync(categoriaId);
            if (categoria is null || !categoria.Activo)
                throw new ExcepcionDominio("La categoría de ingrediente indicada no existe o está inactiva.");

            return categoria;
        }

        private async Task<UnidadMedida> ObtenerUnidadActivaOFallar(Guid unidadMedidaId)
        {
            var unidad = await contexto.UnidadesMedida.FindAsync(unidadMedidaId);
            if (unidad is null || !unidad.Activo)
                throw new ExcepcionDominio("La unidad de medida indicada no existe o está inactiva.");

            return unidad;
        }

        private async Task ValidarNombreDisponible(string nombre, Guid? idAExcluir)
        {
            var nombreNormalizado = nombre.Trim();
            var existe = await contexto.Ingredientes
                .AnyAsync(x => x.Nombre == nombreNormalizado && x.Id != (idAExcluir ?? Guid.Empty));

            if (existe)
                throw new ExcepcionDominio($"Ya existe un ingrediente con el nombre '{nombreNormalizado}'.");
        }

        private static IngredienteDTO MapearADTO(Ingrediente ingrediente, string categoriaNombre, string unidadMedidaNombre) => new()
        {
            Id = ingrediente.Id,
            Nombre = ingrediente.Nombre,
            Descripcion = ingrediente.Descripcion,
            Activo = ingrediente.Activo,
            PrecioUnitario = ingrediente.PrecioUnitario,
            StockActual = ingrediente.StockActual,
            StockMinimo = ingrediente.StockMinimo,
            PorDebajoDelMinimo = ingrediente.StockPorDebajoDelMinimo(),
            CategoriaIngredienteId = ingrediente.CategoriaIngredienteId,
            CategoriaIngredienteNombre = categoriaNombre,
            UnidadMedidaId = ingrediente.UnidadMedidaId,
            UnidadMedidaNombre = unidadMedidaNombre,
            FechaCreacion = ingrediente.FechaCreacion,
            FechaModificacion = ingrediente.FechaModificacion,
            UsuarioCreacionId = ingrediente.UsuarioCreacionId
        };
    }
}
