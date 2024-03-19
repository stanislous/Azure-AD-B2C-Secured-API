using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using TodoListService.Constants;
using TodoListService.Models;
using TodoListService.Services;

namespace TodoListService.Controllers;

[Route("api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly MigrationService _service;
    private readonly AppSettingsModel _model;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, IOptions<AppSettingsModel> options)
    {
        _userManager = userManager;
        _configuration = configuration; 
        _service = new MigrationService(options);
        _model = options.Value;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(/*UserCredentials model*/)
    {
        string input;

        // If not data came in, then return
        if (Request.Body == null)
        {
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
        }

        // Read the input claims from the request body
        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            input = await reader.ReadToEndAsync();
        }

        // Check input content value
        if (string.IsNullOrEmpty(input))
        {
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
        }

        // Convert the input string into InputClaimsModel object
        InputClaimsModel inputClaims = InputClaimsModel.Parse(input);

        if (inputClaims == null)
        {
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
        }

        if (string.IsNullOrEmpty(inputClaims.signInName))
        {
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'signInName' is null or empty", HttpStatusCode.Conflict));
        }

        if (string.IsNullOrEmpty(inputClaims.password))
        {
            return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Password is null or empty", HttpStatusCode.Conflict));
        }

        // Create a retrieve operation that takes a customer entity.
        // Note: Azure Blob Table query is case sensitive, always set the input email to lower case
        //var retrieveOperation = TableOperation.Retrieve<UserTableEntity>(Consts.MigrationTablePartition, inputClaims.signInName.ToLower());

        //CloudTable table = await _service.GetSignUpTable(_model.BlobStorageConnectionString);

        // Execute the retrieve operation.
        //TableResult userMigrationEntity = await table.ExecuteAsync(retrieveOperation);
        if(ModelState.IsValid)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == inputClaims.signInName);
            if (user != null)
            {
                await _userManager.CheckPasswordAsync(user, inputClaims.password);

                try
                {
                    try
                    {
                        await _service.MigrateUser(inputClaims, user);
                        await Task.Delay(1500);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode((int)HttpStatusCode.Conflict,
                            new B2CResponseModel("Can not migrate user", HttpStatusCode.Conflict));
                    }

                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.Conflict,
                        new B2CResponseModel($"User migration error: {ex.Message}", HttpStatusCode.Conflict));
                }
            }
        }

        return Ok();

        //if (ModelState.IsValid)
        //{
        //    var user = _userManager.Users.FirstOrDefault(x => x.Email == model.SignInName);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //return Ok(model);
        //    }
        //}
        //return Unauthorized();
    }
}

