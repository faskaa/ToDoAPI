using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Text;
using ToDoApp.DAL;
using ToDoApp.DTOs;
using ToDoApp.Helpers;

namespace ToDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ToDoDBContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(ToDoDBContext context, IMapper mapper , ILogger<AccountController> logger, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager , IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registration([FromForm]RegistrationDTO account)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Registration failed due to invalid model state");
                return BadRequest();
            }

            IdentityUser user = new IdentityUser
            {
                UserName = account.Name,
                Email = account.Email,
            };

            _logger.LogInformation("Creating a new user account");
            IdentityResult result = await _userManager.CreateAsync(user, account.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("Registration failed due to errors");
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error: {error.Description}");

                }
                    return BadRequest(result.Errors.Select(error=>error.Description));
            }

            await _userManager.AddToRoleAsync(user, RoleEnum.Member.ToString());
            _logger.LogInformation("User added to the Member role");
                    
            return Ok(result);
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromForm]LoginDTO account)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Login failed: {account}");
                return BadRequest();
            }

            _logger.LogInformation($"User finding by name: {account.Username}");
            IdentityUser user = await _userManager.FindByNameAsync(account.Username);
            if (user is null)
            {
                _logger.LogError($"User not found: {account}");
                return NotFound();
            }

            _logger.LogInformation($"Checking user{account.Username}'s password");
            bool result = await _userManager.CheckPasswordAsync(user, account.Password);
            if (!result)
            {
                _logger.LogError("Password is not correct");
                return BadRequest();
            }

            List<Claim> claims = new List<Claim>
              {
               new Claim(ClaimTypes.NameIdentifier , user.Id),
               new Claim(ClaimTypes.Name , user.UserName),
               new Claim(ClaimTypes.Email , user.Email),
              };

            IList<string> roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
           
            _logger.LogInformation("Creating claims");

            string keyStr = _configuration["JWT:SecurityKey"]!;
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims:claims,
                signingCredentials:creds,
                expires: DateTime.Now.AddMinutes(1)
                );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(tokenStr);
        }

        [HttpPost("SignOut")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Exit()
        {
            await _signInManager.SignOutAsync(); 
            return Ok();
        }

        //[HttpPost("CreateRole")]
        //public async Task CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));
        //}
    }
}
