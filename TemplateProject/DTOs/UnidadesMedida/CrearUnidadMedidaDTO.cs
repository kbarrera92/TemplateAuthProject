using System.ComponentModel.DataAnnotations;

namespace TemplateProject.DTOs.UnidadesMedida
{
    public class CrearUnidadMedidaDTO
    {
        [Required]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [Required]
        [MaxLength(10)]
        public required string Abreviatura { get; set; }
    }
}
