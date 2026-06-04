using System.ComponentModel.DataAnnotations;

public class Region
{
    [Key]
    public int RegionID { get; set; }
    public string RegionDescription { get; set; }

    public ICollection<Territory>? Territories { get; set; }
}
