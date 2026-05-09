namespace NorthwindTraders.CustomerService.DTOs;

public record OrderSummaryDto(
    int OrderId,
    DateTime? OrderDate,
    decimal TotalValue,
    int ProductCount
);
