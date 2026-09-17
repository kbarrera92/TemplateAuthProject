namespace TemplateProject.DTOs.Platos
{
    public class ItemRecetaDTO
    {
        public Guid IngredienteId { get; set; }
        public required string IngredienteNombre { get; set; }
        public decimal Cantidad { get; set; }
        public required string UnidadMedidaAbreviatura { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
