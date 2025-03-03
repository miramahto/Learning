using AuthApi.Data;
using AuthApi.DTO;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Result = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var userRole = await _userManager.FindByIdAsync(user.Id);
                var token = _jwtService.GenerateToken(user.Id, user.Email);
                return Ok(new { Token = token });
            }

            return Unauthorized();

          
        }
        [HttpGet("getUserDetails")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails([FromQuery] string email, [FromQuery] string AppTypeName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { Result = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                return NotFound(new { Result = "User Role not found" });
            }
            var role = roles.FirstOrDefault();
            if (role == null)
            {
                return NotFound(new { Result = "User Role not found" });
            }

            var UserApps = from app in _context.Applications
                         join appMapp in _context.UserApplicationsMappings on app.AppID equals appMapp.AppID
                         join usr in _context.Users on appMapp.UserID equals usr.UserName
                         join AType in _context.ApplicationsTypes on app.AppTypeID equals AType.AppTypeID
                         where AType.AppTypeName == AppTypeName && appMapp.UserID == email
                         select new
                         {
                             app.GUID,
                             app.AppName
                         };
            if (UserApps == null)
            {
                return NotFound(new { Result = "User application mapping not found" });
            }
            var userapp = UserApps.FirstOrDefault();
            if (userapp == null)
            {
                return NotFound(new { Result = "User application mapping not found" });
            }

            return Ok(new { Role = role, Userapp = userapp });
        }
        [HttpPost("registerApps")]
        [Authorize]
        public async Task<IActionResult> RegisterApplication([FromBody] RegisterApps model)
        {
            
            var user = new ApplicationsRegisterModel
            {
                AppName = model.AppName,
                GUID =Guid.NewGuid(),
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            };
             _context.Applications.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Result = "Application registered successfully" });
        }

        [HttpPost("userAppMap")]
        public async Task<IActionResult> UserApplicationsMappin([FromBody] UserApplicationsMapping model)
        {

            var userAppsmapp = new UserApplicationsMappingModel
            {
                AppID = model.AppID,
                UserID =model.UserID
               
            };
            _context.UserApplicationsMappings.Add(userAppsmapp);
            await _context.SaveChangesAsync();

            return Ok(new { Result = "Application registered successfully" });
        }
        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userEmail = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { Result = "User not found" });
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest(new { Result = "New password and confirmation password do not match" });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Result = "Password changed successfully" });
            }

            return BadRequest(result.Errors);
        }
    }
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
