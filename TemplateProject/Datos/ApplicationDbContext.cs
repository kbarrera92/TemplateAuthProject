using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Dominio.Entidades;

namespace TemplateProject.Datos
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
        public DbSet<CategoriaIngrediente> CategoriasIngredientes => Set<CategoriaIngrediente>();
        public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
        public DbSet<Plato> Platos => Set<Plato>();
        public DbSet<ItemReceta> ItemsReceta => Set<ItemReceta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}
