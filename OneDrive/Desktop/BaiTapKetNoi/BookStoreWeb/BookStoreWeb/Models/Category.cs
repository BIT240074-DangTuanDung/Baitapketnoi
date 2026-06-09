using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreWeb.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Mối quan hệ: Một danh mục có thể chứa nhiều sách
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
