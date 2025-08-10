using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Models;            // EDMX
using LaVentaMusical.Helpers;
using LaVentaMusical.Models.ViewModels;

namespace LaVentaMusical.Controllers
{
    public class CancionesController : Controller
    {
        // ⬇️ Usa el nombre real de TU contexto
        private readonly PAV_PF_Grupo02Entities db = new PAV_PF_Grupo02Entities();

        public ActionResult Index()
        {
            var list = db.Canciones
                        .Where(c => c.CantidadDisponible > 0)
                        .OrderBy(c => c.NombreCancion)
                        .ToList();
            return View(list);
        }
        public ActionResult Details() { return View(); }
        [HttpPost]
        public ActionResult Agregar(string id, int cantidad = 1)
        {
            var song = db.Canciones.Find(id);
            if (song == null) return HttpNotFound();

            var cart = CarritoHelper.Get(Session);
            var line = cart.Lineas.FirstOrDefault(x => x.CancionID == id);
            if (line == null)
                cart.Lineas.Add(new CarritoLineaVM
                {
                    CancionID = id,
                    Nombre = song.NombreCancion,
                    Precio = song.Precio,
                    Cantidad = cantidad
                });
            else
                line.Cantidad += cantidad;

            TempData["Success"] = "Agregado al carrito.";
            return RedirectToAction("Index");
        }
    }
}
