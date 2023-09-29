using System.Text.Json.Serialization;
using Bugoom;
using Bugoom.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts =>
                                                    {
                                                        var enumConverter = new JsonStringEnumConverter();
                                                        opts.JsonSerializerOptions.Converters.Add(enumConverter);
                                                    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BuggingContext>();
builder.Services.AddScoped<IUsersService, UsersService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BuggingContext>();
    db.Database.Migrate();

    var users = db.Users.ToList();
    foreach (var user in users)
    {
        db.Users.Remove(user);
    }
    db.SaveChanges();

    db.Users.Add(
        new User
        {
            Id = 1,
            Username = "TheBoss",
            Password = "LikeABoss",
            Role = UserRole.Boss,
            CreatedAt = DateTime.UtcNow
        }
    );

    db.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
