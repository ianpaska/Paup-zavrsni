using System.Collections.Generic;
using System.Linq;

namespace GameShop3.Models
{
    public static class Store
    {
        public static List<Game> Games { get; } = new List<Game>();
        public static List<Comment> Comments { get; } = new List<Comment>();
        public static List<Order> Orders { get; } = new List<Order>();

        private static int _gameId = 1;
        private static int _commentId = 1;
        private static int _orderId = 1;

        static Store()
        {
            Seed();
        }

        public static void Seed()
        {
            if (Games.Any()) return;

            Games.Add(new Game
            {
                Id = _gameId++,
                Title = "Elder Realms",
                Category = Category.RPG,
                Platforms = new List<Platform> { Platform.PC, Platform.PlayStation },
                Description = "Veliki otvoreni svijet, boss borbe.",
                Price = 59.99m,
                Popularity = 95
            });
            Games.Add(new Game
            {
                Id = _gameId++,
                Title = "Speed League 24",
                Category = Category.Sports,
                Platforms = new List<Platform> { Platform.PC, Platform.Xbox, Platform.PlayStation },
                Description = "Realistične utrke, online liga.",
                Price = 49.99m,
                Popularity = 80
            });
            Games.Add(new Game
            {
                Id = _gameId++,
                Title = "Star Voyage 2",
                Category = Category.Adventure,
                Platforms = new List<Platform> { Platform.PC, Platform.Nintendo },
                Description = "Svemirska avantura s pričom.",
                Price = 39.99m,
                Popularity = 70
            });
        }

        public static int NextCommentId() => _commentId++;
        public static int NextOrderId() => _orderId++;
    }
}
