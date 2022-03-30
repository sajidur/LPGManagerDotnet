﻿namespace LPGManager.Models
{
    public class CustomerEntity:BaseEntity
    {
        public string? Image { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? CustomerType { get; set; } //1 for retailer, 2 for customer
    }
}