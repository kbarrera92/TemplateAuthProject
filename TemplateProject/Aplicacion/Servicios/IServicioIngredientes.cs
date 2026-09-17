using TemplateProject.DTOs.Ingredientes;

namespace TemplateProject.Aplicacion.Servicios
{
    public interface IServicioIngredientes
    {
        Task<List<IngredienteDTO>> Listar(bool? activo);
        Task<IngredienteDTO?> ObtenerPorId(Guid id);
        Task<IngredienteDTO> Crear(CrearIngredienteDTO dto);
        Task<IngredienteDTO?> Actualizar(Guid id, ActualizarIngredienteDTO dto);
        Task<bool> Desactivar(Guid id);
        Task<bool> Activar(Guid id);
        Task<IngredienteDTO?> AjustarStock(Guid id, decimal cantidad);
    }
}
