using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Dominio.Entidades;

namespace TemplateProject.Datos.Configuraciones
{
    public class ItemRecetaConfiguracion : IEntityTypeConfiguration<ItemReceta>
    {
        public void Configure(EntityTypeBuilder<ItemReceta> builder)
        {
            builder.ToTable("ItemsReceta");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Cantidad)
                .HasPrecision(18, 3);

            builder.HasIndex(x => new { x.PlatoId, x.IngredienteId })
                .IsUnique();

            builder.HasOne<Plato>()
                .WithMany(p => p.ItemsReceta)
                .HasForeignKey(x => x.PlatoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Ingrediente)
                .WithMany()
                .HasForeignKey(x => x.IngredienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
