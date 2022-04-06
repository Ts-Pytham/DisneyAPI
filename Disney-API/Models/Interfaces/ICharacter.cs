namespace Disney_API.Models.Interfaces
{
    public interface ICharacter
    {
        public string Imagen { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; }

    }

}
