using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Cors;
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
using ToDoApp.Entities;
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
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(ToDoDBContext context, IMapper mapper , ILogger<AccountController> logger, SignInManager<CustomUser> signInManager,
            UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager , IConfiguration configuration)
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
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Registration(RegistrationDTO account)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Registration failed due to invalid model state" , account);
                return BadRequest(new
                {
                    Message = "Invalid model state",
                    Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x=>x.ErrorMessage)
                });
            }

            CustomUser user = new CustomUser
            {
                UserName = account.Name,
                Email = account.Email,
            };

            _logger.LogInformation("Attempting to create a new user account" , user);
            IdentityResult result = await _userManager.CreateAsync(user, account.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed", new
                {
                    Error = result.Errors.Select(x=>x.Description)
                });
                
                return BadRequest(new
                {
                    Message = "User creation failed",
                    Error = result.Errors.Select(x=>x.Description)
                });
            }

            bool isRole = _context.Roles.Any(x=>x.Name == "Member");
            if (!isRole)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Member"));
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
            CustomUser user = await _userManager.FindByNameAsync(account.Username);
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
                expires: DateTime.Now.AddMinutes(5)
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

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUserInf()
        {
            CustomUser user =  await _userManager.GetUserAsync(User);
            string email  = await _userManager.GetEmailAsync(user);
            string name = User.Identity!.Name!;

            
            return Ok(new{Email = email, Name = name});
        }


        //[HttpPost("CreateRole")]
        //public async Task CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));
        //}


        //private async Task CheckRoleAsync(string roleName)
        //{
        //    if (!_context.Roles.Any(x=>x.Name == roleName))
        //    {
        //        _logger.LogInformation("Role does not exist. Creating role", new
        //        {
        //            RoleName = roleName
        //        });

        //        IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
        //        await _roleManager.CreateAsync(new IdentityRole("Admin"));


        //        if (!roleResult.Succeeded)
        //        {
        //            _logger.LogError("Failed to creatin role", new
        //            {
        //                RoleName = roleName,
        //                Error = roleResult.Errors
        //            });
        //            throw new InvalidOperationException($"Failed to creating role: {roleName} ");
        //        }
        //    }

            
        //}
    }

}
