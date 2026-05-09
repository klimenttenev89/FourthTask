namespace NorthwindTraders.CustomerService.DTOs;

public record CustomerDetailDto(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country,
    IEnumerable<OrderSummaryDto> Orders
);
