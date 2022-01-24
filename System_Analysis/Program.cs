using Socket;
using System_Analysis;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureWebHostDefaults(config => config.UseUrls(urls: "185.235.40.19:5001"));
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions
//{
   
//    ApplicationName = typeof(Program).Assembly.FullName,
//    ContentRootPath = Directory.GetCurrentDirectory(),
//    EnvironmentName = Environments.Staging,
//    WebRootPath = "customwwwroot"
//});



// add collection of services: dbcontext, jwtAuth, scopedServices, signalr, memorycache
builder.Services.AddServiceCollection(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
app.UseCors(config =>
{
    config.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapHub<AppHub>("/hub");

app.Run();
