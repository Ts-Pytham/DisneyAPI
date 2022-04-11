using Microsoft.AspNetCore.Mvc;

namespace Disney_API.ModelBinder
{
    [BindProperties]
    public class GetRequestMovies
    {
        public string? Name { get; set; }
        public int? Genre { get; set; }

        public string Order { get; set; } = "ASC";
    }
}
