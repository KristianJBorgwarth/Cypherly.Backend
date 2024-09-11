using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MiniEmail.API.Test.Integration.Setup;
using MinimalEmail.API.Features.Requests;

namespace MiniEmail.API.Test.Integration.EndpointTest;

public class SendEmailEndpointTest(IntegrationTestFactory<Program> factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async void Given_Valid_Request_Should_Send_Email_And_Return_200()
    {
        // Arrange
        var sendEmailRequest = new SendEmailRequest()
        {
            To = "test@mail.dk",
            Subject = "Test",
            Body = "Test"
        };

        // Act
        var response = await Client.PostAsJsonAsync("send-email", sendEmailRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var mailResult = await MailHogHelper.GetMessagesFromMailHog();
        mailResult.Should().NotBeNull();
        var item = mailResult!.Items.FirstOrDefault();
        item!.From.Domain.Should().Be("cypherly.org");
        item.Content.Body.Should().Contain(sendEmailRequest.Body);
        item.To.FirstOrDefault()!.Domain.Should().Be("mail.dk");
    }

    [Fact]
    public async void Given_Invalid_Request_Should_Return_400()
    {
        // Arrange
        var sendEmailRequest = new SendEmailRequest()
        {
            To = "",
            Subject = "Test",
            Body = "Test"
        };

        // Act
        var response = await Client.PostAsJsonAsync("send-email", sendEmailRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var mailResult = await MailHogHelper.GetMessagesFromMailHog();
        mailResult.Count.Should().Be(0);
    }
}