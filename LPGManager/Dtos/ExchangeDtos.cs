﻿using System.Text.Json.Serialization;

namespace LPGManager.Dtos
{
    public class ExchangeMasterDtos:BaseDtos
    {
        public long InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public decimal TotalPricePaid { get; set; }
        public decimal TotalPriceReceive { get; set; }
        public string PaymentType { get; set; }
        public string? Notes { get; set; }
        public List<ExchangeDetailsDto> MyProductDetails { get; set; }
        public List<ExchangeDetailsDto> ReceiveProductDetails { get; set; }
    }
    public class ExchangeDetailsDto : BaseDtos
    {
        public long CompanyId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string ProductType { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Quantity { get; set; }
        public long ExchangeMasterId { get; set; }
        public int ExchangeType { get; set; }//1 for my product 2 for receive product
        [JsonIgnore]
        public CompanyDtos? Company { get; set; }

        [JsonIgnore]
        public ExchangeMasterDtos? ExchangeMaster { get; set; }
    }
}
