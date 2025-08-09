using System;
using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Helpers;
using LaVentaMusical.Infrastructure;
using LaVentaMusical.Models.ViewModels;
using LaVentaMusical.Services;

namespace LaVentaMusical.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: /Checkout
        public ActionResult Index()
        {
            var cart = CarritoHelper.Get(Session);

            var vm = new CheckoutVM
            {
                Carrito = cart,
                Subtotal = cart.Subtotal,
                IVA = Math.Round(cart.Subtotal * 0.13m, 2),
                DineroDisponibleUsuario = DemoStore.DemoDineroDisponible
            };

            return View(vm);
        }

        // POST: /Checkout/Pagar
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Pagar(CheckoutVM input)
        {
            var cart = CarritoHelper.Get(Session);

            if (!cart.Lineas.Any())
            {
                TempData["error"] = "El carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            // ====== Validación y manejo de datos de tarjeta ======
            if (input.MetodoPago == "Tarjeta de Crédito")
            {
                // Debe venir número de tarjeta completo y CVV
                if (string.IsNullOrWhiteSpace(input.NumeroTarjeta) || string.IsNullOrWhiteSpace(input.CVV))
                    ModelState.AddModelError("", "Ingrese el número de tarjeta y el CVV.");

                // Dejar solo dígitos (quita espacios/guiones)
                var digits = new string((input.NumeroTarjeta ?? "").Where(char.IsDigit).ToArray());

                // Validación básica de longitud (12–19)
                if (digits.Length < 12 || digits.Length > 19)
                    ModelState.AddModelError("NumeroTarjeta", "Número de tarjeta inválido.");

                // (Opcional) Validación Luhn:
                // if (!Luhn(digits)) ModelState.AddModelError("NumeroTarjeta", "Número de tarjeta inválido.");

                // Calcular últimos 4 y DESCARTAR el número completo por seguridad
                input.Ultimos4 = digits.Length >= 4 ? digits.Substring(digits.Length - 4) : null;
                input.NumeroTarjeta = null; // no persistimos el PAN
            }
            else
            {
                // Si no es tarjeta, no guardamos datos de tarjeta
                input.NumeroTarjeta = null;
                input.CVV = null;
                input.Ultimos4 = null;
            }

            if (!ModelState.IsValid) return View("Index", input);
            // ====== Fin manejo tarjeta ======

            // Cálculos
            var subtotal = cart.Subtotal;
            var iva = Math.Round(subtotal * 0.13m, 2);
            var comision = input.MetodoPago == "Tarjeta de Crédito" ? Math.Round(subtotal * 0.02m, 2) : 0m;
            var bruto = subtotal + iva + comision;

            // Aplicar nota de crédito (dinero disponible demo)
            var creditoAplicar = Math.Min(DemoStore.DemoDineroDisponible, bruto);
            var total = bruto - creditoAplicar;

            // Validar stock
            foreach (var l in cart.Lineas)
            {
                var s = DemoStore.Canciones.First(x => x.CancionID == l.CancionID);
                if (s.CantidadDisponible < l.Cantidad)
                {
                    TempData["error"] = $"Stock insuficiente para '{s.Nombre}'. Disponible: {s.CantidadDisponible}.";
                    return RedirectToAction("Index");
                }
            }

            // Descontar stock
            foreach (var l in cart.Lineas)
            {
                var s = DemoStore.Canciones.First(x => x.CancionID == l.CancionID);
                s.CantidadDisponible -= l.Cantidad;
            }

            // Actualizar crédito demo
            DemoStore.DemoDineroDisponible -= creditoAplicar;
            if (DemoStore.DemoDineroDisponible < 0) DemoStore.DemoDineroDisponible = 0;

            // Crear "venta" en memoria
            var venta = DemoStore.CrearVentaDesdeCarrito(cart, input.MetodoPago, input.Ultimos4, creditoAplicar);
            venta.IVA = iva;
            venta.ComisionTarjeta = comision;
            venta.Total = total;

            // Generar PDF
            var pdf = PdfService.BuildInvoice(venta);

            // Enviar por correo (SMTP configurado en Web.config)
            try
            {
                InvoiceEmailService.Send(
                    DemoStore.DemoUserEmail,
                    $"Factura {venta.NumeroFactura}",
                    pdf,
                    $"{venta.NumeroFactura}.pdf"
                );
            }
            catch (Exception ex)
            {
                TempData["warn"] = "Factura generada, pero el correo no se pudo enviar: " + ex.Message;
            }

            //  guardar copia local del PDF
            try
            {
                var path = Server.MapPath("~/App_Data/Facturas");
                System.IO.Directory.CreateDirectory(path);
                System.IO.File.WriteAllBytes(System.IO.Path.Combine(path, venta.NumeroFactura + ".pdf"), pdf);
            }
            catch { /* ignorar en demo */ }

            // Vaciar carrito
            CarritoHelper.Clear(Session);

            TempData["ok"] = $"Compra realizada. Factura {venta.NumeroFactura}";
            return RedirectToAction("Detalle", "Ordenes", new { id = venta.VentaID });
        }

       
        private bool Luhn(string s)
        {
            int sum = 0; bool alt = false;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                int n = s[i] - '0';
                if (alt) { n *= 2; if (n > 9) n -= 9; }
                sum += n; alt = !alt;
            }
            return (sum % 10) == 0;
        }
    }
}
