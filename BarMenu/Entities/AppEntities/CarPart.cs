using BarMenu.Entities.AppEntities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CarPart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]  // JSON'dan çıkarılacak
    public int Id { get; set; }

    // Yalnızca CarId ve PartId'yi kullanacağız, ilişkiler veritabanında kurulur
    [JsonIgnore]
    public int CarId { get; set; }
    public int PartId { get; set; }

    [JsonIgnore] // Car nesnesini ignore et
    public Car? Car { get; set; }

    [JsonIgnore] // Part nesnesini ignore et
    public Part? Part { get; set; }

    public int UsedCount { get; set; }
}
