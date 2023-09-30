using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Configuration;

namespace TelegramBotExperiments
{
    class TgBot : IBot
    {
        TelegramBotClient botClient;
        public TgBot(string token) 
        {
            botClient = new TelegramBotClient(ConfigurationManager.AppSettings["TgBotToken"]);
        }
        public override void StartReceiving()
        {
            botClient.StartReceiving(Update, Error);
        }
        public override async Task Send(string id, string s) //, params string[] args
        {
            await botClient.SendTextMessageAsync(id, s);
        }
        async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token) 
        {
            Console.WriteLine("Error");
            Console.WriteLine(exception);
        }
        async Task Update(ITelegramBotClient client, Update update, CancellationToken token) 
        {
            RaiseSampleEvent(update.Message.Chat.Id.ToString(), update.Message.Text);
        }
    }
}