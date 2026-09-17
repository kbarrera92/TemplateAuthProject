namespace TemplateProject.DTOs.UnidadesMedida
{
    public class UnidadMedidaDTO
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public required string Abreviatura { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public required string UsuarioCreacionId { get; set; }
    }
}
