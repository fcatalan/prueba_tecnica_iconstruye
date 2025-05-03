using FacturaSii.src.FacturaComponent.Domain.Interfaces;

namespace FacturaSii.src.FacturaComponent.Application.UseCases
{
    public class ConsultarEstadoUseCase : IConsultarEstadoUseCase
    {
        private readonly IFacturaRepository _repo;

        public ConsultarEstadoUseCase(IFacturaRepository repo)
        {
            _repo = repo;
        }

        public async Task<string?> ConsultarEstadoAsync(int folio)
        {
            var factura = await _repo.ObtenerPorFolioAsync(folio);
            return factura?.EstadoRespuesta;
        }
    }
}
