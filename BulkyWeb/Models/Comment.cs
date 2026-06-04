//using System.Threading.Tasks;

namespace BulkyWeb.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public TaskItem TaskItem { get; set; }
        public int TaskItemId { get; set; }


        // public User User { get; set; }
        public int UserId { get; set; }



        public DateTime CreatedAt { get; set; }
    }
}
