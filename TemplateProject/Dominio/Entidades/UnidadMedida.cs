namespace TemplateProject.Dominio.Entidades
{
    public class UnidadMedida
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public required string Abreviatura { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
    }
}
