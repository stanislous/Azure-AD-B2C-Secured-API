using System.Net;
using System.Reflection;

namespace TodoListService.Models;

public class B2CResponseModel
{
    public string version { get; set; }
    public int status { get; set; }
    public string userMessage { get; set; }

    // Optional claims
    public string needToMigrate { get; set; }
    public string newPassword { get; set; }
    public string email { get; set; }
    public string displayName { get; set; }
    public string givenName { get; set; }
    public string surName { get; set; }
    public string streetAddress { get; set; }

    public B2CResponseModel(string message, HttpStatusCode status)
    {
        this.userMessage = message;
        this.status = (int)status;
        this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}