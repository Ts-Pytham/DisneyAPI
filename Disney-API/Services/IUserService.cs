using Disney_API.Models;

namespace Disney_API.Services
{
    public interface IUserService
    {
        public Task<bool> IsUser(Usuario user);

    }
}
