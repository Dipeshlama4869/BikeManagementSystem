using InventoryManagementSystem.Models;
using System.Text.Json;

namespace InventoryManagementSystem.Services;

public class OrderService
{
	private static void SaveAll(List<Order> orders)
	{
		var appDataDirectoryPath = Utils.GetAppDirectoryPath();

		var appOrdersFilePath = Utils.GetAppOrdersFilePath();

		if (!Directory.Exists(appDataDirectoryPath))
		{
			Directory.CreateDirectory(appDataDirectoryPath);
		}

		var json = JsonSerializer.Serialize(orders);

		File.WriteAllText(appOrdersFilePath, json);
	}

	public static List<Order> GetAll()
	{
		string appDataDirectoryPath = Utils.GetAppOrdersFilePath();

		if (!File.Exists(appDataDirectoryPath))
		{
			return new List<Order>();
		}

		var json = File.ReadAllText(appDataDirectoryPath);

		var result = JsonSerializer.Deserialize<List<Order>>(json);

		return result;
	}

	public static List<Order> Create(Guid userId, Guid productId, int quantity)
	{
		var day = (int)DateTime.Now.DayOfWeek + 1;

		var time = (int)DateTime.Now.Hour;

		if (day >= 2 && day <= 6)
		{
			if (time >= 9 && time <= 16)
			{
				var orders = GetAll();
				
				var products = ProductService.GetAll();

				var product = products.FirstOrDefault(x => x.Id == productId);

				if (quantity > product.Quantity)
				{
					throw new Exception("Can not order more items than the item's actual quantity.");
				}

				if (quantity < 0 || quantity == 0)
				{
					throw new Exception("Add a positive integer value for the item to be ordered.");
				}

				orders.Add(new Order
				{
					ProductId = productId,
					OrderedBy = userId,
					Quantity = quantity,
					OrderedDate = DateTime.Now,
				});

				SaveAll(orders);

				return orders;
			}
			else
			{
				throw new Exception("Can order only between 9AM and 4PM.");
			}
		}
		else
		{
			throw new Exception("Can order only between Monday and Friday.");
		}
		
	}

	public static List<Order> Update(Guid orderId, Guid userId)
	{

		var orders = GetAll();

		var products = ProductService.GetAll();

		var order = orders.FirstOrDefault(x => x.Id == orderId);

		var product = products.FirstOrDefault(x => x.Id == order.ProductId);

		if (order == null)
		{
			throw new Exception("Order not found.");
		}

		order.ApprovedBy = userId;
		order.IsApproved = true;
		order.ApprovedDate = DateTime.Now;
		product.Quantity -= order.Quantity;

		SaveAll(orders);
		ProductService.SaveAll(products);

		return orders;
	}
}
