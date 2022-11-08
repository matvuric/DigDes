using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class CreateUserModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password))]
        public string RetryPassword { get; set; } = null!;

        [Required]
        public DateTimeOffset BirthDate { get; set; }
    }
}
