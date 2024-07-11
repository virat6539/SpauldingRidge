using System;
using System.Collections.Generic;

namespace SpauldingRidge.Models;

public partial class Product
{
    public string? OrderId { get; set; }

    public string? ProductId { get; set; }

    public string? Category { get; set; }

    public string? SubCategory { get; set; }

    public string? ProductName { get; set; }

    public double? Sales { get; set; }

    public double? Quantity { get; set; }

    public double? Discount { get; set; }

    public double? Profit { get; set; }
}
