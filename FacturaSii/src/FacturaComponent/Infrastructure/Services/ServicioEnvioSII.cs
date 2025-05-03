using FacturaSii.src.FacturaComponent.Application.Interfaces;

namespace FacturaSii.src.FacturaComponent.Infrastructure.Servicios
{
    public class ServicioEnvioSII : IServicioEnvioSII
    {
        public Task<string> EnviarAsync(string xml)
        {
            var estados = new[] { "Aceptado", "Rechazado", "Con Reparos" };
            var rnd = new Random();
            return Task.FromResult(estados[rnd.Next(estados.Length)]);
        }
    }
}