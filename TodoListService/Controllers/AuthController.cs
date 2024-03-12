using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TodoListService.Models;

namespace TodoListService.Controllers;

[Route("api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    
    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserCredentials model)
    {
        if (ModelState.IsValid)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == "hir@systemate.com");
            if (user != null && await _userManager.CheckPasswordAsync(user, "hiran1234"))
            {
                return Ok();
            }
        }
        return Unauthorized();
    }
}

