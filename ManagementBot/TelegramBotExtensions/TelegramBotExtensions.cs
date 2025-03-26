using ManagementBot.Commons;

namespace ManagementBot.TelegramBotExtensions
{
    public class TelegramBotExtensions
    {
        public static bool IsStartOrSettingCommand(string text) => text == CommandsKey.Start
           || text == CommandsKey.SendRequest;
    }
}
