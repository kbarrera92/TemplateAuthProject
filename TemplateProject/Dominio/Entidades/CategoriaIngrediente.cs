namespace TemplateProject.Dominio.Entidades
{
    public class CategoriaIngrediente
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
    }
}
