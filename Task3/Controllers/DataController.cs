using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task3.Data.Models;

namespace Task3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController:ControllerBase
    {
        private readonly UserManager<Employee> _userManager;


        public DataController(UserManager<Employee> userManager) 
        {
            _userManager=userManager;
     
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetData()
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(new string[]{
                user.UserName,
                user.Email,
                user.DepartmentName
             
            });
      
        }

        [HttpGet]
        [Authorize(Policy ="Doctors")]
        [Route("ForDoctors")]
        public async Task<IActionResult> GetDataForDoctors()
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(new string[]{
                "Hello From Doctors"
            });

        }

        [HttpGet]
        [Authorize(Policy = "Students")]
        [Route("ForStudents")]
        public async Task<IActionResult> GetDataForStudents()
        {
            var user = await _userManager.GetUserAsync(User);

            return Ok(new string[]{
                "Hello from Student"
              
            });
        }
    }
}
