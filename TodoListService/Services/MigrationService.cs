using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using TodoListService.Constants;
using TodoListService.Models;

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
        AzureADGraphClient azureADGraphClient = new AzureADGraphClient(_model.Tenant, _model.ClientId, _model.ClientSecret);

        // Create the user using Graph API
        await azureADGraphClient.CreateAccount(
            "emailAddress",
            inputClaims.signInName,
            "systemateus",
            null,
            null,
            inputClaims.password,
            user.UserName,
            user.UserName,
            user.UserName);

        // Remove the user entity from migration table
        //TableOperation deleteOperation = TableOperation.Delete((UserTableEntity)userMigrationEntity.Result);
        //await table.ExecuteAsync(deleteOperation);
    }

    public async Task<CloudTable> GetSignUpTable(string conectionString)
    {
        // Retrieve the storage account from the connection string.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conectionString);

        // Create the table client.
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

        // Create the CloudTable object that represents the "people" table.
        CloudTable table = tableClient.GetTableReference(Consts.MigrationTable);

        // Create the table if it doesn't exist.
        await table.CreateIfNotExistsAsync();

        return table;
    }
}