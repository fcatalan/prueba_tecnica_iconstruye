using System.ComponentModel.DataAnnotations;

namespace FacturaSii.src.FacturaComponent.Shared.DTOs
{
    public class FacturaDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El folio debe ser un número positivo")]
        public int Folio { get; set; }

        
        //[ValidRut(ErrorMessage = "El RUT del emisor no es válido")]
        public required string RutEmisor { get; set; }

        
        //[ValidRut(ErrorMessage = "El RUT del receptor no es válido")]
        public required string RutReceptor { get; set; }

        
        [MinLength(1, ErrorMessage = "Debe haber al menos un item")]
        public required List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
}
