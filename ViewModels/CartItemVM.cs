namespace Zoolirante_Open_Minded.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public string? ImageUrl { get; set; }
        public decimal LineTotal => Price * Qty;
    }
}

