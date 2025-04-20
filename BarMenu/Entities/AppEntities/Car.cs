using BarMenu.Entities.AppEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Car
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Plate { get; set; }
    
    public List<Issue>? ErrorHistory { get; set; } = new List<Issue>();
    public DateTime? LastMaintenanceDate { get; set; }
    public string UserId {  get; set; }
}