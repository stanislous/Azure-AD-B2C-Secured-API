using System.Net;
using IdentityAuthService.Model;
using Microsoft.AspNetCore.Identity;
using IdentityAuthService.Repositories.Interfaces;

namespace IdentityAuthService.Services;

public class AuthService(UserManager<IdentityUser> userManager, IUserRepository userRepository)
{
    public async Task<B2CResponseModel> UserLoginValidation(InputClaimsModel inputClaims)
    {
        var user = userManager.Users.FirstOrDefault(x => x.Email == inputClaims.SignInName);
        var outputClaims = new B2CResponseModel("", HttpStatusCode.OK);

        if (user != null)
        {
            var isPasswordValid = await userManager.CheckPasswordAsync(user, inputClaims.Password);
            var userDetails = userRepository.GetUserDetails(inputClaims.SignInName).Result;

            if (isPasswordValid)
            {
                try
                {
                    outputClaims.needToMigrate = "local";
                    outputClaims.newPassword = inputClaims.Password;
                    outputClaims.email = userDetails.UserEmailAddress;
                    outputClaims.displayName = userDetails.UserFirstName + " " +userDetails.UserLastName;
                    outputClaims.surName = userDetails.UserFirstName;
                    outputClaims.givenName = userDetails.UserFirstName;
                    outputClaims.phoneNumber = userDetails.UserPhoneNumber;
                    return outputClaims;
                }
                catch (Exception ex)
                {
                    return new B2CResponseModel("Interal Error", HttpStatusCode.Conflict);
                }
            }

            return new B2CResponseModel("Password is incorrect", HttpStatusCode.Conflict);
        }

        outputClaims.needToMigrate = null;
        return outputClaims;
    }
}