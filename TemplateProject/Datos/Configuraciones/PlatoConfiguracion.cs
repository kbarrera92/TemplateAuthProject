using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Dominio.Entidades;

namespace TemplateProject.Datos.Configuraciones
{
    public class PlatoConfiguracion : IEntityTypeConfiguration<Plato>
    {
        public void Configure(EntityTypeBuilder<Plato> builder)
        {
            builder.ToTable("Platos");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.Property(x => x.PrecioVenta)
                .HasPrecision(18, 2);

            builder.Property(x => x.FechaCreacion)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(x => x.Nombre)
                .IsUnique();

            builder.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
