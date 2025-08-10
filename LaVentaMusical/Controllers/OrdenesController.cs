using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using LaVentaMusical.Services;
using LaVentaMusical.Models;

namespace LaVentaMusical.Controllers
{
    public class OrdenesController : Controller
    {
        private readonly PAV_PF_Grupo02Entities1 db = new PAV_PF_Grupo02Entities1();

        public ActionResult Index()
        {
            // Usar solo base de datos real
            var ventas = db.Ventas
                          .Include(v => v.Usuarios)
                          .Include(v => v.DetalleVenta)
                          .Include(v => v.DetalleVenta.Select(d => d.Canciones))
                          .Where(v => !v.EsReversada)
                          .OrderByDescending(v => v.FechaCompra)
                          .ToList();

            return View(ventas);
        }

        public ActionResult Detalle(int id)
        {
            // Usar solo base de datos real
            var venta = db.Ventas
                         .Include(v => v.Usuarios)
                         .Include(v => v.DetalleVenta)
                         .Include(v => v.DetalleVenta.Select(d => d.Canciones))
                         .FirstOrDefault(v => v.VentaID == id && !v.EsReversada);

            if (venta == null) return HttpNotFound();

            return View(venta);
        }

        public ActionResult Pdf(int id)
        {
            // Usar solo base de datos real
            var venta = db.Ventas
                         .Include(v => v.Usuarios)
                         .Include(v => v.DetalleVenta)
                         .Include(v => v.DetalleVenta.Select(d => d.Canciones))
                         .FirstOrDefault(v => v.VentaID == id && !v.EsReversada);

            if (venta == null) return HttpNotFound();

            var pdfDocument = PdfService.BuildInvoice(venta);
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
