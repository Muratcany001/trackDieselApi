using System;
using System.ComponentModel.DataAnnotations;
namespace BarMenu.Entities.AppEntities

{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
