using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class EditUserProfileModel
    {
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public bool IsPrivate { get; set; } = false;
    }   
}
