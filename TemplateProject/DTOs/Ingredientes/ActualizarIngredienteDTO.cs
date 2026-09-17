using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.Ingredientes
{
    public class ActualizarIngredienteDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StockMinimo { get; set; }

        [Required]
        public Guid CategoriaIngredienteId { get; set; }

        [Required]
        public Guid UnidadMedidaId { get; set; }
    }
}
