using System.ComponentModel.DataAnnotations;

namespace BarMenu.Entities.AppEntities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public string EngineType { get; set; }
        public string ErrorHistory { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public string ErrorDescription { get; set; }
        public string PartsReplaced { get; set; }
        public int Age { get; set; }
    }
}
