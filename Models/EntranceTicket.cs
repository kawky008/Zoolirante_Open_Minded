using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zoolirante_Open_Minded.Models
{
    public class EntranceTicket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Type { get; set; } 

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public DateTime IssuedDate { get; set; } = DateTime.Now;

        public DateTime ExpiredDate { get; set; } = DateTime.Now.AddMonths(1);

        
        public User User { get; set; }
    }
}
