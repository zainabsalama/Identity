using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task3.Data.Models;
using Task3.Dtos.DoctorDto;


namespace Task3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Employee> _userManager;

        public UserController(IConfiguration configuration, UserManager<Employee> userManager)
        {
            _configuration = configuration;
            _userManager=userManager;

        }

        #region Register AS Doctor
        [HttpPost]
        [Route("RegisterForDoctors")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var newDoctor = new Employee
            {
               UserName=registerDto.Name,
               Id=registerDto.Id,
               Email=registerDto.Email,
            };

            var creationResult = await _userManager.CreateAsync(newDoctor, registerDto.Password);
            if (!creationResult.Succeeded)
            {
                return BadRequest();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newDoctor.Id),
            new Claim(ClaimTypes.Role, "Doctor"),
            new Claim("Nationality", "Egyptian")
        };

            await _userManager.AddClaimsAsync(newDoctor, claims);

            return NoContent();
        }

        #endregion
        #region Register AS Student
        [HttpPost]
        [Route("RegisterForStudents")]
        public async Task<ActionResult> RegisterAsStudent(RegisterDto registerDto)
        {
            var newStudent = new Employee
            {
                UserName = registerDto.Name,
                Id = registerDto.Id,
                Email=registerDto.Email,
                DepartmentName = registerDto.DeptName,
            };

            var creationResult = await _userManager.CreateAsync(newStudent, registerDto.Password);
            if (!creationResult.Succeeded)
            {
                return BadRequest(creationResult.Errors);
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newStudent.Id),
            new Claim(ClaimTypes.Role, "Student"),
            new Claim("Nationality", "Egyptian")
        };

            await _userManager.AddClaimsAsync(newStudent, claims);

            return NoContent();
        }

        #endregion
        #region Login AS Doctor
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto credentials)
        {
            var user = await _userManager.FindByNameAsync(credentials.Name);
            if (user == null)
            {
                // you can send a message
                return BadRequest();
            }

            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (!isPasswordCorrect)
            {
                // you can send a message
                return BadRequest();
            }

            var claimsList = await _userManager.GetClaimsAsync(user);
            return GenerateToken(claimsList);

            
        }


        #endregion

        #region Login AS Student
        [HttpPost]
        [Route("LoginForStudent")]
        public async Task<ActionResult<TokenDto>> LoginAsStudent(LoginDto credentials)
        {
            var user = await _userManager.FindByNameAsync(credentials.Name);
            if (user == null)
            {
                // you can send a message
                return BadRequest();
            }

            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (!isPasswordCorrect)
            {
                // you can send a message
                return BadRequest();
            }

            var claimsList = await _userManager.GetClaimsAsync(user);
            return GenerateToken(claimsList);


        }


        #endregion
        #region Helpers

        private TokenDto GenerateToken(IList<Claim> claimsList)
        {
            string keyString = _configuration.GetValue<string>("SecretKey") ?? string.Empty;
            var keyInBytes = Encoding.ASCII.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyInBytes);

            //Combination of secret Key and HashingAlgorithm
            var signingCredentials = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha256Signature);

            //Putting All together
            var expiry = DateTime.Now.AddMinutes(15);

            var jwt = new JwtSecurityToken(
                    expires: expiry,
                    claims: claimsList,
                    signingCredentials: signingCredentials);

            //Getting Token String
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return new TokenDto
            {
                Token = tokenString,
                Expiry = expiry
            };
        }

        #endregion

    }
}
