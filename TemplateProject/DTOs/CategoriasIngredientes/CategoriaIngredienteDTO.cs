namespace TemplateProject.DTOs.CategoriasIngredientes
{
    public class CategoriaIngredienteDTO
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public required string UsuarioCreacionId { get; set; }
    }
}
