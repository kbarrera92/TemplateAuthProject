using Microsoft.EntityFrameworkCore;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.Dominio.Comun;
using TemplateProject.DTOs.UnidadesMedida;
using Xunit;

namespace TemplateProject.Tests.Aplicacion.Servicios
{
    public class ServicioUnidadesMedidaTests : BaseServicioTests
    {
        [Fact]
        public async Task Crear_DatosValidos_PersisteYSellaAutoria()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioUnidadesMedida(contexto, CrearReloj(), CrearProveedorUsuario("usuario-1"));

            var resultado = await servicio.Crear(new CrearUnidadMedidaDTO { Nombre = "Kilogramo", Abreviatura = "kg" });

            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Equal("usuario-1", resultado.UsuarioCreacionId);
            Assert.Equal(InstanteInicial.UtcDateTime, resultado.FechaCreacion);
            Assert.Equal(1, await contexto.UnidadesMedida.CountAsync());
        }

        [Fact]
        public async Task Crear_NombreDuplicado_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioUnidadesMedida(contexto, CrearReloj(), CrearProveedorUsuario());
            await servicio.Crear(new CrearUnidadMedidaDTO { Nombre = "Kilogramo", Abreviatura = "kg" });

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Crear(new CrearUnidadMedidaDTO { Nombre = "Kilogramo", Abreviatura = "kilo" }));
        }

        [Fact]
        public async Task ObtenerPorId_IdInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioUnidadesMedida(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.ObtenerPorId(Guid.NewGuid());

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Desactivar_UnidadExistente_QuedaInactivaSinBorrarse()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioUnidadesMedida(contexto, CrearReloj(), CrearProveedorUsuario());
            var unidad = await servicio.Crear(new CrearUnidadMedidaDTO { Nombre = "Kilogramo", Abreviatura = "kg" });

            var resultado = await servicio.Desactivar(unidad.Id);

            Assert.True(resultado);
            var enBd = await contexto.UnidadesMedida.FindAsync(unidad.Id);
            Assert.NotNull(enBd);
            Assert.False(enBd!.Activo);
        }

        [Fact]
        public async Task Desactivar_IdInexistente_RetornaFalse()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioUnidadesMedida(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.Desactivar(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task Listar_FiltraPorActivo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioUnidadesMedida(contexto, CrearReloj(), CrearProveedorUsuario());
            var activa = await servicio.Crear(new CrearUnidadMedidaDTO { Nombre = "Kilogramo", Abreviatura = "kg" });
            var inactiva = await servicio.Crear(new CrearUnidadMedidaDTO { Nombre = "Litro", Abreviatura = "l" });
            await servicio.Desactivar(inactiva.Id);

            var soloActivas = await servicio.Listar(activo: true);

            Assert.Single(soloActivas);
            Assert.Equal(activa.Id, soloActivas[0].Id);
        }
    }
}
