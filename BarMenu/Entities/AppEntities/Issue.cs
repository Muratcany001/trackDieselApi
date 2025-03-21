using BarMenu.Entities.AppEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

public class Issue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string PartName { get; set; }
    public string Description { get; set; }
    public DateTime DateReported { get; set; }
    public bool IsReplaced { get; set; }
    public int CarId { get; set; }
    [JsonIgnore]
    public Car? Car { get; set; }
}