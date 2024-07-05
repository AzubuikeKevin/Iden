using Iden.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Iden.AppDBContext
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Organisation> Organization { get; set; }
        public DbSet<UserOrganisation> UserOrganisation { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserOrganisation>()
                 .HasKey(uo => new { uo.userId, uo.orgId });

            builder.Entity<UserOrganisation>()
                .HasOne(uo => uo.User)
                .WithMany(u => u.UserOrganisations)
                .HasForeignKey(uo => uo.userId);

            builder.Entity<UserOrganisation>()
                .HasOne(uo => uo.Organisation)
                .WithMany(o => o.UserOrganisations)
                .HasForeignKey(uo => uo.orgId);

            List<IdentityRole> roles =
            [
                new() {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new() {
                    Name = "User",
                    NormalizedName = "USER"
                }
            ];
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
