namespace FacturaSii.src.FacturaComponent.Domain.Services
{
    public class ValidadorDeFolio : IValidadorDeFolio
    {
        private readonly IRangoFolioProvider _rangoFolioProvider;

        public ValidadorDeFolio(IRangoFolioProvider rangoFolioProvider)
        {
            _rangoFolioProvider = rangoFolioProvider;
        }

        public bool EsValido(int folio)
        {
            var rango = _rangoFolioProvider.ObtenerRangoActual();
            return rango.Contiene(folio);
        }
    }
}
