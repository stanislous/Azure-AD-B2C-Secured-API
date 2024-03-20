using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using TodoListService.Constants;
using TodoListService.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Graph;
using Newtonsoft.Json;
using Microsoft.Graph.Models;
using System;

namespace TodoListService.Services;

public class MigrationService
{
    private readonly AppSettingsModel _model;
    public MigrationService(IOptions<AppSettingsModel> options)
    {
        _model = options.Value;
    }
    public async Task MigrateUser(InputClaimsModel inputClaims, IdentityUser user)
    {
        var tokenEndpoint = $"https://login.microsoftonline.com/{_model.TenantId}/oauth2/v2.0/token";

        var accessToken = await GetAccessTokenAsync(tokenEndpoint, _model.ClientId, _model.ClientSecret);

        if (!string.IsNullOrEmpty(accessToken))
        {
            var userData = await AddNewUser(accessToken, user, inputClaims);

            if (!string.IsNullOrEmpty(userData))
            {
                Console.WriteLine("User data retrieved successfully:");
                Console.WriteLine(userData);
            }
            else
            {
                Console.WriteLine("Failed to retrieve user data.");
            }
        }
        else
        {
            Console.WriteLine("Failed to obtain access token.");
        }
    }

    static async Task<string> AddNewUser(string accessToken, IdentityUser user, InputClaimsModel inputClaims)
    {
        var requestBody = new User
        {
            DisplayName = user.UserName,
            Identities =
            [
                new()
                {
                    SignInType = "emailAddress",
                    Issuer = "systemateus.onmicrosoft.com",
                    IssuerAssignedId = user.Email,
                }
            ],
            UserType = "Guest",
            StreetAddress = "123 Main St",
            City = "Ohio",
            PostalCode = "12345",

            //OnPremisesExtensionAttributes = new OnPremisesExtensionAttributes
            //{
            //    ExtensionAttribute1 = "AspNetUserId.123-4520",
            //}

            PasswordProfile = new Microsoft.Graph.Models.PasswordProfile
            {
                Password = "1qaz@WSX",
                ForceChangePasswordNextSignIn = true,
            },
            PasswordPolicies = "DisablePasswordExpiration",
        };

        // Send the HTTP POST request to create the user
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var graphClient = new GraphServiceClient(httpClient);
        try
        {
            var response = await graphClient.Users.PostAsync(requestBody);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        //var response = await httpClient.PostAsync("https://graph.microsoft.com/v1.0/users", 
        //    new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json"));
        //if (response.Serialize())
        //{
        //    Console.WriteLine("User created successfully.");
        //    return await response.Content.ReadAsStringAsync();
        //}

        //Console.WriteLine($"Failed to create user. Status code: {response.StatusCode}");

        return "null";
    }

    static async Task<string> GetAccessTokenAsync(string tokenEndpoint, string clientId, string clientSecret)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tokenRequestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default")
            });

            var tokenResponse = await httpClient.PostAsync(tokenEndpoint, tokenRequestContent);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseData = await tokenResponse.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseData);
                return token.access_token;
            }

            return null;
        }
    }

    public async Task<CloudTable> GetSignUpTable(string conectionString)
    {
        // Retrieve the storage account from the connection string.
        var storageAccount = CloudStorageAccount.Parse(conectionString);

        // Create the table client.
        var tableClient = storageAccount.CreateCloudTableClient();

        // Create the CloudTable object that represents the "people" table.
        var table = tableClient.GetTableReference(Consts.MigrationTable);

        // Create the table if it doesn't exist.
        await table.CreateIfNotExistsAsync();

        return table;
    }
}