using Microsoft.Extensions.Time.Testing;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class PlatoTests
    {
        private static readonly DateTimeOffset InstanteInicial = new(2026, 9, 17, 6, 0, 0, TimeSpan.Zero);

        private static FakeTimeProvider CrearReloj() => new(InstanteInicial);

        private static Plato CrearPlatoValido(FakeTimeProvider tiempo, decimal precioVenta = 25m)
        {
            return Plato.Crear("Bandeja paisa", "Plato típico", precioVenta, "usuario-1", tiempo);
        }

        [Fact]
        public void Crear_DatosValidos_InicializaActivoSinReceta()
        {
            var tiempo = CrearReloj();

            var plato = CrearPlatoValido(tiempo);

            Assert.Equal("Bandeja paisa", plato.Nombre);
            Assert.True(plato.Activo);
            Assert.Empty(plato.ItemsReceta);
            Assert.False(plato.TieneReceta());
            Assert.Equal("usuario-1", plato.UsuarioCreacionId);
            Assert.Equal(InstanteInicial.UtcDateTime, plato.FechaCreacion);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Plato.Crear(nombre!, "desc", 10m, "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_PrecioVentaNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Plato.Crear("Bandeja paisa", null, -1m, "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_UsuarioCreacionVacio_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Plato.Crear("Bandeja paisa", null, 10m, " ", tiempo));
        }

        [Fact]
        public void ActualizarNombre_ValorValido_ActualizaYRecortaEspacios()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            plato.ActualizarNombre("  Bandeja paisa especial  ", tiempo);

            Assert.Equal("Bandeja paisa especial", plato.Nombre);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarNombre_ValorInvalido_LanzaExcepcion(string nombre)
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => plato.ActualizarNombre(nombre, tiempo));
        }

        [Fact]
        public void ActualizarPrecioVenta_ValorNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => plato.ActualizarPrecioVenta(-5m, tiempo));
        }

        [Fact]
        public void ActualizarPrecioVenta_ValorValido_ActualizaYRegistraFechaModificacion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);
            var instanteCambio = InstanteInicial.AddDays(1);
            tiempo.SetUtcNow(instanteCambio);

            plato.ActualizarPrecioVenta(30m, tiempo);

            Assert.Equal(30m, plato.PrecioVenta);
            Assert.Equal(instanteCambio.UtcDateTime, plato.FechaModificacion);
        }

        [Fact]
        public void Desactivar_PlatoActivo_QuedaInactivo()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            plato.Desactivar(tiempo);

            Assert.False(plato.Activo);
        }

        [Fact]
        public void Activar_PlatoInactivo_QuedaActivo()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);
            plato.Desactivar(tiempo);

            plato.Activar(tiempo);

            Assert.True(plato.Activo);
        }

        [Fact]
        public void AgregarIngrediente_DatosValidos_AgregaItemYRegistraFechaModificacion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);
            var ingredienteId = Guid.NewGuid();
            var instanteAjuste = InstanteInicial.AddHours(1);
            tiempo.SetUtcNow(instanteAjuste);

            plato.AgregarIngrediente(ingredienteId, 3m, tiempo);

            Assert.True(plato.TieneReceta());
            var item = Assert.Single(plato.ItemsReceta);
            Assert.Equal(ingredienteId, item.IngredienteId);
            Assert.Equal(3m, item.Cantidad);
            Assert.Equal(instanteAjuste.UtcDateTime, plato.FechaModificacion);
        }

        [Fact]
        public void AgregarIngrediente_IngredienteDuplicado_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);
            var ingredienteId = Guid.NewGuid();
            plato.AgregarIngrediente(ingredienteId, 3m, tiempo);

            Assert.Throws<ExcepcionDominio>(() => plato.AgregarIngrediente(ingredienteId, 1m, tiempo));
        }

        [Fact]
        public void AgregarIngrediente_CantidadInvalida_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => plato.AgregarIngrediente(Guid.NewGuid(), 0m, tiempo));
        }

        [Fact]
        public void ActualizarCantidadIngrediente_IngredienteExistente_Actualiza()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);
            var ingredienteId = Guid.NewGuid();
            plato.AgregarIngrediente(ingredienteId, 3m, tiempo);

            plato.ActualizarCantidadIngrediente(ingredienteId, 5m, tiempo);

            Assert.Equal(5m, plato.ItemsReceta.Single().Cantidad);
        }

        [Fact]
        public void ActualizarCantidadIngrediente_IngredienteInexistente_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => plato.ActualizarCantidadIngrediente(Guid.NewGuid(), 5m, tiempo));
        }

        [Fact]
        public void QuitarIngrediente_IngredienteExistente_LoElimina()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);
            var ingredienteId = Guid.NewGuid();
            plato.AgregarIngrediente(ingredienteId, 3m, tiempo);

            plato.QuitarIngrediente(ingredienteId, tiempo);

            Assert.Empty(plato.ItemsReceta);
            Assert.False(plato.TieneReceta());
        }

        [Fact]
        public void QuitarIngrediente_IngredienteInexistente_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var plato = CrearPlatoValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => plato.QuitarIngrediente(Guid.NewGuid(), tiempo));
        }
    }
}
