using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ACitas.Data
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public AuthDbContext(
            Microsoft.EntityFrameworkCore.DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }
    }
}