using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task3.Data.Models;

namespace Task3.Data;

public class UneversityContext : IdentityDbContext<Employee>
{

    public UneversityContext(DbContextOptions<UneversityContext> options):base(options) { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>().ToTable("Doctors");
        //builder.Entity<IdentityUserClaim<string>>().ToTable("EmployesClaims");
    }
}

