﻿namespace Village_Manager.ViewModel
{
    public class RetailOrderItemViewModel
    {
        public int? OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
