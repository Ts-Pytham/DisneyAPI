using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Disney_API.Models
{
    public partial class GeneroPelicula
    {
        [Key]
        public int ID { get; set; }
        public int Idgenero { get; set; }
        public int Idpelicula { get; set; }

        public virtual Genero IdgeneroNavigation { get; set; } = null!;
        public virtual Pelicula IdpeliculaNavigation { get; set; } = null!;
    }
}
