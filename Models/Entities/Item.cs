﻿using System.ComponentModel.DataAnnotations;

namespace Project_API.Models.Entities
{
    public class Item
    {
        
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int PricePerItem { get; set; }
        public int Quantity { get; set; }

    }
}
