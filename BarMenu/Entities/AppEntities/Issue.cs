using BarMenu.Entities.AppEntities;
using System.ComponentModel.DataAnnotations;

namespace trackDieselApi.Entities.AppEntities
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }
        public string PartName { get; set; }
        public string Description { get; set; }
        public DateTime DateReported { get; set; }
        public string IssueType { get; set; }
        public bool isReplaced { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}