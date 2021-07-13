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

        public async Task<ulong> CountUsersWithRole(string roleName)
        {
            var result = await _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new {user, userRole})
                .Join(_context.Roles, tuple => tuple.userRole.RoleId, role => role.Id, (user, role) => new {user.user, role})
                .Where(arg => arg.role.Name == roleName).LongCountAsync();

            return (ulong) result;
        }
        
        public Task<IEnumerable<IdentityUser<Guid>>> GetUsers(ushort skip, ushort take) => Task.FromResult(_context.Users.OrderBy(user => user.UserName).Skip(skip).Take(take).AsEnumerable());

        public Task<IEnumerable<(IdentityUser<Guid> User, IdentityRole<Guid>? Role)>> GetUsersWithRole(ushort skip, ushort take)
        {
            var result = _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new {user, userRole})
                .Join(_context.Roles, tuple => tuple.userRole.RoleId, role => role.Id, (user, role) => new {user.user, role}).OrderBy(tuple => tuple.user.UserName)
                .Skip(skip).Take(take).Select(tuple => new ValueTuple<IdentityUser<Guid>, IdentityRole<Guid>?>(tuple.user, tuple.role)).AsEnumerable();

            return Task.FromResult(result);
        }

        public async Task<IdentityUser<Guid>?> GetUserById(Guid id) => await _context.Users.SingleOrDefaultAsync(entity => entity.Id == id);

        public async Task<(IdentityUser<Guid>? User, IdentityRole<Guid>? Role)> GetUserByIdWithRole(Guid id)
        {
            var result = await _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new {user, userRole})
                .Join(_context.Roles, tuple => tuple.userRole.RoleId, role => role.Id, (user, role) => new {user.user, role}).FirstOrDefaultAsync(tuple => tuple.user.Id == id);
            
            return (result.user, result.role);
        }
    }
}