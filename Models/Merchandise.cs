using System;
using System.Collections.Generic;



namespace Zoolirante_Open_Minded.Models;

public partial class Merchandise
{
    public decimal? SpecialPrice { get; set; }
    
    public int SpecialQty { get; set; } = 0;
    
    public string? SpecialReason { get; set; }   

    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? ImageUrl { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
