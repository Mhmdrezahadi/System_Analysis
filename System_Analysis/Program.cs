using Socket;
using System_Analysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// add collection of services: dbcontext, jwtAuth, scopedServices, signalr, memorycache
builder.Services.AddServiceCollection(builder.Configuration);


builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.MapHub<AppHub>("/hub");

app.Run();
