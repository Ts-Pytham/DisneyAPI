using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Disney_API.Models.Interfaces;
namespace Disney_API.Models
{
    /* Esta clase es donde se guarda el objeto a la base de datos*/
    public partial class Personaje : ICharacter, ICharacterWithInfo
    {


        public int Idpersonaje { get; set; }
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; } = null!;

        public static implicit operator Personaje(CharacterCreate v)
        {
            return new Personaje { Imagen = v.Imagen, Nombre = v.Nombre, 
                Edad = v.Edad, Peso = v.Peso, Historia = v.Historia };
        }
    }

    /* Esta clase sirve para crear al personaje y luego introducirlo a la base de datos */
    public class CharacterCreate : ICharacter, ICharacterWithInfo
    {
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; } = null!;

        
    }

    public class CharacterUpdate : CharacterCreate
    {
        
    }

    /* Esta clase sirve para solo mostrar la Imagen y Nombre */
    public class Character : ICharacter
    {
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
     
}
