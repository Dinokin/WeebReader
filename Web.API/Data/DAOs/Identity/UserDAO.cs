using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeebReader.Web.API.Data.Contexts.Abstract;

namespace WeebReader.Web.API.Data.DAOs.Identity
{
    public class UserDAO
    {
        private readonly BaseContext _context;

        public UserDAO(BaseContext context) => _context = context;

        public async Task<ulong> CountUsers() => (ulong) await _context.Users.LongCountAsync();
        
        public Task<IEnumerable<IdentityUser<Guid>>> GetUsers(ushort skip, ushort take) => Task.FromResult(_context.Users.OrderBy(user => user.UserName).Skip(skip).Take(take).AsEnumerable());

        public Task<IEnumerable<(IdentityUser<Guid> User, IdentityRole<Guid>? Role)>> GetUsersWithRole(ushort skip, ushort take)
        {
            var result = _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new {user, userRole})
                .Join(_context.Roles, user => user.userRole.RoleId, role => role.Id, (user, role) => new ValueTuple<IdentityUser<Guid>, IdentityRole<Guid>?>(user.user, role))
                .Skip(skip).Take(take).AsEnumerable();

            return Task.FromResult(result);
        }

        public async Task<IdentityUser<Guid>?> GetUserById(Guid id) => await _context.Users.SingleOrDefaultAsync(entity => entity.Id == id);

        public async Task<(IdentityUser<Guid>? User, IdentityRole<Guid>? Role)> GetUserByIdWithRole(Guid id)
        {
            var result = await _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new {user, userRole})
                .Join(_context.Roles, user => user.userRole.RoleId, role => role.Id, (user, role) => new ValueTuple<IdentityUser<Guid>, IdentityRole<Guid>?>(user.user, role))
                .FirstOrDefaultAsync(tuple => tuple.Item1.Id == id);
            
            return result;
        }
    }
}