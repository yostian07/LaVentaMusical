using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Infrastructure;
using LaVentaMusical.Services;

namespace LaVentaMusical.Controllers
{
    public class OrdenesController : Controller
    {
        public ActionResult Index()
        {
            var list = DemoStore.Ventas.OrderByDescending(v => v.FechaCompra).ToList();
            return View(list);
        }

        public ActionResult Detalle(int id)
        {
            var v = DemoStore.Ventas.FirstOrDefault(x => x.VentaID == id);
            if (v == null) return HttpNotFound();
            return View(v);
        }

        public ActionResult Pdf(int id)
        {
            var v = DemoStore.Ventas.FirstOrDefault(x => x.VentaID == id);
            if (v == null) return HttpNotFound();
            var pdf = PdfService.BuildInvoice(v);
            return File(pdf, "application/pdf", v.NumeroFactura + ".pdf");
        }
    }
}
