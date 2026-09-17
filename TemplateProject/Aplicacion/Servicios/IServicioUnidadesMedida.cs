using TemplateProject.DTOs.UnidadesMedida;

namespace TemplateProject.Aplicacion.Servicios
{
    public interface IServicioUnidadesMedida
    {
        Task<List<UnidadMedidaDTO>> Listar(bool? activo);
        Task<UnidadMedidaDTO?> ObtenerPorId(Guid id);
        Task<UnidadMedidaDTO> Crear(CrearUnidadMedidaDTO dto);
        Task<UnidadMedidaDTO?> Actualizar(Guid id, ActualizarUnidadMedidaDTO dto);
        Task<bool> Desactivar(Guid id);
        Task<bool> Activar(Guid id);
    }
}
