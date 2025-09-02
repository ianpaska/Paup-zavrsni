using GameShop3.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace GameShop3.Controllers
{
    public class GamesController : Controller
    {
        public ActionResult Index(string search, Category? category, Platform? platform, string sort)
        {
            var games = Store.Games.AsQueryable();

          
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedPlatform = platform;
            ViewBag.Sort = sort;
            ViewBag.Categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToList(); // List<Category>
            ViewBag.Platforms = Enum.GetValues(typeof(Platform)).Cast<Platform>().ToList(); // List<Platform>

            if (!string.IsNullOrWhiteSpace(search))
                games = games.Where(g => g.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);

            if (category.HasValue)
                games = games.Where(g => g.Category == category.Value);

            if (platform.HasValue)
                games = games.Where(g => g.Platforms.Contains(platform.Value));

            // sortiranje
            switch ((sort ?? "").ToLowerInvariant())
            {
                case "price_asc": games = games.OrderBy(g => g.Price); break;
                case "price_desc": games = games.OrderByDescending(g => g.Price); break;
                case "popularity": games = games.OrderByDescending(g => g.Popularity); break;
                case "rating":
                    games = games.OrderByDescending(g => g.AverageRating(Store.Comments));
                    break;
                default:
                    games = games.OrderBy(g => g.Title);
                    break;
            }

            return View(games.ToList());
        }

        public ActionResult Details(int id)
        {
            var game = Store.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return HttpNotFound();

            var comments = Store.Comments.Where(c => c.GameId == id)
                                         .OrderByDescending(c => c.CreatedAt)
                                         .ToList();

            ViewBag.Avg = game.AverageRating(Store.Comments);
            ViewBag.CanComment = User.Identity.IsAuthenticated && UserHasPurchased(User.Identity.GetUserId(), id);

            return View(Tuple.Create(game, comments));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int gameId, string text, int rating)
        {
            var userId = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            if (!UserHasPurchased(userId, gameId))
            {
                TempData["Msg"] = "Komentirati i ocjenjivati možeš samo igre koje si kupio/la.";
                return RedirectToAction("Details", new { id = gameId });
            }

            if (rating < 1 || rating > 5) rating = 5;

            Store.Comments.Add(new Comment
            {
                Id = Store.NextCommentId(),
                GameId = gameId,
                UserId = userId,
                UserEmail = email,
                Text = text ?? "",
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            });

            TempData["Msg"] = "Komentar spremljen!";
            return RedirectToAction("Details", new { id = gameId });
        }

        private bool UserHasPurchased(string userId, int gameId) =>
            Store.Orders.Any(o => o.UserId == userId && o.Items.Any(i => i.GameId == gameId));
    }
}
