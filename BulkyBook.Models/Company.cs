using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(100)]
        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(50)]
		[DisplayName("Postal Code")]
		public string? PostalCode { get; set; }

        [MaxLength(50)]
		[DisplayName("Phone Number")]
		public string? PhoneNumber { get; set; }
    }
}
