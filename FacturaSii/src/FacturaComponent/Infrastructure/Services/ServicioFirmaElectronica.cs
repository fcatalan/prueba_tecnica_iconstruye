using FacturaSii.src.FacturaComponent.Application.Interfaces;
using FacturaSii.src.FacturaComponent.Domain.Entities;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace FacturaSii.src.FacturaComponent.Infrastructure.Servicios
{
    public class ServicioFirmaElectronica : IServicioFirmaElectronica
    {
        private readonly X509Certificate2 _cert;

        public ServicioFirmaElectronica(IConfiguration config)
        {
            var path = config["Certificate:Path"];
            var pwd  = config["Certificate:Password"];
            _cert = new X509Certificate2(path, pwd, X509KeyStorageFlags.MachineKeySet);
        }

        public string Firmar(Factura factura)
        {
            var doc = GenerarXml(factura);
            AplicarFirma(doc);
            return doc.OuterXml;
        }

        private XmlDocument GenerarXml(Factura factura)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            // <DTE version="1.0">
            var root = doc.CreateElement("DTE");
            var versionAttr = doc.CreateAttribute("version");
            versionAttr.Value = "1.0";
            root.Attributes.Append(versionAttr);
            doc.AppendChild(root);

            // <Documento ID="FacturaElectronica_{folio}">
            var documento = doc.CreateElement("Documento");
            var idAttr = doc.CreateAttribute("ID");
            idAttr.Value = $"FacturaElectronica_{factura.Folio}";
            documento.Attributes.Append(idAttr);
            root.AppendChild(documento);

            // <Encabezado>
            var encabezado = doc.CreateElement("Encabezado");
            documento.AppendChild(encabezado);

            //   <IdDoc><Folio>...</Folio></IdDoc>
            var IdDoc = doc.CreateElement("IdDoc");
            var Folio = doc.CreateElement("Folio");
            Folio.InnerText = factura.Folio.ToString();
            IdDoc.AppendChild(Folio);
            encabezado.AppendChild(IdDoc);

            //   <Emisor><RUTEmisor>...</RUTEmisor></Emisor>
            var emisor = doc.CreateElement("Emisor");
            var rutEm = doc.CreateElement("RUTEmisor");
            rutEm.InnerText = factura.RutEmisor;
            emisor.AppendChild(rutEm);
            encabezado.AppendChild(emisor);

            //   <Receptor><RUTRecep>...</RUTRecep></Receptor>
            var receptor = doc.CreateElement("Receptor");
            var rutRec = doc.CreateElement("RUTRecep");
            rutRec.InnerText = factura.RutReceptor;
            receptor.AppendChild(rutRec);
            encabezado.AppendChild(receptor);

            //   <Totales>...
            var totales = doc.CreateElement("Totales");
            void addTot(string name, string val)
            {
                var el = doc.CreateElement(name);
                el.InnerText = val;
                totales.AppendChild(el);
            }
            addTot("MntNeto",   factura.MontoNeto.ToString());
            addTot("TasaIVA",   "19");
            addTot("IVA",       factura.IVA.ToString());
            addTot("MntTotal",  factura.Total.ToString());
            encabezado.AppendChild(totales);

            // <Detalle> por cada línea
            int linea = 1;
            foreach (var item in factura.Items)
            {
                var detalle = doc.CreateElement("Detalle");
                
                void addDet(string name, string val)
                {
                    var el = doc.CreateElement(name);
                    el.InnerText = val;
                    detalle.AppendChild(el);
                }

                addDet("NroLinDet",  linea.ToString());
                addDet("NmbItem",    item.Descripcion);
                addDet("QtyItem",    item.Cantidad.ToString());
                addDet("PrcItem",    item.PrecioUnitario.ToString());
                addDet("MontoItem",  (item.Cantidad * item.PrecioUnitario).ToString());

                documento.AppendChild(detalle);
                linea++;
            }

            return doc;
        }

        private void AplicarFirma(XmlDocument doc)
        {
            var signedXml = new SignedXml(doc) { SigningKey = _cert.GetRSAPrivateKey() };
            var reference = new Reference(string.Empty);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(_cert));
            signedXml.KeyInfo = keyInfo;
            signedXml.ComputeSignature();
            var sig = signedXml.GetXml();
            doc.DocumentElement.AppendChild(doc.ImportNode(sig, true));
        }
    }
}