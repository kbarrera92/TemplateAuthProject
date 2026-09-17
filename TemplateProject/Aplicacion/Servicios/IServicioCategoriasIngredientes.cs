using TemplateProject.DTOs.CategoriasIngredientes;

namespace TemplateProject.Aplicacion.Servicios
{
    public interface IServicioCategoriasIngredientes
    {
        Task<List<CategoriaIngredienteDTO>> Listar(bool? activo);
        Task<CategoriaIngredienteDTO?> ObtenerPorId(Guid id);
        Task<CategoriaIngredienteDTO> Crear(CrearCategoriaIngredienteDTO dto);
        Task<CategoriaIngredienteDTO?> Actualizar(Guid id, ActualizarCategoriaIngredienteDTO dto);
        Task<bool> Desactivar(Guid id);
    }
}
