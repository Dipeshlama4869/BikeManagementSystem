using InventoryManagementSystem.Models;
using System.Text.Json;

namespace InventoryManagementSystem.Services;

public class ProductService
{
	public static void SaveAll(List<Product> products)
	{
		var appDataDirectoryPath = Utils.GetAppDirectoryPath();
		var appProductsFilePath = Utils.GetAppProductsFilePath();

		if (!Directory.Exists(appDataDirectoryPath))
		{
			Directory.CreateDirectory(appDataDirectoryPath);
		}

		var json = JsonSerializer.Serialize(products);

		File.WriteAllText(appProductsFilePath, json);
	}

	public static List<Product> GetAll()
	{
		string appDataDirectoryPath = Utils.GetAppProductsFilePath();

		if (!File.Exists(appDataDirectoryPath))
		{
			return new List<Product>();
		}

		var json = File.ReadAllText(appDataDirectoryPath);

		var result = JsonSerializer.Deserialize<List<Product>>(json);

		return result;
	}

	public static Product GetById(Guid id)
	{
		var products = GetAll();
		return products.FirstOrDefault(x => x.Id == id);
	}

	public static List<Product> Create(Guid userId, string title, int quantity)
	{
		if (quantity <= 0)
		{
			throw new Exception("Add a positive integer value for the item.");
		}

		var products = GetAll();
		
		var product = new Product()
		{
            Title = title,
            Quantity = quantity,
            CreatedBy = userId
        };

		products.Add(product);

		SaveAll(products);

		return products;
	}

	public static List<Product> Update(Guid productId, string title, int quantity)
	{
		var products = GetAll();

		var product = products.FirstOrDefault(x => x.Id == productId);

		if (product == null)
		{
			throw new Exception("Product not found.");
		}

		product.Title = title;
		product.Quantity = quantity;
		product.LastModifiedAt = DateTime.Now;

		SaveAll(products);

		return products;
	}

	public static List<Product> Delete(Guid id)
	{
		var products = GetAll();

		var product = products.FirstOrDefault(x => x.Id == id);

		if (product == null)
		{
			throw new Exception("Product not found.");
		}

		products.Remove(product);

		SaveAll(products);
		
		return products;
	}
}
