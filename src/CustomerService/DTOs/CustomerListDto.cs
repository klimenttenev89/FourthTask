namespace NorthwindTraders.CustomerService.DTOs;

public record CustomerListDto(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? Country,
    int OrderCount
);
