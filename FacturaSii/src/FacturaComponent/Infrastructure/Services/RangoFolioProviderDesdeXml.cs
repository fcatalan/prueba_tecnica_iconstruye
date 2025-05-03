
using System.Xml;
using FacturaSii.src.FacturaComponent.Domain.Services;

public class RangoFolioProviderDesdeXml : IRangoFolioProvider
{
    private readonly string _rutaXml;

    public RangoFolioProviderDesdeXml(string rutaXml)
    {
        _rutaXml = rutaXml;
    }

    public RangoFolio ObtenerRangoActual()
    {
        var doc = new XmlDocument();
        doc.Load(_rutaXml);

        var nodoD = doc.SelectSingleNode("//CAF/DA/RNG/D");
        var nodoH = doc.SelectSingleNode("//CAF/DA/RNG/H");

        if (nodoD == null || nodoH == null)
            throw new InvalidOperationException("CAF inválido: no se encontró el rango.");

        int desde = int.Parse(nodoD.InnerText);
        int hasta = int.Parse(nodoH.InnerText);

        return new RangoFolio(desde, hasta);
    }
}
