using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Helpers;

namespace LaVentaMusical.Controllers
{
    public class CarritoController : Controller
    {
        public ActionResult Index()
        {
            var vm = CarritoHelper.Get(Session);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Actualizar(string id, int cantidad)
        {
            var cart = CarritoHelper.Get(Session);
            var line = cart.Lineas.FirstOrDefault(x => x.CancionID == id);
            if (line != null) line.Cantidad = (cantidad < 1) ? 1 : cantidad;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Quitar(string id)
        {
            var cart = CarritoHelper.Get(Session);
            cart.Lineas.RemoveAll(x => x.CancionID == id);
            return RedirectToAction("Index");
        }

        public ActionResult Vaciar()
        {
            CarritoHelper.Clear(Session);
            return RedirectToAction("Index");
        }
    }
}
