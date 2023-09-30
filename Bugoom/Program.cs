using System.Runtime.Intrinsics.X86;
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
builder.Services.AddScoped<IBugsService, BugsService>();

var app = builder.Build();

// Apply the migration to create and update the sqlite database,
// then create one Boss user, two Staff users, and two User users.
// If the database is already up to date and the users already exist,
// nothing will happen.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BuggingContext>();
    db.Database.Migrate();

    var seedUserRoles = new Dictionary<string, UserRole> {
        { "TheBoss", UserRole.Boss },
        { "FirstStaff", UserRole.Staff },
        { "SecondStaff", UserRole.Staff },
        { "FirstUser", UserRole.User },
        { "SecondUser", UserRole.User },
    };

    int seedUserId = 1;

    foreach (KeyValuePair<string, UserRole> kvp in seedUserRoles)
    {
        var user = db.Users.Where(u => u.Id == seedUserId).FirstOrDefault();
        if (user == null)
        {
            db.Users.Add(
                new User
                {
                    Id = seedUserId,
                    Username = kvp.Key,
                    Role = kvp.Value,
                    CreatedAt = DateTime.UtcNow
                }
            );

            db.SaveChanges();
        }        

        seedUserId++;
    }
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
