using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace TelegramBotExperiments 
{
    class BotOperator
    {
        public IBot Bot;
        public DataBaseContext Context;
        public BotOperator(IBot bot, DataBaseContext context) 
        {
            Bot = bot;
            Context = context;
            Bot.UpdateDelegate += Update;

            commands = new Dictionary<string, CommandDelegate> {
                { "/start", Start },
                { "/help", Help },
                { "/location", Location },
            };
        }
        static Dictionary<string, string> Discriptions = new Dictionary<string, string> {
            { "/start", "Запрос на авторизацию, если вы были авторизованы, то вы выйдете из аккаунта TeamFlame" },
            { "/help", "Выводит все команды" },
            { "/location", "Выводит ваше рабочее пространство в TeamFlame" },
        }; 
        static Dictionary<string, CommandDelegate> commands;

        private async Task Location(string id, string messageText)
        {
            string location = Context.Sessions.Find(id)?.Location;
            if (location == null)
                location = "Выберите пространство.";
            await Bot.Send(id, location);
        }

        private async Task Help(string id, string messageText)
        {
            StringBuilder answer = new StringBuilder();
            foreach(KeyValuePair<string, string> pair in Discriptions)
            {
                answer.Append(String.Format("{0} - {1}\n", pair.Key, pair.Value));
            }
            await Bot.Send(id, answer.ToString());
        }

        public async Task Start(string id, string messageText)
        {
            var l = messageText.Split(" ");
            if (l.Length >= 3)
            {
                var loginResponce = Helper.GetLogInResponse(l[1], l[2]);
                if (loginResponce == null) 
                {
                    await Bot.Send(id, "Неправильный логин или пароль.");
                    return;
                }
                await Bot.Send(id, "Вы авторизованы.");
                var user = Context.Users.Find(id);
                if (user == null)
                {
                    Context.Users.Add(new User{MediaUserId=id,token=loginResponce.tokens.accessToken.token});
                    return;
                }
            }        
            await Bot.Send(id, "Неправильный логин или пароль.");
        }

        public async Task Update(string id, string messageText) 
        {
            messageText = messageText.Trim();
            if (messageText == null) 
            {
                Console.WriteLine("Message == null");
                return;
            }

            Console.WriteLine(String.Format("Bot try to make command {0}.", messageText));
            int indexOfSpace = messageText.IndexOf(" ");
            string command;
            if (indexOfSpace != -1)
                command = messageText.Substring(0, indexOfSpace);
            else
                command = messageText;
            if (commands.TryGetValue(command, out CommandDelegate commandDelegate)) 
            {
                try
                {
                    await commandDelegate(id, messageText); 
                    Console.WriteLine(String.Format("Bot successfully make command {0}.", messageText));
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Bot fails to make command {0}.", messageText));
                    Console.WriteLine(e);
                }
                return;
            }
            Console.WriteLine("Bot command not found.");
            Bot.Send(id, "Команда и сессия не найдена");
            return;
        }
        public delegate Task CommandDelegate(string id, string messageText);
    }
}