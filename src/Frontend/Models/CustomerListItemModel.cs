namespace NorthwindTraders.Frontend.Models;

public record CustomerListItemModel(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? Country,
    int OrderCount
);
