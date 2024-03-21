using System.Net;
using TodoListService.Models;
using Microsoft.AspNetCore.Mvc;
using IdentityAuthService.Model;
using IdentityAuthService.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace IdentityAuthService.Controllers;

[Route("api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(InputClaimsModel inputClaims)
    {

        if (string.IsNullOrEmpty(inputClaims.SignInName) || string.IsNullOrEmpty(inputClaims.Password))
            return StatusCode((int)HttpStatusCode.Conflict, 
                new B2CResponseModel("UserName or Password cannot be empty.", HttpStatusCode.Conflict));
        
        if (ModelState.IsValid)
        {
            var userClaim = _authService.UserLoginValidation(inputClaims);
            return Ok(userClaim.Result);
        }

        return StatusCode((int)HttpStatusCode.Conflict,
            new B2CResponseModel("Validation error occured.", HttpStatusCode.Conflict));
    }
}