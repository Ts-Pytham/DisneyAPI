using System;
using System.Collections.Generic;

namespace Disney_API.Models
{
    public partial class Genero
    {
        public int Idgenero { get; set; }
        public string Imagen { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
