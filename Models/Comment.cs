using System;

namespace GameShop3.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
