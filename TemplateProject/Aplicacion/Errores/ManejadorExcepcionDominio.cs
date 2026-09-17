using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Dominio.Comun;

namespace TemplateProject.Aplicacion.Errores
{
    public class ManejadorExcepcionDominio : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken cancellationToken)
        {
            if (excepcion is not ExcepcionDominio)
                return false;

            var problema = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Regla de negocio no cumplida",
                Detail = excepcion.Message
            };

            contexto.Response.StatusCode = StatusCodes.Status400BadRequest;
            await contexto.Response.WriteAsJsonAsync(problema, cancellationToken);
            return true;
        }
    }
}
