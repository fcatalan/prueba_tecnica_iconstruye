using FacturaSii.src.FacturaComponent.Domain.Entities;

namespace FacturaSii.src.FacturaComponent.Application.Interfaces
{
    public interface IServicioFirmaElectronica
    {
        string Firmar(Factura factura);
    }
}
