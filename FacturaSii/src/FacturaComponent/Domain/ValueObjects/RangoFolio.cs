// /FacturaElectronica.Domain/ValueObjects/RangoFolio.cs
public class RangoFolio
{
    public int Desde { get; }
    public int Hasta { get; }

    public RangoFolio(int desde, int hasta)
    {
        if (desde <= 0 || hasta <= 0)
            throw new ArgumentException("Rango inválido: los valores deben ser mayores que cero.");
        if (desde > hasta)
            throw new ArgumentException("Rango inválido: 'Desde' no puede ser mayor que 'Hasta'.");

        Desde = desde;
        Hasta = hasta;
    }

    public bool Contiene(int folio) => folio >= Desde && folio <= Hasta;
}
