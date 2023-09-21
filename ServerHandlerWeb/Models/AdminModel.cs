using System.ComponentModel.DataAnnotations;

namespace ServerHandlerWeb.Models
{
    public class AdminModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string password { get; set; }
    }
}
