using System;
using System.Configuration;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExperiments
{
    public delegate Task Func<in ITelegramBotClient, in Update, in CancellationToken, out Task>(ITelegramBotClient client, Update update, CancellationToken token);
    class Program
    {
        static void Main()
        {
            Console.WriteLine("App start.");
            TgBot tgBot = new TgBot(ConfigurationManager.AppSettings["TgBotToken"]);
            var context = new DataBaseContext();
            BotOperator botOperator = new BotOperator(tgBot, context);
            Console.WriteLine("Bot client created.");
            botOperator.Bot.StartReceiving();
            Console.WriteLine("Bot start reciving.");
            Console.ReadLine();
            context.Dispose();
        }
    }
}

/*/*async static Task Help(ITelegramBotClient client, Update update, CancellationToken token)
        {
            StringBuilder answer = new StringBuilder();
            List<KeyboardButton> buttons = new List<KeyboardButton>();
            foreach(KeyValuePair<string, string> pair in Discriptions)
            {
                answer.Append(String.Format("{0} - {1}\n", pair.Key, pair.Value));
                KeyboardButton Button = new KeyboardButton(pair.Key);
                buttons.Add(Button);
            }

            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(SortButtons(buttons));

            await client.SendTextMessageAsync(update.Message.Chat.Id, answer.ToString(), replyMarkup: keyboard);

            //await client.SendTextMessageAsync(update.Message.Chat.Id, answer.ToString());
        }
        async static Task Location(ITelegramBotClient client, Update update, CancellationToken token)
        {
            //await client.SendTextMessageAsync(update.Message.Chat.Id, answer.ToString());
        }
        private static List<List<KeyboardButton>> SortButtons(List<KeyboardButton> buttons) 
        {
            List<List<KeyboardButton>> MatrixOfButtons = new List<List<KeyboardButton>>();
            int i = 0;
            if (buttons.Count % 2 != 0)
            {
                MatrixOfButtons.Add(new List<KeyboardButton>{buttons[0]});
                i = 1;
            }
            for ( ; i < buttons.Count; i++) 
            {
                List<KeyboardButton> row = new List<KeyboardButton>{buttons[i], buttons[i+1]};
                i = i + 1;
                MatrixOfButtons.Add(row);
            }
            return MatrixOfButtons;
        }*/