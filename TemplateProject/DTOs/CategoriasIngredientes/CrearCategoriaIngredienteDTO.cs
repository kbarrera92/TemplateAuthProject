using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.CategoriasIngredientes
{
    public class CrearCategoriaIngredienteDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }
    }
}
