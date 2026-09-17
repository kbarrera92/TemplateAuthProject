using Microsoft.EntityFrameworkCore;
using TemplateProject.Datos;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using TemplateProject.DTOs.UnidadesMedida;

namespace TemplateProject.Aplicacion.Servicios
{
    public class ServicioUnidadesMedida : IServicioUnidadesMedida
    {
        private readonly ApplicationDbContext contexto;
        private readonly TimeProvider tiempo;
        private readonly IProveedorUsuario proveedorUsuario;

        public ServicioUnidadesMedida(ApplicationDbContext contexto, TimeProvider tiempo, IProveedorUsuario proveedorUsuario)
        {
            this.contexto = contexto;
            this.tiempo = tiempo;
            this.proveedorUsuario = proveedorUsuario;
        }

        public async Task<List<UnidadMedidaDTO>> Listar(bool? activo)
        {
            var consulta = contexto.UnidadesMedida.AsQueryable();

            if (activo is not null)
                consulta = consulta.Where(x => x.Activo == activo);

            var unidades = await consulta.OrderBy(x => x.Nombre).ToListAsync();
            return unidades.Select(MapearADTO).ToList();
        }

        public async Task<UnidadMedidaDTO?> ObtenerPorId(Guid id)
        {
            var unidad = await contexto.UnidadesMedida.FindAsync(id);
            return unidad is null ? null : MapearADTO(unidad);
        }

        public async Task<UnidadMedidaDTO> Crear(CrearUnidadMedidaDTO dto)
        {
            await ValidarNombreDisponible(dto.Nombre, idAExcluir: null);

            var usuarioId = proveedorUsuario.ObtenerUsuarioId();
            var unidad = UnidadMedida.Crear(dto.Nombre, dto.Abreviatura, usuarioId, tiempo);

            contexto.UnidadesMedida.Add(unidad);
            await contexto.SaveChangesAsync();

            return MapearADTO(unidad);
        }

        public async Task<UnidadMedidaDTO?> Actualizar(Guid id, ActualizarUnidadMedidaDTO dto)
        {
            var unidad = await contexto.UnidadesMedida.FindAsync(id);
            if (unidad is null)
                return null;

            await ValidarNombreDisponible(dto.Nombre, idAExcluir: id);

            unidad.ActualizarNombre(dto.Nombre, tiempo);
            unidad.ActualizarAbreviatura(dto.Abreviatura, tiempo);

            await contexto.SaveChangesAsync();
            return MapearADTO(unidad);
        }

        public async Task<bool> Desactivar(Guid id)
        {
            var unidad = await contexto.UnidadesMedida.FindAsync(id);
            if (unidad is null)
                return false;

            unidad.Desactivar(tiempo);
            await contexto.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Activar(Guid id)
        {
            var unidad = await contexto.UnidadesMedida.FindAsync(id);
            if (unidad is null)
                return false;

            unidad.Activar(tiempo);
            await contexto.SaveChangesAsync();
            return true;
        }

        private async Task ValidarNombreDisponible(string nombre, Guid? idAExcluir)
        {
            var nombreNormalizado = nombre.Trim();
            var existe = await contexto.UnidadesMedida
                .AnyAsync(x => x.Nombre == nombreNormalizado && x.Id != (idAExcluir ?? Guid.Empty));

            if (existe)
                throw new ExcepcionDominio($"Ya existe una unidad de medida con el nombre '{nombreNormalizado}'.");
        }

        private static UnidadMedidaDTO MapearADTO(UnidadMedida unidad) => new()
        {
            Id = unidad.Id,
            Nombre = unidad.Nombre,
            Abreviatura = unidad.Abreviatura,
            Activo = unidad.Activo,
            FechaCreacion = unidad.FechaCreacion,
            FechaModificacion = unidad.FechaModificacion,
            UsuarioCreacionId = unidad.UsuarioCreacionId
        };
    }
}
