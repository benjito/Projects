using Microsoft.EntityFrameworkCore;

namespace BackgroundTasks.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
