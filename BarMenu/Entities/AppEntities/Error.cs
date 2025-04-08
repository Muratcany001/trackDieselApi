using System.ComponentModel.DataAnnotations;

namespace BarMenu.Entities.AppEntities
{
    public class Error
    {
        [Key]
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
