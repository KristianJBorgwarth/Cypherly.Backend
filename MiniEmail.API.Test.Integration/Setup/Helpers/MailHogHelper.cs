using System.Text;
using System.Text.Json;

namespace MiniEmail.API.Test.Integration.Setup.Helpers;

public class MailHogHelper
{
    private const string MailHogApiUrl = "http://localhost:8025/api/v2/messages";
    private const string MailHogDeleteUrl = "http://localhost:8025/api/v1/messages";


    public async Task<EmailData?> GetMessagesFromMailHog()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(MailHogApiUrl);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var mailHogResponse = JsonSerializer.Deserialize<EmailData>(response, options);
        return mailHogResponse;
    }

    public async Task ClearMailHogMessage()
    {
        using var client = new HttpClient();
        await client.DeleteAsync(MailHogDeleteUrl);
    }

    public string DecodeBody(string contentBody)
    {
        var base64EncodedBytes = Convert.FromBase64String(contentBody);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}

public class EmailData
{
    public int Total { get; set; }
    public int Count { get; set; }
    public int Start { get; set; }
    public List<Item> Items { get; set; }
}

public class Item
{
    public string ID { get; set; }
    public EmailAddress From { get; set; }
    public List<EmailAddress> To { get; set; }
    public Content Content { get; set; }
    public DateTime Created { get; set; }
    public object MIME { get; set; }
    public Raw Raw { get; set; }
}

public class EmailAddress
{
    public object Relays { get; set; }
    public string Mailbox { get; set; }
    public string Domain { get; set; }
    public string Params { get; set; }
}

public class Content
{
    public Dictionary<string, List<string>> Headers { get; set; }
    public string Body { get; set; }
    public int Size { get; set; }
    public object MIME { get; set; }
}

public class Raw
{
    public string From { get; set; }
    public List<string> To { get; set; }
    public string Data { get; set; }
    public string Helo { get; set; }
}