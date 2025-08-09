using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Infrastructure;
using LaVentaMusical.Helpers;
using LaVentaMusical.Models.ViewModels;

namespace LaVentaMusical.Controllers
{
    public class CancionesController : Controller
    {
        public ActionResult Index()
        {
            var list = DemoStore.Canciones.Where(c => c.CantidadDisponible > 0).ToList();
            return View(list);
        }

        [HttpPost]
        public ActionResult Agregar(string id, int cantidad = 1)
        {
            var song = DemoStore.Canciones.FirstOrDefault(s => s.CancionID == id);
            if (song == null) return HttpNotFound();

            var cart = CarritoHelper.Get(Session);
            var line = cart.Lineas.FirstOrDefault(x => x.CancionID == id);
            if (line == null)
                cart.Lineas.Add(new CarritoLineaVM { CancionID = id, Nombre = song.Nombre, Precio = song.Precio, Cantidad = cantidad });
            else
                line.Cantidad += cantidad;

            TempData["ok"] = "Agregado al carrito.";
            return RedirectToAction("Index");
        }
    }
}
