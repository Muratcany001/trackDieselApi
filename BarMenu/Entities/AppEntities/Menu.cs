using System.ComponentModel.DataAnnotations;

namespace BarMenu.Entities.AppEntities
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingeredents { get; set; }
        public string Category { get; set; }
        public string Owner { get; set; }
    }
}
