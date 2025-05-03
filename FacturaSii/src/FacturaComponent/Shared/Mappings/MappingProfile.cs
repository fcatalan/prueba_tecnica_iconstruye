using AutoMapper;
using FacturaSii.src.FacturaComponent.Domain.Entities;
using FacturaSii.src.FacturaComponent.Domain.ValueObjects;
using FacturaSii.src.FacturaComponent.Shared.DTOs;


namespace Shared.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FacturaDto, Factura>()
            .ConstructUsing(dto => new Factura(
                dto.Folio,
                dto.RutEmisor,
                dto.RutReceptor,
                dto.Items.Select(i => new Item(i.Descripcion, i.Cantidad, i.PrecioUnitario))
            ))
            .ForMember(dest => dest.Items, opt => opt.Ignore());
        }
    }
}