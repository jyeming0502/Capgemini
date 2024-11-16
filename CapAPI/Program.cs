using CapApplication.Services;
using Domain;
using CapPersistence.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowCertainOrigins",
        policy =>
        {
            policy.WithOrigins("https://localhost:7049")
            .AllowCredentials()
            .WithMethods("PUT", "DELETE", "GET", "POST")
            .AllowAnyHeader();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowCertainOrigins");
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
