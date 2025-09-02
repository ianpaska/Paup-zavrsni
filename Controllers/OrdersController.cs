using GameShop3.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace GameShop3.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        // Kupnja jedne igre (za demo)
        public ActionResult Buy(int gameId)
        {
            var game = Store.Games.FirstOrDefault(g => g.Id == gameId);
            if (game == null) return HttpNotFound();

            var userId = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            var order = new Order
            {
                Id = Store.NextOrderId(),
                UserId = userId,
                UserEmail = email,
                CreatedAt = DateTime.UtcNow
            };
            order.Items.Add(new OrderItem
            {
                GameId = game.Id,
                GameTitle = game.Title,
                Price = game.Price
            });

            Store.Orders.Add(order);

            TempData["Msg"] = $"Kupnja uspješna! Račun #{order.Id} spreman za preuzimanje.";
            return RedirectToAction("Invoice", new { id = order.Id });
        }

        // Pregled vlastitih računa
        public ActionResult MyInvoices()
        {
            var uid = User.Identity.GetUserId();
            var my = Store.Orders.Where(o => o.UserId == uid)
                                 .OrderByDescending(o => o.CreatedAt)
                                 .ToList();
            return View(my);
        }

        // Admin pregled svih
        [Authorize(Roles = "Admin")]
        public ActionResult AllInvoices()
        {
            var all = Store.Orders.OrderByDescending(o => o.CreatedAt).ToList();
            return View("MyInvoices", all); // recikliramo view
        }

        // Prikaz jedne potvrde + link za PDF
        public ActionResult Invoice(int id)
        {
            var uid = User.Identity.GetUserId();
            var order = Store.Orders.FirstOrDefault(o => o.Id == id && (o.UserId == uid || User.IsInRole("Admin")));
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // Preuzimanje PDF-a (bez paketa, minimalistički PDF)
        public FileResult DownloadPdf(int id)
        {
            var uid = User.Identity.GetUserId();
            var order = Store.Orders.FirstOrDefault(o => o.Id == id && (o.UserId == uid || User.IsInRole("Admin")));
            if (order == null) return null;

            var pdfBytes = SimplePdfGenerator.GenerateInvoicePdf(order);
            var fileName = $"Racun_{order.Id}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }

    // Minimalni PDF generator (jednostavni tekst)
    public static class SimplePdfGenerator
    {
        public static byte[] GenerateInvoicePdf(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine("GameShop3 - Račun");
            sb.AppendLine($"Broj: {order.Id}");
            sb.AppendLine($"Datum: {order.CreatedAt:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Korisnik: {order.UserEmail}");
            sb.AppendLine("--------------------------------");
            foreach (var it in order.Items)
                sb.AppendLine($"{it.GameTitle}  -  {it.Price:0.00} EUR");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Ukupno: {order.Total:0.00} EUR");
            sb.AppendLine();
            sb.AppendLine("Hvala na kupnji!");

            string text = sb.ToString().Replace("(", "\\(").Replace(")", "\\)");
            var pdf = new StringBuilder();
            pdf.AppendLine("%PDF-1.4");
            pdf.AppendLine("1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj");
            pdf.AppendLine("2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj");
            pdf.AppendLine("3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj");
            string stream = $"BT /F1 12 Tf 50 780 Td ({text.Replace("\r\n", "\\r").Replace("\n", "\\r")}) Tj ET";
            pdf.AppendLine($"4 0 obj << /Length {stream.Length} >> stream");
            pdf.AppendLine(stream);
            pdf.AppendLine("endstream endobj");
            pdf.AppendLine("5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj");
            pdf.AppendLine("xref");
            pdf.AppendLine("0 6");
            pdf.AppendLine("0000000000 65535 f ");
            string header = "%PDF-1.4\n";
            string[] objs = {
                "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n",
                "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n",
                "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj\n",
                $"4 0 obj << /Length {stream.Length} >> stream\n{stream}\nendstream endobj\n",
                "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj\n"
            };
            var bytes = Encoding.ASCII.GetBytes(header + string.Concat(objs));
            pdf.Clear();
            pdf.Append(Encoding.ASCII.GetString(bytes));
            pdf.Append("xref\n0 6\n");
            pdf.Append("0000000000 65535 f \n");
            int running = 0;
            foreach (var o in objs)
            {
                pdf.Append((running + header.Length).ToString("0000000000") + " 00000 n \n");
                running += Encoding.ASCII.GetByteCount(o);
            }
            int startxref = header.Length + Encoding.ASCII.GetByteCount(string.Concat(objs));
            pdf.Append($"trailer << /Size 6 /Root 1 0 R >>\nstartxref\n{startxref}\n%%EOF");

            return Encoding.ASCII.GetBytes(pdf.ToString());
        }
    }
}
