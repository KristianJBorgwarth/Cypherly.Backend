﻿using Cypherly.Application.Contracts.Messaging.PublishMessages.Email;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniEmail.API.Test.Integration.Setup;
using MinimalEmail.API.Email;
using MinimalEmail.API.Features.Consumers;

namespace MiniEmail.API.Test.Integration.ConsumerTest;

public class SendEmailConsumerTest : IntegrationTestBase
{
    private readonly SendEmailConsumer _sut;
    public SendEmailConsumerTest(IntegrationTestFactory<Program> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SendEmailConsumer>>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        _sut = new(emailService, logger);
    }

    [Fact]
    public async void Consume_Given_Valid_Message_Should_Send_Email()
    {
        // Arrange
        var msg = new SendEmailMessage("test@mail.dk", "Test subject", "Test body");

        var fakeConsumeContext = A.Fake<ConsumeContext<SendEmailMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(msg);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => fakeConsumeContext.ConsumeCompleted).MustHaveHappened();
        var email = await MailHogHelper.GetMessagesFromMailHog();
        email.Should().NotBeNull();
        var emailItem = email!.Items.FirstOrDefault();
        emailItem!.From.Domain.Should().Be("cypherly.org");
        emailItem.To.FirstOrDefault()!.Domain.Should().Be("mail.dk");
    }
}