using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameShop3.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal Total => Items.Sum(i => i.Price);
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }          // PRIMARNI KLJUČ - obavezno!!!

        // Strani ključ na Order
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        // Podaci o igri / artiklu
        public int GameId { get; set; }
        public string GameTitle { get; set; }

        public decimal Price { get; set; }

        // Dodano polje Quantity, preporučeno
        public int Quantity { get; set; } = 1;
    }
}

