namespace BulkyWeb.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        = string.Empty;
        public string Description { get; set; }
        = string.Empty;
        public string Status { get; set; }
    = string.Empty;
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        // public User AssignedUser  { get; set; }
        public int AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }




    }
}
