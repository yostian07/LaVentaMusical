using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using LaVentaMusical.Helpers;
using LaVentaMusical.Infrastructure;
using LaVentaMusical.Models.ViewModels;
using LaVentaMusical.Services;
using LaVentaMusical.Models;

namespace LaVentaMusical.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PAV_PF_Grupo02Entities db = new PAV_PF_Grupo02Entities();

        // GET: /Checkout
        public ActionResult Index()
        {
            var cart = CarritoHelper.Get(Session);
            bool demoMode = bool.Parse(ConfigurationManager.AppSettings["DemoMode"] ?? "false");

            var vm = new CheckoutVM
            {
                Carrito = cart,
                Subtotal = cart.Subtotal,
                IVA = Math.Round(cart.Subtotal * 0.13m, 2),
                DineroDisponibleUsuario = demoMode ? DemoStore.DemoDineroDisponible : GetUserAvailableMoney()
            };

            return View(vm);
        }

        // POST: /Checkout/Pagar
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Pagar(CheckoutVM input)
        {
            var cart = CarritoHelper.Get(Session);
            bool demoMode = bool.Parse(ConfigurationManager.AppSettings["DemoMode"] ?? "false");

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
            var dineroDisponible = demoMode ? DemoStore.DemoDineroDisponible : GetUserAvailableMoney();
            var creditoAplicar = Math.Min(dineroDisponible, bruto);
            var total = bruto - creditoAplicar;

            if (demoMode)
            {
                return ProcessDemoPayment(cart, input, subtotal, iva, comision, creditoAplicar, total);
            }
            else
            {
                return ProcessRealPayment(cart, input, subtotal, iva, comision, creditoAplicar, total);
            }
        }

        private ActionResult ProcessDemoPayment(CarritoVM cart, CheckoutVM input, decimal subtotal, decimal iva, decimal comision, decimal creditoAplicar, decimal total)
        {
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

            return FinalizarCompra(venta, DemoStore.DemoUserEmail);
        }

        private ActionResult ProcessRealPayment(CarritoVM cart, CheckoutVM input, decimal subtotal, decimal iva, decimal comision, decimal creditoAplicar, decimal total)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Validar stock
                    foreach (var l in cart.Lineas)
                    {
                        var cancion = db.Canciones.Find(l.CancionID);
                        if (cancion == null || cancion.CantidadDisponible < l.Cantidad)
                        {
                            TempData["error"] = $"Stock insuficiente para '{cancion?.NombreCancion ?? "canción"}'. Disponible: {cancion?.CantidadDisponible ?? 0}.";
                            return RedirectToAction("Index");
                        }
                    }

                    // Crear venta en base de datos
                    var venta = new Venta
                    {
                        UsuarioID = GetCurrentUserId(),
                        FechaCompra = DateTime.Now,
                        TipoPago = input.MetodoPago,
                        NumeroTarjetaEnmascarado = input.Ultimos4 != null ? $"****-****-****-{input.Ultimos4}" : null,
                        CodigoTarjeta = input.Ultimos4,
                        Subtotal = subtotal,
                        IVA = iva,
                        ComisionTarjeta = comision,
                        Total = total,
                        DineroUtilizado = creditoAplicar,
                        EsReversada = false
                    };

                    db.Ventas.Add(venta);
                    db.SaveChanges(); // Para obtener el VentaID

                    // Crear detalles de venta
                    foreach (var l in cart.Lineas)
                    {
                        var detalle = new DetalleVenta
                        {
                            VentaID = venta.VentaID,
                            CancionID = l.CancionID,
                            PrecioUnitario = l.Precio,
                            Cantidad = l.Cantidad,
                            Subtotal = l.SubtotalLinea // ✅ Corrección: usar SubtotalLinea
                        };
                        db.DetalleVenta.Add(detalle);

                        // Descontar stock
                        var cancion = db.Canciones.Find(l.CancionID);
                        cancion.CantidadDisponible -= l.Cantidad;
                    }

                    // Actualizar dinero disponible del usuario
                    var usuario = GetCurrentUser();
                    if (usuario != null)
                    {
                        usuario.DineroDisponible -= creditoAplicar;
                        if (usuario.DineroDisponible < 0) usuario.DineroDisponible = 0;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // Convertir a DemoVenta para mantener compatibilidad
                    var ventaDemo = new DemoVenta
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
                        Detalles = cart.Lineas.Select(l => new DemoVentaDetalle
                        {
                            CancionID = l.CancionID,
                            Nombre = l.Nombre,
                            Cantidad = l.Cantidad,
                            PrecioUnitario = l.Precio,
                            Subtotal = l.SubtotalLinea // ✅ Corrección: usar SubtotalLinea
                        }).ToList()
                    };

                    return FinalizarCompra(ventaDemo, usuario?.CorreoElectronico ?? "cliente@demo.com");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["error"] = "Error procesando el pago: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }

        private ActionResult FinalizarCompra(DemoVenta venta, string email)
        {
            // Generar PDF
            var pdfBytes = PdfService.BuildInvoice(venta); // ✅ Corrección: cambiar nombre de variable

            // Enviar por correo
            try
            {
                InvoiceEmailService.Send(
                    email,
                    $"Factura {venta.NumeroFactura}",
                    pdfBytes, // ✅ Usar nuevo nombre de variable
                    $"{venta.NumeroFactura}.pdf"
                );
            }
            catch (Exception ex)
            {
                TempData["warn"] = "Factura generada, pero el correo no se pudo enviar: " + ex.Message;
            }

            // Guardar copia local del PDF
            try
            {
                var path = Server.MapPath("~/App_Data/Facturas");
                System.IO.Directory.CreateDirectory(path);
                System.IO.File.WriteAllBytes(System.IO.Path.Combine(path, venta.NumeroFactura + ".pdf"), pdfBytes); // ✅ Usar nuevo nombre de variable
            }
            catch { /* ignorar */ }

            // Vaciar carrito
            CarritoHelper.Clear(Session);

            TempData["ok"] = $"Compra realizada. Factura {venta.NumeroFactura}";
            return RedirectToAction("Detalle", "Ordenes", new { id = venta.VentaID });
        }

        private decimal GetUserAvailableMoney()
        {
            // Implementar lógica para obtener dinero disponible del usuario actual
            // Por ahora retorna un valor por defecto
            return 1000m;
        }

        private int GetCurrentUserId()
        {
            // Implementar lógica para obtener ID del usuario actual
            // Por ahora retorna 1 (usuario demo)
            return 1;
        }

        private Usuario GetCurrentUser()
        {
            // Implementar lógica para obtener usuario actual
            // Por ahora retorna el primer usuario o null
            return db.Usuarios.FirstOrDefault();
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
