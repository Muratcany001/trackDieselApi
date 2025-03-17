using System.ComponentModel.DataAnnotations;
using trackDieselApi.Entities.AppEntities;

namespace BarMenu.Entities.AppEntities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public int Age { get; set; }
        public string Plate { get; set; }
        public string EngineType { get; set; }
        public List<Issue> ErrorHistory { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public string PartsReplaced { get; set; }
        
    }
}
