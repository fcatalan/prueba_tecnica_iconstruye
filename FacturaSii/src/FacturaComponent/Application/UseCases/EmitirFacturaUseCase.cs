using FacturaSii.src.FacturaComponent.Application.Interfaces;
using FacturaSii.src.FacturaComponent.Domain.Entities;
using FacturaSii.src.FacturaComponent.Domain.Interfaces;
using FacturaSii.src.FacturaComponent.Domain.Services;

namespace FacturaSii.src.FacturaComponent.Application.UseCases
{
    public class EmitirFacturaUseCase
    {
        private readonly IFacturaRepository _repo;
        private readonly IServicioFirmaElectronica _firma;
        private readonly IServicioEnvioSII _envio;
        private readonly IValidadorDeFolio _validadorDeFolio;

        public EmitirFacturaUseCase(IFacturaRepository repo,
                                    IServicioFirmaElectronica firma,
                                    IServicioEnvioSII envio,
                                    IValidadorDeFolio validadorDeFolio)
        {
            _repo = repo;
            _firma = firma;
            _envio = envio;
            _validadorDeFolio = validadorDeFolio;
        }

        public async Task<string> EjecutarAsync(Factura factura)
        {
            if (!_validadorDeFolio.EsValido(factura.Folio))
                throw new InvalidOperationException($"El folio {factura.Folio} no está dentro del rango autorizado.");

            if (await _repo.ExistePorFolioAsync(factura.Folio))
                throw new InvalidOperationException($"Ya existe una factura con el folio {factura.Folio}.");

            var ultimoFolio = await _repo.ObtenerUltimoFolioAsync();
            if (ultimoFolio.HasValue && factura.Folio != ultimoFolio.Value + 1)
                throw new InvalidOperationException($"El folio debe ser correlativo. El último folio registrado fue {ultimoFolio.Value}.");

            var xmlFirmado = _firma.Firmar(factura);
            var resultado = await _envio.EnviarAsync(xmlFirmado);

            factura.XmlGenerado = xmlFirmado;
            factura.EstadoRespuesta = resultado;

            await _repo.AgregarAsync(factura);
            return resultado;
        }
    }
}
