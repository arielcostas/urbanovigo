using System.ComponentModel.DataAnnotations;

namespace Vigo360.VitrApi.TelegramBot.Data.Models;

public class Subscriber
{
    [MaxLength(32)] [Key] public required string ChatId { get; set; }
    public DateTime SubscriptionDate { get; set; }
    public DateTime? BanDate { get; set; } = null;
}