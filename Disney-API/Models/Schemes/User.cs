using Disney_API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Disney_API.Models.Schemes
{
    public class User : IUser
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Password { get; set; } = null!;
    }
}
