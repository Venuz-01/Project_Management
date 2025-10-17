using Microsoft.AspNetCore.Mvc;
using ModelForPMS.ModelDtos;
using RepositoriesForPMS.Interfaces;

namespace Project_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            var user = _userRepository.ValidateUser(loginDto.Email, loginDto.Password, loginDto.UserRole);

            if (user == null)
                return Unauthorized("Invalid credentials or role.");

            return Ok(new
            {
                Message = $"Welcome {user.UserName}, you are logged in as {user.UserRole}.",
                Role = user.UserRole,
                Name = user.UserName
            });
        }
    }

}
