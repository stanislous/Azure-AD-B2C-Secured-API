using System.Net;
using TodoListService.Models;
using Microsoft.AspNetCore.Mvc;
using IdentityAuthService.Model;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace IdentityAuthService.Controllers;

[Route("api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppSettingsModel _model;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, IOptions<AppSettingsModel> options)
    {
        _userManager = userManager;
        _configuration = configuration;
        _model = options.Value;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(InputClaimsModel inputClaims)
    {

        if (string.IsNullOrEmpty(inputClaims.signInName) || string.IsNullOrEmpty(inputClaims.password))
            return StatusCode((int)HttpStatusCode.Conflict, 
                new B2CResponseModel("UserName or Password cannot be empty.", HttpStatusCode.Conflict));
        
        var outputClaims = new B2CResponseModel("", HttpStatusCode.OK);
        if (ModelState.IsValid)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == inputClaims.signInName);
           
            if (user != null)
            {
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, inputClaims.password);

                if (isPasswordValid)
                {
                    try
                    {
                        outputClaims.needToMigrate = "local";
                        outputClaims.newPassword = inputClaims.password;
                        outputClaims.email = inputClaims.signInName;
                        outputClaims.displayName = "Hiran";
                        outputClaims.surName = "Herath";
                        outputClaims.givenName = "Hiran Herath";
                        return Ok(outputClaims);
                    }
                    catch (Exception ex)
                    {
                        outputClaims.needToMigrate = null;
                        return Ok(outputClaims);
                    }
                }
                return StatusCode((int)HttpStatusCode.Conflict,
                    new B2CResponseModel("Password is incorrect", HttpStatusCode.Conflict));
            }
            outputClaims.needToMigrate = null;
            return Ok(outputClaims);
        }

        return Ok();
    }
}