namespace TemplateProject.Dominio.Entidades
{
    public class Ingrediente
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        // Costo y stock
        public decimal PrecioUnitario { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }

        // Relaciones
        public Guid CategoriaIngredienteId { get; set; }
        public CategoriaIngrediente? CategoriaIngrediente { get; set; }
        public Guid UnidadMedidaId { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }

        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public required string UsuarioCreacionId { get; set; }
    }
}
