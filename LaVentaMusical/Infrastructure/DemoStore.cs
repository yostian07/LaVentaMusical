using System;
using System.Collections.Generic;
using System.Linq;
using LaVentaMusical.Models.ViewModels;

namespace LaVentaMusical.Infrastructure
{
    public class DemoCancion
    {
        public string CancionID { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int CantidadDisponible { get; set; }
    }

    public class DemoVentaDetalle
    {
        public string CancionID { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class DemoVenta
    {
        public int VentaID { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaCompra { get; set; }
        public string MetodoPago { get; set; }
        public string TarjetaEnmascarada { get; set; }

        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal ComisionTarjeta { get; set; }
        public decimal DineroUtilizado { get; set; }
        public decimal Total { get; set; }

        public List<DemoVentaDetalle> Detalles { get; set; } = new List<DemoVentaDetalle>();
    }

    public static class DemoStore
    {
        public static List<DemoCancion> Canciones = new List<DemoCancion>
        {
            new DemoCancion { CancionID="C-001", Nombre="Canción A", Precio=1000m, CantidadDisponible=5 },
            new DemoCancion { CancionID="C-002", Nombre="Canción B", Precio=1500m, CantidadDisponible=3 },
            new DemoCancion { CancionID="C-003", Nombre="Canción C", Precio=2000m, CantidadDisponible=10 },
        };

        public static string DemoUserId = "DEMO-USER-1";
        public static string DemoUserEmail = "yostiancortes123@gmail.com";
        public static string DemoUserName = "Cliente Demo";
        public static decimal DemoDineroDisponible = 150m;

        public static List<DemoVenta> Ventas = new List<DemoVenta>();
        private static int _seq = 0;

        public static string NextFactura()
        {
            _seq++;
            return $"FAC{_seq:00000000}";
        }

        public static DemoVenta CrearVentaDesdeCarrito(CarritoVM cart, string metodoPago, string ultimos4, decimal creditoAplicado)
        {
            var venta = new DemoVenta
            {
                VentaID = Ventas.Any() ? Ventas.Max(v => v.VentaID) + 1 : 1,
                NumeroFactura = NextFactura(),
                FechaCompra = DateTime.Now,
                MetodoPago = metodoPago,
                TarjetaEnmascarada = metodoPago == "Tarjeta de Crédito" && !string.IsNullOrWhiteSpace(ultimos4) ? $"****-****-****-{ultimos4}" : null,
                Subtotal = cart.Subtotal,
                IVA = Math.Round(cart.Subtotal * 0.13m, 2),
                ComisionTarjeta = metodoPago == "Tarjeta de Crédito" ? Math.Round(cart.Subtotal * 0.02m, 2) : 0m,
                DineroUtilizado = creditoAplicado,
            };
            venta.Total = venta.Subtotal + venta.IVA + venta.ComisionTarjeta - venta.DineroUtilizado;

            foreach (var it in cart.Lineas)
            {
                venta.Detalles.Add(new DemoVentaDetalle
                {
                    CancionID = it.CancionID,
                    Nombre = it.Nombre,
                    Cantidad = it.Cantidad,
                    PrecioUnitario = it.Precio,
                    Subtotal = it.SubtotalLinea
                });
            }

            Ventas.Add(venta);
            return venta;
        }
    }
}
