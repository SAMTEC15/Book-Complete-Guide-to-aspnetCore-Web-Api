using Microsoft.AspNetCore.Identity;

namespace MyBook.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }                
        public string? Gender { get; set; }
       // public string CloudinaryPublicId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = new DateTime();
        public string? ImageUrl { get; set; }
      
    }
}
