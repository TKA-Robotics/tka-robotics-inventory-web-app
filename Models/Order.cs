using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DotNetCoreSqlDb.Models
{
    public class Order
    {
        
        [Key]
        public int ID { get; set; }

        public DateTime Timestamp { get; set; }

        [Display(Name = "Ordering Student")]
        public string OrderingStudent { get; set; }

        [Display(Name = "Team")]
        public Team RoboticsTeam { get; set; }

        public string Vendor { get; set; }

        [Display(Name = "Part Name")]
        [Required]
        public string PartName { get; set; }

        public string SKU { get; set; }

        public string Link { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Justification { get; set; }

        public OrderStatus Status { get; set; }

        public enum Team
        {
            [Display(Name = "Madness")]
            Madness,
            [Display(Name = "Mayhem")]
            Mayhem
        }

        public enum OrderStatus
        {
            [Display(Name = "Requested")]
            Requested,
            [Display(Name = "Ordered")]
            Ordered,
            [Display(Name = "Received")]
            Received,
            [Display(Name = "Added")]
            Added
        }


        public decimal TotalPrice {
            get
            {
                return Quantity * Price;
            }
        }
    }

    
}

