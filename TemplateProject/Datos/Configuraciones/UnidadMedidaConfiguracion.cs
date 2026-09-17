using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Dominio.Entidades;

namespace TemplateProject.Datos.Configuraciones
{
    public class UnidadMedidaConfiguracion : IEntityTypeConfiguration<UnidadMedida>
    {
        public void Configure(EntityTypeBuilder<UnidadMedida> builder)
        {
            builder.ToTable("UnidadesMedida");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Abreviatura)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(x => x.Nombre)
                .IsUnique();
        }
    }
}
