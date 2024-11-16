using CapApplication.Services;
using Capgemini;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CapPersistence.Services;
using Microsoft.Graph.Models;
using FluentEmail.Smtp;
using Domain;
using Blazored.Toast;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddScoped<IMailService, MailService>();
var baseUrl = builder.Configuration.GetSection("MicrosoftGraph")["BaseUrl"];
var scopes = builder.Configuration.GetSection("MicrosoftGraph:Scopes")
    .Get<List<string>>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAPIUrl")) });
builder.Services.AddHttpClient("CapAPI", httpc =>
{
    httpc.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAPIUrl"));
});

builder.Services
       .AddFluentEmail("jyemingchan05@gmail.com")
       .AddSmtpSender("localhost", 25);
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddBlazoredToast();


var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
await builder.Build().RunAsync();
