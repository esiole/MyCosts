﻿namespace MyCosts.Api.Models.Cost;

public class CostModel
{
    public decimal Amount { get; init; }
    public int Count { get; set; }
    public double? Weight { get; set; }
    public required int ProductId { get; set; }
}