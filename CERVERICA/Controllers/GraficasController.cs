using CERVERICA.Data;
using CERVERICA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class GraficasController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public GraficasController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet("nuevos-clientes-por-mes")]
        public async Task<IActionResult> GetNuevosClientesPorMes()
        {
            // Obtiene todos los usuarios con el rol "Cliente"
            var usuariosClientes = await _userManager.GetUsersInRoleAsync("Cliente");

            // Agrupa los usuarios por año y mes de registro
            var result = usuariosClientes
                .GroupBy(u => new { u.FechaRegistro?.Year, u.FechaRegistro?.Month })
                .Select(g => new
                {
                    Mes = g.Key.Month,
                    Año = g.Key.Year,
                    NuevosClientes = g.Count()
                })
                .OrderBy(r => r.Año).ThenBy(r => r.Mes)
                .ToList();

            return Ok(result);
        }

        [HttpGet("ventas-por-estatus")]
        public async Task<IActionResult> GetVentasPorEstatus()
        {
            var result = await _db.Ventas
                .GroupBy(v => v.EstatusVenta)
                .Select(g => new
                {
                    Estatus = g.Key.ToString(),
                    Cantidad = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("ingresos-por-mes")]
        public async Task<ActionResult> ObtenerIngresosPorMes()
        {
            var ingresosPorMes = await _db.Ventas
       .SelectMany(v => v.DetallesVenta, (venta, detalle) => new { venta.FechaVenta.Year, venta.FechaVenta.Month, detalle.MontoVenta })
       .GroupBy(x => new { x.Year, x.Month })
       .Select(g => new 
       {
           Ano = g.Key.Year,
           Mes = g.Key.Month,
           IngresosTotales = g.Sum(x => x.MontoVenta)
       })
       .ToListAsync();

            if (!ingresosPorMes.Any())
            {
                return NotFound();
            }

            return Ok(ingresosPorMes);
        }



        [HttpGet("productos-mas-vendidos")]
        public async Task<IActionResult> GetProductosMasVendidos()
        {
            var result = await _db.DetallesVenta
                .GroupBy(dv => dv.Stock.Receta.Nombre)
                .Select(g => new
                {
                    Producto = g.Key,
                    CantidadVendida = g.Sum(dv => dv.Cantidad)
                })
                .OrderByDescending(r => r.CantidadVendida)
                .Take(10) // Limitar a los 10 productos más vendidos
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("pedidos-por-metodo-pago")]
        public async Task<IActionResult> GetPedidosPorMetodoPago()
        {
            var result = await _db.Ventas
                .GroupBy(v => v.MetodoPago)
                .Select(g => new
                {
                    MetodoPago = g.Key.ToString(),
                    Cantidad = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }


    }
}
