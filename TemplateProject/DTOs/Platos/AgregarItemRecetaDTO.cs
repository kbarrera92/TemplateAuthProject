using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.Platos
{
    public class AgregarItemRecetaDTO
    {
        [Required]
        public Guid IngredienteId { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal Cantidad { get; set; }
    }
}
