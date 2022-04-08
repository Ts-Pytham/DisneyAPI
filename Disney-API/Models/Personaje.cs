using Disney_API.Models.Interfaces;
using Disney_API.Models.Schemes;
using System;
using System.Collections.Generic;

namespace Disney_API.Models
{
    public partial class Personaje : ICharacter
    {
        public int Idpersonaje { get; set; }
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; } = null!;

        public static implicit operator Personaje(CharacterCreate v)
        {
            return new Personaje
            {
                Imagen = v.Imagen,
                Nombre = v.Nombre,
                Edad = v.Edad,
                Peso = v.Peso,
                Historia = v.Historia
            };
        }
    }
}
