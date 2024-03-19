using Microsoft.WindowsAzure.Storage.Table;
using TodoListService.Constants;

namespace TodoListService.Models;

public class UserTableEntity : TableEntity
{
    public UserTableEntity()
    {

    }

    public UserTableEntity(string signInName, string password, string firstName, string lastName)
    {
        PartitionKey = Consts.MigrationTablePartition;
        RowKey = signInName.ToLower();
        Password = password;
        DisplayName = firstName + " " + lastName;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Password { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}