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
                var token = _jwtService.GenerateToken(user.Id, user.Email);
                return Ok(new { Token = token });
            }

            return Unauthorized();

          
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
    }
}
