using System.Net;
using System.Reflection;

namespace TodoListService.Models;

public class B2CResponseModel(string message, HttpStatusCode status)
{
    public string version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    public int status { get; set; } = (int)status;
    public string userMessage { get; set; } = message;
}