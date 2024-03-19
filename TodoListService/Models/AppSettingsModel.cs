namespace TodoListService.Models;

public class AppSettingsModel
{
    public string Tenant { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string BlobStorageConnectionString { get; set; }
}