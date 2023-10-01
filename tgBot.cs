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
        public async Task Update(ITelegramBotClient client, Update update, CancellationToken token) 
        {
            if (update == null)
                return;
            if (update.Message == null)
                return;
            if (update.Message.Text == null)
                return;
            RaiseSampleEvent(update.Message.Chat.Id.ToString(), update.Message.Text);
        }
        public override async Task SendButtonsIfPossible(string id, string message, string[] buttons)
        {
            List<KeyboardButton> tgButtons = new List<KeyboardButton>();
            foreach(var elem in buttons) 
            {
                if (elem != null)
                    tgButtons.Add(new KeyboardButton(elem));
            }
            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(SortButtons(tgButtons));
            await botClient.SendTextMessageAsync(id, message, replyMarkup: keyboard);
        }
        private static List<List<KeyboardButton>> SortButtons(List<KeyboardButton> buttons) 
        {
            List<List<KeyboardButton>> MatrixOfButtons = new List<List<KeyboardButton>>();
            List<KeyboardButton> row;
            for (int i = 0; i < buttons.Count; i = i + 2)
            {
                if (i + 1 < buttons.Count) 
                    row = new List<KeyboardButton>{buttons[i], buttons[i+1]};
                else
                    row = new List<KeyboardButton>{buttons[i]};
                MatrixOfButtons.Add(row);
            }
            return MatrixOfButtons;
        }
    }
}