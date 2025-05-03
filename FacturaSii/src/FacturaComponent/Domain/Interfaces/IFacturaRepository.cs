using FacturaSii.src.FacturaComponent.Domain.Entities;

namespace FacturaSii.src.FacturaComponent.Domain.Interfaces
{
    public interface IFacturaRepository
    {
        Task AgregarAsync(Factura factura);
        Task<Factura?> ObtenerPorFolioAsync(int folio);
        Task<bool> ExistePorFolioAsync(int folio);
        Task<int?> ObtenerUltimoFolioAsync();
    }
}