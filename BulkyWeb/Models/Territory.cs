using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Territory
{
    [Key]
    public string TerritoryID { get; set; } // nvarchar(20)
    public string TerritoryDescription { get; set; }
    public int RegionID { get; set; }

    [ForeignKey("RegionID")]
    public Region? Region { get; set; }
    //public ICollection<EmployeeTerritory>? EmployeeTerritories { get; set; }
}
