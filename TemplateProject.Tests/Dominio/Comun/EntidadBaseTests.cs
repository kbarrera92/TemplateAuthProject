using System.Reflection;
using Microsoft.Extensions.Time.Testing;
using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Comun
{
    public class EntidadBaseTests
    {
        private static readonly DateTimeOffset InstanteInicial = new(2026, 9, 17, 6, 0, 0, TimeSpan.Zero);

        private static FakeTimeProvider CrearReloj() => new(InstanteInicial);

        private static void ForzarId(object entidad, Guid id)
        {
            var propiedad = entidad.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!;
            propiedad.SetMethod!.Invoke(entidad, new object[] { id });
        }

        [Fact]
        public void Equals_MismoTipoYMismoId_SonIguales()
        {
            var tiempo = CrearReloj();
            var categoria = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);

            var mismaCategoria = categoria;

            Assert.True(categoria.Equals(mismaCategoria));
            Assert.True(categoria == mismaCategoria);
        }

        [Fact]
        public void Equals_MismoTipoDistintoId_NoSonIguales()
        {
            var tiempo = CrearReloj();
            var categoriaA = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);
            var categoriaB = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);

            Assert.False(categoriaA.Equals(categoriaB));
            Assert.True(categoriaA != categoriaB);
        }

        [Fact]
        public void Equals_DistintoTipoConMismoId_NoSonIguales()
        {
            var tiempo = CrearReloj();
            var categoria = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);
            var unidad = UnidadMedida.Crear("Kilogramo", "kg", "usuario-1", tiempo);
            ForzarId(unidad, categoria.Id);

            Assert.False(categoria.Equals(unidad));
        }

        [Fact]
        public void Equals_AmbasEntidadesSinPersistir_NoSonIgualesPorReferencia()
        {
            var tiempo = CrearReloj();
            var categoriaA = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);
            var categoriaB = CategoriaIngrediente.Crear("Carnes", null, "usuario-1", tiempo);
            ForzarId(categoriaA, Guid.Empty);
            ForzarId(categoriaB, Guid.Empty);

            Assert.False(categoriaA.Equals(categoriaB));
        }

        [Fact]
        public void GetHashCode_MismaEntidad_DevuelveMismoValor()
        {
            var tiempo = CrearReloj();
            var categoria = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);

            Assert.Equal(categoria.GetHashCode(), categoria.GetHashCode());
        }
    }
}
