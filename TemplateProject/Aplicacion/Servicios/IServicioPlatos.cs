using TemplateProject.DTOs.Platos;

namespace TemplateProject.Aplicacion.Servicios
{
    public interface IServicioPlatos
    {
        Task<List<PlatoDTO>> Listar(bool? activo);
        Task<PlatoDTO?> ObtenerPorId(Guid id);
        Task<PlatoDTO> Crear(CrearPlatoDTO dto);
        Task<PlatoDTO?> Actualizar(Guid id, ActualizarPlatoDTO dto);
        Task<bool> Desactivar(Guid id);
        Task<PlatoDTO?> AgregarIngrediente(Guid platoId, AgregarItemRecetaDTO dto);
        Task<PlatoDTO?> ActualizarCantidadIngrediente(Guid platoId, Guid ingredienteId, ActualizarItemRecetaDTO dto);
        Task<PlatoDTO?> QuitarIngrediente(Guid platoId, Guid ingredienteId);
    }
}
