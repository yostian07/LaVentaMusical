using System.ComponentModel.DataAnnotations;

namespace LaVentaMusical.Models.ViewModels
{
    public class CheckoutVM
    {
        public CarritoVM Carrito { get; set; } = new CarritoVM();

        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }              // 13%
        public decimal ComisionTarjeta { get; set; }  // 2% si tarjeta
        public decimal DineroAplicado { get; set; }   // Nota de crédito usada
        public decimal Total { get; set; }
        public decimal DineroDisponibleUsuario { get; set; }

        [Required]
        public string MetodoPago { get; set; } // PayPal | Transferencia Bancaria | Tarjeta de Crédito


        [Display(Name = "Número de tarjeta")]
        [RegularExpression(@"^[\d\s\-]{12,23}$", ErrorMessage = "Número de tarjeta inválido")]
        public string NumeroTarjeta { get; set; }

        [Display(Name = "CVV")]
        [RegularExpression(@"\d{3,4}", ErrorMessage = "CVV inválido")]
        public string CVV { get; set; }

        // Calculado en el controlador a partir de NumeroTarjeta
        public string Ultimos4 { get; set; }
    }
}
