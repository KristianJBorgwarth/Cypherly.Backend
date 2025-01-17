using System.Reflection;
using Cypherly.ChatServer.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Application Layer

builder.Services.AddChatServerApplication(Assembly.Load("Cypherly.ChatServer.Application"));

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();