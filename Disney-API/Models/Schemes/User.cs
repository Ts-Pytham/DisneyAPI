using Disney_API.Models.Interfaces;

namespace Disney_API.Models.Schemes
{
    public class User : IUser
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
