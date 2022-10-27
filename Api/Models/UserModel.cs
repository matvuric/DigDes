using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class UserModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public DateTimeOffset BirthDate { get; set; }
    }
}
