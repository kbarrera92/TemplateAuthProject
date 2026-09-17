namespace TemplateProject.DTOs.Platos
{
    public class PlatoDTO
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal CostoReceta { get; set; }
        public decimal MargenGanancia { get; set; }
        public List<ItemRecetaDTO> ItemsReceta { get; set; } = new();

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public required string UsuarioCreacionId { get; set; }
    }
}
