using Disney_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Disney_API.Services
{
    public class UserService : IUserService
    {

        private readonly DisneyContext? _context;

        public UserService(DisneyContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUser(Usuario user) =>
           await _context?.Usuarios.Where(x => x.Email == user.Email && x.Password == user.Password).CountAsync()! > 0;

    }
}
