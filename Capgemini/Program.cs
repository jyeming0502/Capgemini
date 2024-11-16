using CapApplication.Services;
using Capgemini;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CapPersistence.Services;
using Microsoft.Graph.Models;
using FluentEmail.Smtp;
using Domain;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddScoped<IMailService, MailService>();
var baseUrl = builder.Configuration.GetSection("MicrosoftGraph")["BaseUrl"];
var scopes = builder.Configuration.GetSection("MicrosoftGraph:Scopes")
    .Get<List<string>>();
builder.Services.AddMicrosoftGraphClient(baseUrl, scopes);

builder.Services
       .AddFluentEmail("jyemingchan05@gmail.com")
       .AddSmtpSender("localhost", 25);
builder.Services.AddScoped<IMailService, MailService>();

var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
await builder.Build().RunAsync();
