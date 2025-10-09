using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Zoolirante_Open_Minded.Models;

public partial class Merchandise
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? ImageUrl { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
public class MerchandiseIndexVM
{
    public string? SelectedCategory { get; set; }
    public List<SelectListItem> Categories { get; set; } = new();
    public List<Merchandise> Items { get; set; } = new();
}