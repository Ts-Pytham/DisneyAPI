using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Disney_API.Models
{
    public partial class Participacion
    {
        [Key]
        public int ID { get; set; }
        public int Idpelicula { get; set; }
        public int Idpersonaje { get; set; }

        public virtual Pelicula IdpeliculaNavigation { get; set; } = null!;
        public virtual Personaje IdpersonajeNavigation { get; set; } = null!;
    }
}
