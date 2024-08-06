using CERVERICA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stocks")]
        public async Task<IActionResult> GetStocks()
        {
            var stocks = await _context.Stocks.Where(p => p.Cantidad > 0).Include(s => s.Receta).ToListAsync();

            var groupedStocks = stocks.GroupBy(s => s.Receta)
                .Select(gr => new
                {
                    RecetaId = gr.Key.Id,
                    Nombre = gr.Key.Nombre,
                    Imagen = gr.Key.Imagen,
                    PrecioLitro = gr.Key.PrecioLitro,
                    Activo = gr.Key.Activo,
                    StocksPorEnvase = gr.GroupBy(s => new { s.TipoEnvase, s.MedidaEnvase })
                                        .Select(g => new
                                        {
                                            TipoEnvase = g.Key.TipoEnvase,
                                            MedidaEnvase = g.Key.MedidaEnvase,
                                            CantidadTotal = g.Sum(s => s.Cantidad),
                                            PrecioPorEnvase = g.Key.MedidaEnvase * (gr.Key.PrecioLitro / 1000)
                                        })
                })
                .ToList();

            return Ok(groupedStocks);
        }
    }
}
