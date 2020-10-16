namespace RushHour.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using RushHour.Data.Entities;
    using RushHour.Domain;
    using RushHour.Domain.Services;
    using RushHour.ViewModels.Account;

    [Route("api/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = GlobalConstants.UserRoleName)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISecurityService _securityService;

        public UserController(UserManager<ApplicationUser> userManager, ISecurityService securityService)
        {
            _userManager = userManager;
            _securityService = securityService;
        }

        //// GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userView = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return Ok(userView);
        }

        //// POST: api/users/signup
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<UserViewModel>> SignUp(SignUpInput input)
        {
            var user = new ApplicationUser
            {
                UserName = input.Username,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName
            };

            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                var addToRole = await _userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                if (!addToRole.Succeeded)
                {
                    return BadRequest(addToRole.Errors.FirstOrDefault().Description);
                }

                var userView = new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = await _userManager.GetRolesAsync(user)
                };

                return CreatedAtAction("GetUser", new { id = userView.Id }, userView);
            }

            return BadRequest(result.Errors.FirstOrDefault().Description);
        }

        //// POST: api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> LogIn(LogInInput input)
        {
            var user = await _userManager.FindByNameAsync(input.Username);
            if (user == null)
            {
                return NotFound();
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, input.Password);
            if (!isValidPassword)
            {
                return BadRequest("Wrong Passowrd");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _securityService.GenerateJwtToken(user.UserName, roles);

            return Ok(token);
        }

        //// GET: api/users
        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetAllUsers(int page = 1, int size = 10)
        {
            if (page <= 0 || size <= 0)
            {
                return BadRequest();
            }

            var users = await _userManager.Users.Skip((page - 1) * size).Take(size).ToListAsync();

            var userViews = new List<UserViewModel>();
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var userView = new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                userView.Roles = userRoles;

                userViews.Add(userView);
            }

            return Ok(userViews);
        }

        //// PUT: api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<ActionResult> GiveAdminRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            return NoContent();
        }
    }
}