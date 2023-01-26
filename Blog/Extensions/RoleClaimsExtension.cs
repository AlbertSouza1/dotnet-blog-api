using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Blog.Models;

namespace Blog.Extensions
{
    public static class RoleClaimsExtension
    {
        public static IEnumerable<Claim> GetClaims(this User user)
        {
            List<Claim> result = new()
            {
                new(ClaimTypes.Name, user.Email)
            };

            result.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug)));

            return result;
        }
    }
}