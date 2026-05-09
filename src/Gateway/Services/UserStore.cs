namespace NorthwindTraders.Gateway.Services;

/// <summary>
/// Hard-coded test users — in production, replace with a real user database.
/// Passwords are BCrypt hashed at startup.
/// </summary>
public static class UserStore
{
    private record TestUser(string PasswordHash, string Role);

    private static readonly Dictionary<string, TestUser> Users = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = new(BCrypt.Net.BCrypt.HashPassword("Admin@123"), "Admin"),
        ["staff"] = new(BCrypt.Net.BCrypt.HashPassword("Staff@123"), "Staff")
    };

    public static (bool Valid, string Role) Validate(string username, string password)
    {
        if (!Users.TryGetValue(username, out var user))
            return (false, string.Empty);

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
            ? (true, user.Role)
            : (false, string.Empty);
    }
}
