namespace Protecc.Models;

public class ExpenseData
{
    public decimal FoodTotal { get; set; }
    public decimal RentTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal SubscriptionTotal { get; set; }
    public decimal CarTotal { get; set; }
    public decimal InsuranceTotal { get; set; }
    public decimal ShoppingTotal { get; set; }
    public decimal Other { get; set; }
    public decimal TotalExpense { get; set; }
}