using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Bugoom
{
    public class BuggingContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<BugChange> BugChanges { get; set; }

        public string DbPath { get; }

        public BuggingContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "bugging.db");

            Console.WriteLine(DbPath);
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public enum BugStatus
    {
        Open = 1,
        Assigned = 2,
        Fixed = 3,
        Closed = 4
    }

    public enum UserRole
    {
        User  = 1,
        Staff = 2,
        Boss = 3
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }        
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Bug
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
        public BugStatus Status { get; set; }
        public List<BugChange> Changes { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class BugChange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BugId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Description { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
