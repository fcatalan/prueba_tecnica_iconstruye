namespace FacturaSii.src.FacturaComponent.Application.Interfaces
{
    public interface IServicioEnvioSII
    {
        Task<string> EnviarAsync(string xmlFirmado);
    }
}