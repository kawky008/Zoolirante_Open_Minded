using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;    
namespace Zoolirante_Open_Minded.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required]
    [Display(Name ="Full Name")]
    [StringLength(200,ErrorMessage  = "Full Name cannot exceed 200 characters.")]
    public string FullName { get; set; } = null!;

    [Required]
    [Display(Name ="Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name ="Password")]
    [DataType(DataType.Password)]
    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

	public string? PaymentMethod { get; set; }

	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
