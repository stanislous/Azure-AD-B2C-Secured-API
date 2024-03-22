using System.Net;
using IdentityAuthService.Model;
using Microsoft.AspNetCore.Identity;
using IdentityAuthService.Repositories.Interfaces;

namespace IdentityAuthService.Services;

public class AuthService(UserManager<IdentityUser> userManager, IUserRepository userRepository)
{
    public async Task<B2CResponseModel> UserLoginValidation(InputClaimsModel inputClaim)
    {
        var userDetails = userRepository.GetUserDetails(inputClaim.SignInName).Result;
        var user = userManager.Users.FirstOrDefault(x => x.Email == inputClaim.SignInName);
        var outputClaims = new B2CResponseModel("", HttpStatusCode.OK) { status = 200 };

        // User is available and is not migrated yet.
        if (user != null & userDetails != null)
        {
            var isPasswordValid = await userManager.CheckPasswordAsync(user, inputClaim.Password);
            await userRepository.UpdateUserByIsMigrated(inputClaim.SignInName);

            if (isPasswordValid)
            {
                try
                {
                    outputClaims.needToMigrate = "local";
                    outputClaims.newPassword = inputClaim.Password;
                    outputClaims.email = userDetails.UserEmailAddress;
                    outputClaims.displayName = userDetails.UserFirstName + " " + userDetails.UserLastName;
                    outputClaims.surName = userDetails.UserFirstName;
                    outputClaims.givenName = userDetails.UserFirstName;
                    //outputClaims.mobilePhone = userDetails.UserPhoneNumber;
                    outputClaims.userType = "Guest";
                    return outputClaims;
                }
                catch (Exception ex)
                {
                    return new B2CResponseModel("Internal Error", HttpStatusCode.BadGateway){status = 502};
                }
            }

            return new B2CResponseModel("Invalid username or password.", HttpStatusCode.Conflict){status = 409};
        }
        // User is available and is already migrated.
        if (user != null & userDetails == null)
        {
            outputClaims.needToMigrate = null;
            return outputClaims;
        }

        // User is not available. Should be a new user.
        return new B2CResponseModel("Invalid username or password.", HttpStatusCode.Conflict) { status = 409 };
    }

    public async Task<bool> UserSignUpValidation(string email)
    {
        var user = userManager.Users.FirstOrDefault(x => x.Email == email);
        if (user != null)
        {
            return true;
        }

        return false;
    }
    
}