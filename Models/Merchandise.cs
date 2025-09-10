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
}
