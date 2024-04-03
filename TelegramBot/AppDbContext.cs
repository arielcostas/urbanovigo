using Microsoft.EntityFrameworkCore;
using Vigo360.VitrApi.Fetcher.Models;
using Vigo360.VitrApi.TelegramBot.Data.Models;

namespace Vigo360.VitrApi.TelegramBot;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public required DbSet<Announcement> Announcements { get; set; }
    public required DbSet<Subscriber> Subscribers { get; set; }
}