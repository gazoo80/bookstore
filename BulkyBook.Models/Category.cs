using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order debe estar entre 1 y 100")]
        public int DisplayOrder { get; set; }

        public DateTime CratedDateTime { get; set; } = DateTime.Now;
    }
}
