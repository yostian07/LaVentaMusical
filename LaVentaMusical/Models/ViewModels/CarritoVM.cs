using System.Collections.Generic;
using System.Linq;

namespace LaVentaMusical.Models.ViewModels
{
    public class CarritoLineaVM
    {
        public string CancionID { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal SubtotalLinea => Precio * Cantidad;
    }

    public class CarritoVM
    {
        public List<CarritoLineaVM> Lineas { get; set; } = new List<CarritoLineaVM>();
        public decimal Subtotal => Lineas.Sum(x => x.SubtotalLinea);
    }
}
