using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Authentication;
using WebApplication1.UserRegisterModels;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AuthenticationController(SignInManager<ApplicationUser> signInManager, IConfiguration config, UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var UserDataExist = await _userManager.FindByNameAsync(model.UserName);
            var UserEmailExist = await _userManager.FindByEmailAsync(model.Email);

            if ((UserEmailExist is not null) || (UserDataExist is not null))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Error = new Error { ErrorMessage = "already exist check your user name and emailId" }, IsSucess = false });
            }
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors=new StringBuilder();
                foreach (var item in result.Errors)
                {
                    errors.Append($"{item.Code}  - {item.Description}");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {

                    Error = new Error
                    {

                        ErrorMessage = errors.ToString(),
                    }
                }) ;
            }
            if (!await _roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(UserRole.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.User));
            }
            if (await _roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRole.User);
            }
            return StatusCode(StatusCodes.Status201Created, new Response { Error = null, IsSucess = true,Data="Register Successed" });
        }
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            var UserDataExist = await _userManager.FindByNameAsync(model.UserName);
            var UserEmailExist = await _userManager.FindByEmailAsync(model.Email);

            if ((UserEmailExist is not null) || (UserDataExist is not null))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Error = new Error { ErrorMessage = "already exist check your user name and emailId" }, IsSucess = false });
            }
            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    errors.Append($"{item.Code}  - {item.Description}");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {

                    Error = new Error
                    {

                        ErrorMessage = errors.ToString(),
                    }
                });
            }
            if(!await _roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(UserRole.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.User));
            }
            if(await _roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await _userManager.AddToRoleAsync(user,UserRole.Admin);
            }
            return StatusCode(StatusCodes.Status201Created, new Response { Error = null, IsSucess = true, Data = "Register Successed" });
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Login model)
        {
            var user=await _userManager.FindByNameAsync(model.UserName);
            var checkUserWithPassword=await _userManager.CheckPasswordAsync(user, model.Password);
            if (user != null && checkUserWithPassword)
            {
                var userRoles=await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var uRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, uRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecurityKey"]));
                var token = new JwtSecurityToken(

                    issuer: _config["JWT:ValidIssure"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                });
            }
            return Unauthorized();
        }
        

    }
}
