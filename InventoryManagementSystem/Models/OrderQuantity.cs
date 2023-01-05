namespace InventoryManagementSystem.Models;

public class OrderQuantity
{
    public Guid Item { get; set; }    

    public int Quantity { get; set; }

    public DateTime LastOrderedDate { get; set; }
}
