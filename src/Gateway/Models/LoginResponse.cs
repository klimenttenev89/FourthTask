namespace NorthwindTraders.Gateway.Models;

public record LoginResponse(string Token, DateTime ExpiresAt);
