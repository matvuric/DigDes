using Api.Models.Attachment;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class CreateUserModel
    {
        [Required] public string Username { get; set; } = null!;
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;
        [Required] public string Bio { get; set; } = null!;
        [Required] public string Gender { get; set; } = null!;
        [Required] public string Phone { get; set; } = null!;
        [Required] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
        [Required][Compare(nameof(Password))] public string RetryPassword { get; set; } = null!;
        [Required] public DateTimeOffset BirthDate { get; set; }
        [Required] public bool IsPrivate { get; set; } = false;
        public MetadataModel? Image { get; set; }
    }
}
