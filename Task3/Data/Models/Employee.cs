using Microsoft.AspNetCore.Identity;

namespace Task3.Data.Models
{
    public class Employee:IdentityUser
    {
        public string? DepartmentName { get; set; }
    }
}
