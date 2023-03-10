using System.Security.Cryptography;

namespace InventoryManagementSystem.Services;

public class Utils
{
    private const char _segmentDelimiter = ':';

    public static string HashSecret(string input)
    {
        var saltSize = 16;
        var iterations = 100_000;
        var keySize = 32;
        var algorithm = HashAlgorithmName.SHA256;
        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

        var result = string.Join(
            _segmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            iterations,
            algorithm
        );

        return result;
    }

    public static bool VerifyHash(string input, string hashString)
    {
        var segments = hashString.Split(_segmentDelimiter);
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            iterations,
            algorithm,
            hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    public static string GetAppDirectoryPath()
    {
        return @"D:\LMU\Year 3\Modules\Application Development\Coursework\Inventory";
    }

    public static string GetAppUsersFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "users.json");
    }

    public static string GetAppProductsFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "products.json");
    }

    public static string GetAppOrdersFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "orders.json");
    }
}
