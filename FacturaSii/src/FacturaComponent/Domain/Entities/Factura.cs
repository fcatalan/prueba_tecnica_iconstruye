using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaSii.src.FacturaComponent.Domain.ValueObjects;


namespace FacturaSii.src.FacturaComponent.Domain.Entities
{
    public class Factura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Folio { get; set; }

        public string RutEmisor { get; set; } = string.Empty; // Default value to avoid null

        public string RutReceptor { get; set; } = string.Empty; // Default value to avoid null

        public string EstadoRespuesta { get; set; }

        [Column(TypeName = "xml")]
        public string XmlGenerado { get; set; }

        public IReadOnlyCollection<Item> Items => _items.AsReadOnly();
        private readonly List<Item> _items = new();

        public decimal MontoNeto => _items.Sum(i => i.Total);
        public decimal IVA => Math.Round(MontoNeto * 0.19m, 2);
        public decimal Total => MontoNeto + IVA;

        public Factura(int folio, string rutEmisor, string rutReceptor, IEnumerable<Item> items)
        {
            Folio = folio;
            RutEmisor = rutEmisor;
            RutReceptor = rutReceptor;
            foreach (var item in items) _items.Add(item);
        }

        public Factura() { }
    }
}
