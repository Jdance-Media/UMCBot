using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMCBot.Models
{
    public class UMCBotDbContext : DbContext
    {
        public DbSet<Application> Applications { get; set; } = null!;

        private readonly string _dbPath;

        public UMCBotDbContext()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Join(path, "UMCBot.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_dbPath}");

        }
    }

    public class Application
    {
        [Key]
        public ulong UserId { get; set; }
        public ulong[]? Votes { get; set; }
    }
}
