using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Infrastructure;
using LaVentaMusical.Services;
using LaVentaMusical.Models;

namespace LaVentaMusical.Controllers
{
    public class OrdenesController : Controller
    {
        private readonly PAV_PF_Grupo02Entities db = new PAV_PF_Grupo02Entities();

        public ActionResult Index()
        {
            bool demoMode = bool.Parse(ConfigurationManager.AppSettings["DemoMode"] ?? "false");
            
            if (demoMode)
            {
                var list = DemoStore.Ventas.OrderByDescending(v => v.FechaCompra).ToList();
                return View(list);
            }

            // Usar base de datos real
            var ventas = db.Ventas
                          .Include("Usuario")
                          .Include("Detalles")
                          .Include("Detalles.Cancion")
                          .Where(v => !v.EsReversada)
                          .OrderByDescending(v => v.FechaCompra)
                          .ToList();

            // Convertir a DemoVenta para mantener compatibilidad con la vista
            var ventasConverted = ventas.Select(v => new DemoVenta
            {
                VentaID = v.VentaID,
                NumeroFactura = v.NumeroFactura,
                FechaCompra = v.FechaCompra,
                MetodoPago = v.TipoPago,
                TarjetaEnmascarada = v.NumeroTarjetaEnmascarado,
                Subtotal = v.Subtotal,
                IVA = v.IVA,
                ComisionTarjeta = v.ComisionTarjeta,
                DineroUtilizado = v.DineroUtilizado,
                Total = v.Total,
                Detalles = v.Detalles.Select(d => new DemoVentaDetalle
                {
                    CancionID = d.CancionID,
                    Nombre = d.Cancion.NombreCancion,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            }).ToList();

            return View(ventasConverted);
        }

        public ActionResult Detalle(int id)
        {
            bool demoMode = bool.Parse(ConfigurationManager.AppSettings["DemoMode"] ?? "false");
            
            if (demoMode)
            {
                var v = DemoStore.Ventas.FirstOrDefault(x => x.VentaID == id);
                if (v == null) return HttpNotFound();
                return View(v);
            }

            // Usar base de datos real
            var venta = db.Ventas
                         .Include("Usuario")
                         .Include("Detalles")
                         .Include("Detalles.Cancion")
                         .FirstOrDefault(v => v.VentaID == id && !v.EsReversada);

            if (venta == null) return HttpNotFound();

            // Convertir a DemoVenta para mantener compatibilidad con la vista
            var ventaConverted = new DemoVenta
            {
                VentaID = venta.VentaID,
                NumeroFactura = venta.NumeroFactura,
                FechaCompra = venta.FechaCompra,
                MetodoPago = venta.TipoPago,
                TarjetaEnmascarada = venta.NumeroTarjetaEnmascarado,
                Subtotal = venta.Subtotal,
                IVA = venta.IVA,
                ComisionTarjeta = venta.ComisionTarjeta,
                DineroUtilizado = venta.DineroUtilizado,
                Total = venta.Total,
                Detalles = venta.Detalles.Select(d => new DemoVentaDetalle
                {
                    CancionID = d.CancionID,
                    Nombre = d.Cancion.NombreCancion,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };

            return View(ventaConverted);
        }

        public ActionResult Pdf(int id)
        {
            bool demoMode = bool.Parse(ConfigurationManager.AppSettings["DemoMode"] ?? "false");
            
            if (demoMode)
            {
                var v = DemoStore.Ventas.FirstOrDefault(x => x.VentaID == id);
                if (v == null) return HttpNotFound();
                var pdfBytes = PdfService.BuildInvoice(v); // ✅ Cambié el nombre de la variable
                return File(pdfBytes, "application/pdf", v.NumeroFactura + ".pdf");
            }

            // Usar base de datos real
            var venta = db.Ventas
                         .Include("Usuario")
                         .Include("Detalles")
                         .Include("Detalles.Cancion")
                         .FirstOrDefault(v => v.VentaID == id && !v.EsReversada);

            if (venta == null) return HttpNotFound();

            // Convertir a DemoVenta para el PDF
            var ventaConverted = new DemoVenta
            {
                VentaID = venta.VentaID,
                NumeroFactura = venta.NumeroFactura,
                FechaCompra = venta.FechaCompra,
                MetodoPago = venta.TipoPago,
                TarjetaEnmascarada = venta.NumeroTarjetaEnmascarado,
                Subtotal = venta.Subtotal,
                IVA = venta.IVA,
                ComisionTarjeta = venta.ComisionTarjeta,
                DineroUtilizado = venta.DineroUtilizado,
                Total = venta.Total,
                Detalles = venta.Detalles.Select(d => new DemoVentaDetalle
                {
                    CancionID = d.CancionID,
                    Nombre = d.Cancion.NombreCancion,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };

            var pdfDocument = PdfService.BuildInvoice(ventaConverted); // ✅ Cambié el nombre de la variable
            return File(pdfDocument, "application/pdf", venta.NumeroFactura + ".pdf");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
