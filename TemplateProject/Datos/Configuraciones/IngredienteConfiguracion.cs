using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Dominio.Entidades;

namespace TemplateProject.Datos.Configuraciones
{
    public class IngredienteConfiguracion : IEntityTypeConfiguration<Ingrediente>
    {
        public void Configure(EntityTypeBuilder<Ingrediente> builder)
        {
            builder.ToTable("Ingredientes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.Property(x => x.PrecioUnitario)
                .HasPrecision(18, 2);

            builder.Property(x => x.StockActual)
                .HasPrecision(18, 3);

            builder.Property(x => x.StockMinimo)
                .HasPrecision(18, 3);

            builder.Property(x => x.FechaCreacion)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(x => x.Nombre)
                .IsUnique();

            builder.HasOne(x => x.CategoriaIngrediente)
                .WithMany(c => c.Ingredientes)
                .HasForeignKey(x => x.CategoriaIngredienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnidadMedida)
                .WithMany(u => u.Ingredientes)
                .HasForeignKey(x => x.UnidadMedidaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
