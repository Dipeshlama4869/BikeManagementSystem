using InventoryManagementSystem.Models;
using System.Text.Json;

namespace InventoryManagementSystem.Services;

public class StatsService
{
    public static List<Order> GetApprovedOrders()
    {
        string appDataDirectoryPath = Utils.GetAppOrdersFilePath();

        if (!File.Exists(appDataDirectoryPath))
        {
            return new List<Order>();
        }

        var json = File.ReadAllText(appDataDirectoryPath);

        var result = JsonSerializer.Deserialize<List<Order>>(json);

        var orders = result.Where(x => x.IsApproved).ToList();

        return orders;
    }

    public static List<OrderQuantity> GetOrderedQuantity()
    {
        var orders = GetApprovedOrders();

        var result = (from order in orders
                     group order by order.ProductId
                     into item
                     select new OrderQuantity
                     {
                         Item = item.Key,
                         Quantity = item.Sum(x => x.Quantity)
                     }).ToList();

        return result;
    }
}
