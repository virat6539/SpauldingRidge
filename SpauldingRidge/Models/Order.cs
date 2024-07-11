using System;
using System.Collections.Generic;

namespace SpauldingRidge.Models;

public partial class Order
{
    public string? OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ShipDate { get; set; }

    public string? ShipMode { get; set; }

    public string? CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? Segment { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public double? PostalCode { get; set; }

    public string? Region { get; set; }
}
