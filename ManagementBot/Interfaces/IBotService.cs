using Telegram.Bot.Types;

namespace ManagementBot.Interfaces
{
    public interface IBotService
    {
        ValueTask HandleUpdateAsync(Update update);
        Task StartPollingAsync(CancellationToken cancellationToken);
    }
}
