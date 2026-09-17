using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.Ingredientes
{
    public class AjustarStockDTO
    {
        [Required]
        public decimal Cantidad { get; set; }
    }
}
