using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// appel de ApplictionServiceExtension
builder.Services.AddApplicationServices(builder.Configuration);
// appel de IdentityServiceExtension
builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();
// middleware de gestions des erreur du serveur position avant toute"
app.UseMiddleware<ExeptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


// position importante ne pas deplacer accorder a la vue angular
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

// a placer entre cors et map 
// avez vous un jeton valide
app.UseAuthentication();
// ok vous avez un jeton valide ou avez vous le droit d'aller
app.UseAuthorization();

app.MapControllers();

app.Run();
