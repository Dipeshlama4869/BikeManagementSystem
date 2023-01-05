using System.Text.Json;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services;

public class UserService
{
    public const string SeedUsername = "admin";
    public const string SeedEmail = "admin@admin.com";
    public const string SeedPassword = "admin";

    public static void SeedUsers()
    {
        var users = GetAll().FirstOrDefault(x => x.Role == Role.Admin);

        if (users == null)
        {
            Create(Guid.Empty, SeedUsername, SeedEmail, SeedPassword, Role.Admin);
        }
    }

    public static List<User> GetAll()
    {
        var appUsersFilePath = Utils.GetAppUsersFilePath();

        if (!File.Exists(appUsersFilePath))
        {
            return new List<User>();
        }

        var json = File.ReadAllText(appUsersFilePath);

        return JsonSerializer.Deserialize<List<User>>(json);
    }

	public static User GetById(Guid id)
	{
		var users = GetAll();
		return users.FirstOrDefault(x => x.Id == id);
	}

	private static void SaveAll(List<User> users)
    {
        var appDataDirectoryPath = Utils.GetAppDirectoryPath();
        var appUsersFilePath = Utils.GetAppUsersFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(users);

        File.WriteAllText(appUsersFilePath, json);
    }

	public static User Login(string username, string password)
	{
		var loginErrorMessage = "Invalid username or password.";

		var users = GetAll();

		var user = users.FirstOrDefault(x => x.Username == username);

		if (user == null)
		{
			throw new Exception(loginErrorMessage);
		}

		bool passwordIsValid = Utils.VerifyHash(password, user.PasswordHash);

		if (!passwordIsValid)
		{
			throw new Exception(loginErrorMessage);
		}

		return user;
	}

	public static List<User> Create(Guid userId, string username, string email, string password, Role role)
    {
        var users = GetAll();

        var usernameExists = users.Any(x => x.Username == username);
        
        var numberOfAdmins = users.Where(x => x.Role == Role.Admin).Count();

        if(numberOfAdmins > 2 && role == Role.Admin) 
        {
            throw new Exception("The system already has two admins. Further addition of admin is not allowed.");
		}

		if (usernameExists)
        {
            throw new Exception("Username already exists. Please choose any other username.");
        }

        var user = new User()
        {
            Username = username,
            Email = email,
            PasswordHash = Utils.HashSecret(password),
            Role = role,
            CreatedBy = userId,
        };

        users.Add(user);

        SaveAll(users);
        
        return users;
    }

    public static List<User> Delete(Guid id)
    {
        var users = GetAll();
        
        var user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        users.Remove(user);
        
        SaveAll(users);

        return users;
    }

    
}
