using System.Collections.Generic;
using System.Linq;

namespace GameShop3.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public List<Platform> Platforms { get; set; } = new List<Platform>();
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Popularity { get; set; } // jednostavan broj za sortiranje

        public double AverageRating(IEnumerable<Comment> comments) =>
            comments?.Where(c => c.GameId == Id).Select(c => c.Rating).DefaultIfEmpty(0).Average() ?? 0;
    }
}
