using Disney_API.Models.Schemes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Disney_API.Models
{
    public partial class Usuario
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = null!;

        public static implicit operator Usuario(User v)
        {
            return new Usuario
            {
                Password = v.Password,
                Email = v.Email,
            };
        }
    }
}
