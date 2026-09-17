namespace TemplateProject.DTOs.Ingredientes
{
    public class IngredienteDTO
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public bool PorDebajoDelMinimo { get; set; }

        public Guid CategoriaIngredienteId { get; set; }
        public required string CategoriaIngredienteNombre { get; set; }
        public Guid UnidadMedidaId { get; set; }
        public required string UnidadMedidaNombre { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public required string UsuarioCreacionId { get; set; }
    }
}
