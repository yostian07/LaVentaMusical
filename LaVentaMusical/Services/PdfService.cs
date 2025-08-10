using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LaVentaMusical.Models;

namespace LaVentaMusical.Services
{
    public static class PdfService
    {
        public static byte[] BuildInvoice(Venta venta)
        {
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var h1 = new Paragraph($"Factura {venta.NumeroFactura}", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                doc.Add(h1);
                doc.Add(new Paragraph($"Fecha: {venta.FechaCompra:yyyy-MM-dd HH:mm}"));
                doc.Add(new Paragraph($"Cliente: {venta.Usuario?.NombreCompleto ?? "Cliente"}"));
                doc.Add(new Paragraph($"Método: {(string.IsNullOrEmpty(venta.NumeroTarjetaEnmascarado) ? venta.TipoPago : venta.NumeroTarjetaEnmascarado)}"));
                doc.Add(new Paragraph(" "));

                var table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 10, 60, 10, 20 });
                table.AddCell("#"); table.AddCell("Canción"); table.AddCell("Cant."); table.AddCell("Total");

                int i = 1;
                foreach (var d in venta.Detalles)
                {
                    table.AddCell((i++).ToString());
                    table.AddCell($"{d.CancionID} - {d.Cancion?.NombreCancion ?? "Canción"}");
                    table.AddCell(d.Cantidad.ToString());
                    table.AddCell(d.Subtotal.ToString("C"));
                }
                doc.Add(table);

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph($"Subtotal: {venta.Subtotal:C}"));
                doc.Add(new Paragraph($"IVA 13%: {venta.IVA:C}"));
                if (venta.ComisionTarjeta > 0) doc.Add(new Paragraph($"Comisión Tarjeta 2%: {venta.ComisionTarjeta:C}"));
                if (venta.DineroUtilizado > 0) doc.Add(new Paragraph($"Nota de crédito aplicada: -{venta.DineroUtilizado:C}"));
                doc.Add(new Paragraph($"TOTAL: {venta.Total:C}", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));

                doc.Close();
                return ms.ToArray();
            }
        }
    }
}
