using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LaVentaMusical.Infrastructure;

namespace LaVentaMusical.Services
{
    public static class PdfService
    {
        public static byte[] BuildInvoice(DemoVenta v)
        {
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var h1 = new Paragraph($"Factura {v.NumeroFactura}", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                doc.Add(h1);
                doc.Add(new Paragraph($"Fecha: {v.FechaCompra:yyyy-MM-dd HH:mm}"));
                doc.Add(new Paragraph($"Cliente: {DemoStore.DemoUserName}"));
                doc.Add(new Paragraph($"Método: {(string.IsNullOrEmpty(v.TarjetaEnmascarada) ? v.MetodoPago : v.TarjetaEnmascarada)}"));
                doc.Add(new Paragraph(" "));

                var table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 10, 60, 10, 20 });
                table.AddCell("#"); table.AddCell("Canción"); table.AddCell("Cant."); table.AddCell("Total");

                int i = 1;
                foreach (var d in v.Detalles)
                {
                    table.AddCell((i++).ToString());
                    table.AddCell($"{d.CancionID} - {d.Nombre}");
                    table.AddCell(d.Cantidad.ToString());
                    table.AddCell(d.Subtotal.ToString("C"));
                }
                doc.Add(table);

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph($"Subtotal: {v.Subtotal:C}"));
                doc.Add(new Paragraph($"IVA 13%: {v.IVA:C}"));
                if (v.ComisionTarjeta > 0) doc.Add(new Paragraph($"Comisión Tarjeta 2%: {v.ComisionTarjeta:C}"));
                if (v.DineroUtilizado > 0) doc.Add(new Paragraph($"Nota de crédito aplicada: -{v.DineroUtilizado:C}"));
                doc.Add(new Paragraph($"TOTAL: {v.Total:C}", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));

                doc.Close();
                return ms.ToArray();
            }
        }
    }
}
