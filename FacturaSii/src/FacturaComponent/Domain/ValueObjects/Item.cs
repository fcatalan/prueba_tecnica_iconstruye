namespace FacturaSii.src.FacturaComponent.Domain.ValueObjects
{
    public class Item
    {
        public string Descripcion { get; private set; }
        public int Cantidad { get; private set; }
        public decimal PrecioUnitario { get; private set; }
        public decimal Total => Cantidad * PrecioUnitario;

        public Item(string descripcion, int cantidad, decimal precioUnitario)
        {
            Descripcion = descripcion;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }
    }
}
