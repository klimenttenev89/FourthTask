namespace NorthwindTraders.Frontend.Models;

public record OrderSummaryModel(
    int OrderId,
    DateTime? OrderDate,
    decimal TotalValue,
    int ProductCount
);

public record CustomerDetailModel(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country,
    IEnumerable<OrderSummaryModel> Orders
);
