using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.Platos
{
    public class CrearPlatoDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecioVenta { get; set; }
    }
}
