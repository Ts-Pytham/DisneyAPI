using Microsoft.AspNetCore.Mvc;

namespace Disney_API.ModelBinder
{
    [BindProperties]
    public class GetRequestCharacters
    {
        public string? Name { get; set; } = null!;
        public int? Age { get; set; } = null!;
        public int? Movies { get; set; } = null!;
    }

}
