using System;
using System.Collections.Generic;

namespace Zoolirante_Open_Minded.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
