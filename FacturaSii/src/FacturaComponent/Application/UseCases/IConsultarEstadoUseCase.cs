namespace FacturaSii.src.FacturaComponent.Application.UseCases
{
    public interface IConsultarEstadoUseCase
    {
        Task<string?> ConsultarEstadoAsync(int folio);
    }
}