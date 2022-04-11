using Disney_API.Models.Schemes;
using System;
using System.Collections.Generic;

namespace Disney_API.Models
{
    public partial class Genero
    {
        public int Idgenero { get; set; }
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;

        public static implicit operator Genero(GenreCreate v)
        {
            return new Genero
            {
                Imagen = v.Image,
                Nombre = v.Name
            };
        }
    }
}
