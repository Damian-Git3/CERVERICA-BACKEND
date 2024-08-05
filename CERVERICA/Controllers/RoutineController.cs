using CERVERICA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Controllers
{
    public class RoutineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RoutineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AsignarLotesInsumoCaducados()
        {
            var insumosCaducados = await _context.LotesInsumos
                .Where(l => l.FechaCaducidad < DateTime.UtcNow && l.Cantidad > 0)
                .ToListAsync();

            foreach (var lote in insumosCaducados)
            {
                var otroLote = await _context.LotesInsumos
                    .Where(l => l.IdInsumo == lote.IdInsumo && l.Id != lote.Id && l.Cantidad > 0)
                    .FirstOrDefaultAsync();

                var cantidad = lote.Cantidad;
                lote.PrecioUnidad = lote.MontoCompra / lote.Cantidad;

                if (otroLote != null)
                {
                    float nuevoPrecioUnidad = (otroLote.PrecioUnidad * otroLote.Cantidad + lote.PrecioUnidad * cantidad) / otroLote.Cantidad;
                    otroLote.PrecioUnidad = nuevoPrecioUnidad;
                    _context.Entry(otroLote).State = EntityState.Modified;
                    lote.PrecioUnidad = (lote.PrecioUnidad * lote.Cantidad) / 1;
                }
                lote.Merma += cantidad;
                lote.Caducado = cantidad;
                lote.Cantidad = 0;
                _context.Entry(lote).State = EntityState.Modified;

                await RecalcularCostoUnitario(lote.IdInsumo);
            }

            await _context.SaveChangesAsync();
        }

        private async Task RecalcularCostoUnitario(int idInsumo)
        {
            var insumo = await _context.Insumos.Include(i => i.LotesInsumos).FirstOrDefaultAsync(i => i.Id == idInsumo);
            if (insumo != null && insumo.LotesInsumos != null && insumo.LotesInsumos.Any())
            {
                var sumaPreciosPorCantidad = insumo.LotesInsumos.Sum(l => l.PrecioUnidad * l.Cantidad);
                var sumaCantidades = insumo.LotesInsumos.Sum(l => l.Cantidad);

                if (sumaCantidades == 0)
                {
                    insumo.CostoUnitario = 0;
                }
                else
                {
                    insumo.CostoUnitario = (float)(sumaPreciosPorCantidad / sumaCantidades);
                }

                _context.Entry(insumo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
