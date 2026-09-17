using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.Platos
{
    public class ActualizarItemRecetaDTO
    {
        [Range(0.001, double.MaxValue)]
        public decimal Cantidad { get; set; }
    }
}
