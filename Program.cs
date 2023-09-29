using System;
using System.Configuration;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotExperiments
{
    public delegate Task Func<in ITelegramBotClient, in Update, in CancellationToken, out Task>(ITelegramBotClient client, Update update, CancellationToken token);
    class Program
    {
        static Dictionary<string, Func<ITelegramBotClient, Update, CancellationToken, Task>> commands = new Dictionary<string, Func<ITelegramBotClient, Update, CancellationToken, Task>> {
            { "/start", Start },
            { "/help", Help },
        };

        private static Task Start(ITelegramBotClient client, Update update, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        static Dictionary<string, string> Discriptions = new Dictionary<string, string> {
            { "/start", "Начало работы с ботом" },
            { "/help", "Выводить все команды" },
        }; 

        async static Task Help(ITelegramBotClient client, Update update, CancellationToken token)
        {
            StringBuilder answer = new StringBuilder();
            foreach(KeyValuePair<string, string> pair in Discriptions)
                answer.Append(String.Format("{0} - {1}\n", pair.Key, pair.Value));
            await client.SendTextMessageAsync(update.Message.Chat.Id, answer.ToString());
        }

        static void Main()
        {
            Console.WriteLine("App start.");
            var botClient = new TelegramBotClient(ConfigurationManager.AppSettings["TgBotToken"]);
            Console.WriteLine("Bot client created.");
            botClient.StartReceiving(Update, Error);
            Console.WriteLine("Bot start reciving.");
            Console.ReadLine();
        }

        async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine("Bot Error.");
            //await client.SendTextMessageAsync(update.Message.Chat.Id, "Команда не найдена");
            //return;
            //throw new NotImplementedException();
        }

        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Console.WriteLine(String.Format("Bot try to make command {0}.", update.Message.Text));
            if (commands.TryGetValue(update.Message.Text, out Func<ITelegramBotClient, Update, CancellationToken, Task> deleg)) 
            {
                try
                {
                    await deleg(client, update, token); 
                    Console.WriteLine(String.Format("Bot successfully make command {0}.", update.Message.Text));
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Bot fails to make command {0}.", update.Message.Text));
                    Console.WriteLine(e);
                }
                return;
            } 
            else 
            {
                Console.WriteLine(String.Format("Bot try to find session and continue with parameter: {0}.", update.Message.Text));
                if (false) // Поиск сессии в базе если нашел, то продолжить сессию в ином случае выкинуть
                {
                    Console.WriteLine(String.Format("Bot found session and try to continue with parameter: {0}.", update.Message.Text));
                    return;
                }
                else 
                {
                    Console.WriteLine(String.Format("Bot fails to find session and continue with parameter: {0}.", update.Message.Text));
                }
            }
            Console.WriteLine("Bot command and session not found.");
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Команда не найдена");
            return; 
        }
    }
}