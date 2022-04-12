using Disney_API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Disney_API.Models.Schemes
{
    public class User : IUser
    {
        [EmailAddress(ErrorMessage = "El correo no es válido, comprueba si lo escribiste bien.")]
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El correo debe de tener como máximo 50 caracteres.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Password { get; set; } = null!;
    }
}
