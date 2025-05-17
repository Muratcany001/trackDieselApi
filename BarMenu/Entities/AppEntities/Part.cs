using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarMenu.Entities.AppEntities
{
    public class Part
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int count { get; set; }
        public string State { get; set; }
        [JsonIgnore]
        public ICollection<CarPart> CarParts { get; set; } = new List<CarPart>();

        [NotMapped]
        public ICollection<Car> Cars => CarParts.Select(cp => cp.Car).ToList();
        public string UserId { get; set; }

    }
}
