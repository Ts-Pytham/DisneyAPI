using Disney_API.Models.Interfaces;

namespace Disney_API.Models.Schemes
{
    /* Esta clase sirve para crear al personaje y luego introducirlo a la base de datos */
    public class CharacterCreate : ICharacter
    {
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; } = null!;
    }
}
