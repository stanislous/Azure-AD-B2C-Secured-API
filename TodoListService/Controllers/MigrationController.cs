using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using TodoListService.Models;
using TodoListService.Services;

namespace TodoListService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    MigrationService _service;
    private readonly AppSettingsModel _model;
    public MigrationController(IOptions<AppSettingsModel> options)
    {
        _service = new MigrationService(options);
    }

    [HttpGet(Name = "PopulateMigrationTable")]
    public async Task<ActionResult> PopulateMigrationTable()
    {

        CloudTable table = await _service.GetSignUpTable(_model.BlobStorageConnectionString);

        // Create the batch operation.
        TableBatchOperation batchOperation = new TableBatchOperation();

        // Create a customer entity and add it to the table.
        List<UserTableEntity> identities = new List<UserTableEntity>();
        identities.Add(new UserTableEntity("Jeff@contoso.com", "1234", "Jeff", "Smith"));
        identities.Add(new UserTableEntity("Ben@contoso.com", "1234", "Ben", "Smith"));
        identities.Add(new UserTableEntity("Linda@contoso.com", "1234", "Linda", "Brown"));
        identities.Add(new UserTableEntity("Sarah@contoso.com", "1234", "Sarah", "Miller"));
        identities.Add(new UserTableEntity("William@contoso.com", "1234", "William", "Johnson"));
        identities.Add(new UserTableEntity("John@contoso.com", "1234", "John", "Miller"));
        identities.Add(new UserTableEntity("Emily@contoso.com", "1234", "Emily", "Miller"));
        identities.Add(new UserTableEntity("David@contoso.com", "1234", "David", "Johnson"));
        identities.Add(new UserTableEntity("Amanda@contoso.com", "1234", "Amanda", "Davis"));
        identities.Add(new UserTableEntity("Brian@contoso.com", "1234", "Brian", "Wilson"));
        identities.Add(new UserTableEntity("Anna@contoso.com", "1234", "Anna", "Miller"));

        // Add both customer entities to the batch insert operation.
        foreach (var item in identities)
        {
            batchOperation.InsertOrReplace(item);
        }

        // Execute the batch operation.
        await table.ExecuteBatchAsync(batchOperation);

        return Ok(identities);
    }
}