using System;
using System.Collections.Generic;

namespace Zoolirante_Open_Minded.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int? Capacity { get; set; }

    public decimal Price { get; set; }

    public string Location { get; set; } = null!;

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
