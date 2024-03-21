using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using IdentityAuthService.Model;
using IdentityAuthService.Services;
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
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email or Password cannot be empty.", HttpStatusCode.Conflict));
        
        if (ModelState.IsValid)
        {
            var userClaim = _authService.UserLoginValidation(inputClaims).Result;
            if(userClaim.status == 200)
                return Ok(userClaim);
            if(userClaim.status == 409)
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Invalid username or password.", HttpStatusCode.Conflict));
            if(userClaim.status == 502)
                return StatusCode((int)HttpStatusCode.BadGateway, new B2CResponseModel("Internal Error.", HttpStatusCode.Conflict));
        }

        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Validation error occured.", HttpStatusCode.Conflict));
    }
    
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpInputClaimModel inputClaims)
    {
        if (string.IsNullOrEmpty(inputClaims.SignInName))
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Email cannot be empty.", HttpStatusCode.Conflict));

        if (ModelState.IsValid)
        {
            var isUserExist = await _authService.UserSignUpValidation(inputClaims.SignInName);
            if (isUserExist)
            {
                return StatusCode((int)HttpStatusCode.Conflict, 
                    new B2CResponseModel("A user with the specified ID already exists. Please choose a different one.", HttpStatusCode.Conflict));
            }
            return Ok();
        }
        return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Validation error occured.", HttpStatusCode.Conflict));
    }
}