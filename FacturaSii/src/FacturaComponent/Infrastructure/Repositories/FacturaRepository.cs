using FacturaSii.src.FacturaComponent.Domain.Entities;
using FacturaSii.src.FacturaComponent.Domain.Interfaces;
using FacturaSii.src.FacturaComponent.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FacturaSii.src.FacturaComponent.Infrastructure.Repositorios
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly AppDbContext _context;

        public FacturaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(Factura factura)
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistePorFolioAsync(int folio)
        {
            return await _context.Facturas.AnyAsync(f => f.Folio == folio);
        }


        public async Task<Factura?> ObtenerPorFolioAsync(int folio)
        {
            return await _context.Facturas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Folio == folio);
        }

        public async Task<int?> ObtenerUltimoFolioAsync()
        {
            return await _context.Facturas
                .OrderByDescending(f => f.Folio)
                .Select(f => (int?)f.Folio)
                .FirstOrDefaultAsync();
        }
    }
}
