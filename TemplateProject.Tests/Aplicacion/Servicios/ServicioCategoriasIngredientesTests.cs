using Microsoft.EntityFrameworkCore;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.Dominio.Comun;
using TemplateProject.DTOs.CategoriasIngredientes;
using Xunit;

namespace TemplateProject.Tests.Aplicacion.Servicios
{
    public class ServicioCategoriasIngredientesTests : BaseServicioTests
    {
        [Fact]
        public async Task Crear_DatosValidos_PersisteYSellaAutoria()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario("usuario-1"));

            var resultado = await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos", Descripcion = "Productos lácteos" });

            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Equal("usuario-1", resultado.UsuarioCreacionId);
            Assert.Equal(InstanteInicial.UtcDateTime, resultado.FechaCreacion);
            Assert.Equal(1, await contexto.CategoriasIngredientes.CountAsync());
        }

        [Fact]
        public async Task Crear_NombreDuplicado_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());
            await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos" });

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos" }));
        }

        [Fact]
        public async Task Actualizar_NombreDuplicadoDeOtraCategoria_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());
            await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos" });
            var carnes = await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Carnes" });

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Actualizar(carnes.Id, new ActualizarCategoriaIngredienteDTO { Nombre = "Lácteos" }));
        }

        [Fact]
        public async Task Actualizar_MismoNombrePropio_NoLanzaExcepcion()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());
            var categoria = await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos" });

            var resultado = await servicio.Actualizar(categoria.Id, new ActualizarCategoriaIngredienteDTO { Nombre = "Lácteos", Descripcion = "Nueva descripción" });

            Assert.NotNull(resultado);
            Assert.Equal("Nueva descripción", resultado!.Descripcion);
        }

        [Fact]
        public async Task ObtenerPorId_IdInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.ObtenerPorId(Guid.NewGuid());

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Actualizar_IdInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.Actualizar(Guid.NewGuid(), new ActualizarCategoriaIngredienteDTO { Nombre = "Lácteos" });

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Desactivar_CategoriaExistente_QuedaInactivaSinBorrarse()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());
            var categoria = await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos" });

            var resultado = await servicio.Desactivar(categoria.Id);

            Assert.True(resultado);
            var enBd = await contexto.CategoriasIngredientes.FindAsync(categoria.Id);
            Assert.NotNull(enBd);
            Assert.False(enBd!.Activo);
        }

        [Fact]
        public async Task Desactivar_IdInexistente_RetornaFalse()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.Desactivar(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task Listar_FiltraPorActivo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioCategoriasIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());
            var activa = await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Lácteos" });
            var inactiva = await servicio.Crear(new CrearCategoriaIngredienteDTO { Nombre = "Carnes" });
            await servicio.Desactivar(inactiva.Id);

            var soloActivas = await servicio.Listar(activo: true);

            Assert.Single(soloActivas);
            Assert.Equal(activa.Id, soloActivas[0].Id);
        }
    }
}
