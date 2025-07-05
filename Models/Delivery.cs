using System;
using System.Collections.Generic;

namespace Village_Manager.Models;

public partial class Delivery
{
    public int Id { get; set; }

    public string? OrderType { get; set; }

    public int? OrderId { get; set; }

    public int? ShipperId { get; set; }

    public decimal? ShippingFee { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

   
}
