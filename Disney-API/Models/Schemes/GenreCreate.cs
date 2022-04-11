using Disney_API.Models.Interfaces;

namespace Disney_API.Models.Schemes
{
    public class GenreCreate : IGenre
    {
        public string Image { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
