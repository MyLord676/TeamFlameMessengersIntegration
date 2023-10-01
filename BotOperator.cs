using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

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

            Commands = new Dictionary<string, CommandDelegate> {
                /*{ "/start", Start },
                { "/help", Help },
                { "/location", Location },*/
                { "Авторизоваться", Authorize },
                { "Меню", DefaultMenu },
                { "Вперед", forward },
                { "Выбор пространства", ChooseSpace },
                { "Создание задачи", CreateTask },
                { "Добавить комментарий", AddComment },
                { "Назад", Back },
                { "Выбор проекта", ChooseProject },
                { "Выбор таблицы", ChooseDesk },
                { "Выбор колонки", ChooseColumn },
                { "Выбор задачи", ChooseTask }
            };
        }

        private async Task ChooseTask(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
            {
                session = new Session(){MediaUserId=id};
                Context.Sessions.Add(session);
            }
            session.ProcedureName = null;
            session.CurrentState = null;
            Context.SaveChanges();
            var user = Context.Users.Find(id);
            if (user == null || user.token == null)
                return;
            if (session.Location == null)
            {
                Console.WriteLine("location == null" + 52);
                return;
            }
            var column = Helper.GetColumnById(session.Location, user.token);
            var tasks = column.GetDirectChilds(user.token);
            if (tasks.Count == 0)
            {
                await Bot.Send(id, "Нет доступных задач.");
                return;
            }
            TFTask? task = null;
            foreach(var elem in tasks)
            {
                if (elem.Name == messageText)
                    task = elem;
            }
            if (task == null)
                return;
            
            session.LocationType = TFItemType.Task;
            session.Location = task.Id;
            Context.SaveChanges();
            await Bot.Send(id, "Установлена задача " + column.Name);
            await FindAndExecProcedure(id, messageText, "Меню");
        }

        private async Task ChooseColumn(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
            {
                session = new Session(){MediaUserId=id};
                Context.Sessions.Add(session);
            }
            session.ProcedureName = null;
            session.CurrentState = null;
            Context.SaveChanges();
            var user = Context.Users.Find(id);
            if (user == null || user.token == null)
                return;
            if (session.Location == null)
            {
                Console.WriteLine("location == null" + 94);
                return;
            }
            var desk = Helper.GetDeskById(session.Location, user.token);
            var columns = desk.GetDirectChilds(user.token);
            if (columns.Count == 0)
            {
                await Bot.Send(id, "Нет доступных колонок.");
                return;
            }
            TFColumn? column = null;
            foreach(var elem in columns)
            {
                if (elem.Name == messageText)
                    column = elem;
            }
            if (column == null)
                return;
            
            session.LocationType = TFItemType.Column;
            session.Location = column.Id;
            Context.SaveChanges();
            await Bot.Send(id, "Установлена колонка " + column.Name);
            await FindAndExecProcedure(id, messageText, "Меню");
        }
        private async Task ChooseDesk(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
            {
                session = new Session(){MediaUserId=id};
                Context.Sessions.Add(session);
            }
            session.ProcedureName = null;
            session.CurrentState = null;
            Context.SaveChanges();
            var user = Context.Users.Find(id);
            if (user == null || user.token == null)
                return;
            if (session.Location == null)
            {
                Console.WriteLine("location == null" + 135);
                return;
            }
            var project = Helper.GetProjectById(session.Location, user.token);
            var desks = project.GetDirectChilds(user.token);
            if (desks.Count == 0)
            {
                await Bot.Send(id, "Нет доступных таблиц.");
                return;
            }
            TFDesk? desk = null;
            foreach(var elem in desks)
            {
                if (elem.Name == messageText)
                    desk = elem;
            }
            if (desk == null)
                return;
            
            session.LocationType = TFItemType.Desk;
            session.Location = desk.Id;
            Context.SaveChanges();
            await Bot.Send(id, "Установлена доска " + desk.Name);
            await FindAndExecProcedure(id, messageText, "Меню");
        }

        private async Task ChooseProject(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
            {
                session = new Session(){MediaUserId=id};
                Context.Sessions.Add(session);
            }
            session.ProcedureName = null;
            session.CurrentState = null;
            Context.SaveChanges();
            var user = Context.Users.Find(id);
            if (user == null || user.token == null)
                return;
            if (session.Location == null)
            {
                Console.WriteLine("location == null" + 177);
                return;
            }
            var space = Helper.GetSpaceById(session.Location, user.token);
            var projects = space.GetDirectChilds(user.token);
            if (projects.Count == 0)
            {
                await Bot.Send(id, "Нет доступных проектов.");
                return;
            }
            TFProject? project = null;
            foreach(var elem in projects)
            {
                if (elem.Name == messageText)
                    project = elem;
            }
            if (project == null)
                return;
            
            session.LocationType = TFItemType.Project;
            session.Location = project.Id;
            Context.SaveChanges();
            await Bot.Send(id, "Установлен проект " + project.Name);
            await FindAndExecProcedure(id, messageText, "Меню");
        }

        private async Task Back(string id, string messageText)
        {
            var user = Context.Users.Find(id);
            if (user == null)
                return;
            var session = Context.Sessions.Find(id);
            if (session == null || session.Location == null || session.LocationType == null)
            {
                await Bot.Send(id, "location не выбран.");
                return;
            }
            if (session.LocationType == TFItemType.Space)
            {
                await Bot.Send(id, "вы в space.");
                return;
            }
            if (session.LocationType == TFItemType.Project)
            {
                var proj = Helper.GetProjectById(session.Location, user.token);
                var space = Helper.GetSpaceById(proj.space, user.token);
                session.LocationType = TFItemType.Space;
                session.Location = space.Id;
                Context.SaveChanges();
                await Bot.Send(id, "вы перешли в Space.");
            }
            if (session.LocationType == TFItemType.Desk)
            {
                var desk = Helper.GetDeskById(session.Location, user.token);
                var proj = Helper.GetProjectById(desk.ProjectId, user.token);
                session.LocationType = TFItemType.Project;
                session.Location = proj.Id;
                Context.SaveChanges();
                await Bot.Send(id, "вы перешли в Project.");
            }
            if (session.LocationType == TFItemType.Column)
            {
                var collumn = Helper.GetColumnById(session.Location, user.token);
                var desk = Helper.GetProjectById(collumn.boardId, user.token);
                session.LocationType = TFItemType.Desk;
                session.Location = desk.Id;
                Context.SaveChanges();
                await Bot.Send(id, "вы перешли в Desk.");
            }
            if (session.LocationType == TFItemType.Task)
            {
                var task = Helper.GetTaskById(session.Location, user.token);
                var collumn = Helper.GetColumnById(task.columnId, user.token);
                session.LocationType = TFItemType.Column;
                session.Location = collumn.Id;
                Context.SaveChanges();
                await Bot.Send(id, "вы перешли в Column.");
            }
        }

        private async Task AddComment(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
                session = new Session();
            if (session.ProcedureName == null)
            {
                session.ProcedureName = "Добавить комментарий";
                session.CurrentState = 0;
                Context.SaveChanges();
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                var spaces = Helper.GetSpaces(user.token);
                var str = new string[spaces.Count];
                int i = 0;
                foreach(var elem in spaces)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите space.", str);
                return;
            }
            if (session.CurrentState == 0)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=0});
                session.CurrentState = 1;
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                var spaces = Helper.GetSpaces(user.token);
                TFSpace? space = null;
                foreach(var elem in spaces)
                {
                    if (elem.Name == messageText)
                        space = elem;
                }
                if (space == null)
                    return;
                session.LocationType = TFItemType.Space;
                session.Location = space.Id;
                Context.SaveChanges();
                var ls = space.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите proj.", str);
                return;
            }
            if (session.CurrentState == 1)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=1});
                session.CurrentState = 2;
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var space = Helper.GetSpaceById(session.Location, user.token);
                var projects = space.GetDirectChilds(user.token);
                TFProject? proj = null;
                foreach(var elem in projects)
                {
                    if (elem.Name == messageText)
                        proj = elem;
                }
                if (proj == null)
                    return;
                session.LocationType = TFItemType.Project;
                session.Location = proj.Id;
                Context.SaveChanges();
                var ls = proj.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите board.", str);
                return;
            }
            if (session.CurrentState == 2)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=2});
                session.CurrentState = 3;
                
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var proj = Helper.GetProjectById(session.Location, user.token);
                var boards = proj.GetDirectChilds(user.token);
                TFDesk? board = null;
                foreach(var elem in boards)
                {
                    if (elem.Name == messageText)
                        board = elem;
                }
                if (board == null)
                    return;
                session.LocationType = TFItemType.Desk;
                session.Location = board.Id;
                Context.SaveChanges();
                var ls = board.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите collumns.", str);
                return;
            }
            if (session.CurrentState == 3)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=3});
                session.CurrentState = 4;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var board = Helper.GetDeskById(session.Location, user.token);
                var columns = board.GetDirectChilds(user.token);
                TFColumn? column = null;
                //if (columns != null) 
                    foreach(var elem in columns)
                    {
                        if (elem.Name == messageText)
                            column = elem;
                    }
                if (column == null)
                    return;
                session.LocationType = TFItemType.Column;
                session.Location = column.Id;
                Context.SaveChanges();

                var ls = column.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите task.", str);
                return;
            }
            if (session.CurrentState == 4)
            {
                session.CurrentState = 5;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var column = Helper.GetColumnById(session.Location, user.token);
                var tasks = column.GetDirectChilds(user.token);
                TFTask? task = null;
                foreach(var elem in tasks)
                {
                    if (elem.Name == messageText)
                        task = elem;
                }
                if (task == null)
                    return;
                session.LocationType = TFItemType.Task;
                session.Location = task.Id;
                Context.SaveChanges();
                await Bot.Send(id, "Напишите комментарий.");
                return;
            }
            if (session.CurrentState == 5)
            {
                session.CurrentState = null;
                session.ProcedureName = null;
                session.LocationType = null;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var task = Helper.GetTaskById(session.Location, user.token);
                try {task.CreateComment(messageText, user.token);}
                catch{}
                Context.SaveChanges();
                await Bot.Send(id, "Комментарий написан.");
                return;
            }
        }

        private async Task CreateTask(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
                session = new Session();
            if (session.ProcedureName == null)
            {
                session.ProcedureName = "Создание задачи";
                session.CurrentState = 0;
                Context.SaveChanges();
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                var spaces = Helper.GetSpaces(user.token);
                var str = new string[spaces.Count];
                int i = 0;
                foreach(var elem in spaces)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите space.", str);
                return;
            }
            if (session.CurrentState == 0)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=0});
                session.CurrentState = 1;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                var spaces = Helper.GetSpaces(user.token);
                TFSpace? space = null;
                foreach(var elem in spaces)
                {
                    if (elem.Name == messageText)
                        space = elem;
                }
                if (space == null)
                    return;
                session.LocationType = TFItemType.Space;
                session.Location = space.Id;
                Context.SaveChanges();
                var ls = space.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите proj.", str);
                return;
            }
            if (session.CurrentState == 1)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=1});
                session.CurrentState = 2;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var space = Helper.GetSpaceById(session.Location, user.token);
                var projects = space.GetDirectChilds(user.token);
                TFProject? proj = null;
                foreach(var elem in projects)
                {
                    if (elem.Name == messageText)
                        proj = elem;
                }
                if (proj == null)
                    return;
                session.LocationType = TFItemType.Project;
                session.Location = proj.Id;
                Context.SaveChanges();
                var ls = proj.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите board.", str);
                return;
            }
            if (session.CurrentState == 2)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=2});
                session.CurrentState = 3;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var proj = Helper.GetProjectById(session.Location, user.token);
                var boards = proj.GetDirectChilds(user.token);
                TFDesk? board = null;
                foreach(var elem in boards)
                {
                    if (elem.Name == messageText)
                        board = elem;
                }
                if (board == null)
                    return;
                session.LocationType = TFItemType.Desk;
                session.Location = board.Id;
                Context.SaveChanges();
                var ls = board.GetDirectChilds(user.token);
                var str = new string[ls.Count];
                int i = 0;
                foreach(var elem in ls)
                {
                    str[i] = elem.Name;
                    i++;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите collumns.", str);
                return;
            }
            if (session.CurrentState == 3)
            {
                //Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=3});
                session.CurrentState = 4;
            
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var board = Helper.GetDeskById(session.Location, user.token);
                var columns = board.GetDirectChilds(user.token);
                TFColumn? column = null;
                foreach(var elem in columns)
                {
                    if (elem.Name == messageText)
                        column = elem;
                }
                if (column == null)
                    return;
                session.LocationType = TFItemType.Column;
                session.Location = column.Id;
                Context.SaveChanges();
                await Bot.Send(id, "Напишите название задачи.");
                return;
            }
            if (session.CurrentState == 4)
            {
                session.CurrentState = null;
                session.ProcedureName = null;
                session.LocationType = null;
                Context.SaveChanges();
                var user = Context.Users.Find(id);
                if (user == null)
                    return;
                if (session.Location == null)
                    return;
                var column = Helper.GetColumnById(session.Location, user.token);
                string idd = column.Id;
                try {column.AddTask(messageText, "фф", out idd, user.token);}
                catch {}
                await Bot.Send(id, "Задача добавлена.");
                await FindAndExecProcedure(id, messageText, "Меню");
                return;
            }
        }

        private async Task ChooseSpace(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
            {
                session = new Session(){MediaUserId=id};
            }
            session.ProcedureName = null;
            session.CurrentState = null;
            Context.SaveChanges();
            var user = Context.Users.Find(id);
            if (user == null || user.token == null)
                return;
            var spaces = Helper.GetSpaces(user.token);
            if (spaces.Count == 0)
            {
                await Bot.Send(id, "Нет доступных пространств.");
                await FindAndExecProcedure(id, messageText, "Меню");
                return;
            }
            TFSpace? space = null;
            foreach(var elem in spaces)
            {
                if (elem.Name == messageText)
                    space = elem;
            }
            if (space == null)
            {
                return;
            }
            session.LocationType = TFItemType.Space;
            session.Location = space.Id;
            Context.SaveChanges();
            await Bot.Send(id, "Установлено пространство " + space.Name);
            await FindAndExecProcedure(id, messageText, "Меню");
        }

        private async Task forward(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null || session.Location == null || session.LocationType == null)
            {
                var user = Context.Users.Find(id);
                if (user == null || user.token == null)
                    return;
                var spaces = Helper.GetSpaces(user.token);
                if (spaces == null)
                {
                    await Bot.Send(id, "Нет доступных пространств.");
                    return;
                }
                var buttons = new string[spaces.Count];
                int i = 0;
                foreach(var elem in spaces)
                {
                    buttons[i] = elem.Name;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите пространство.", buttons);
                if (session == null)
                    Context.Sessions.Add(new Session() {MediaUserId=id, ProcedureName="Выбор пространства", CurrentState=0});
                else
                {
                    session.ProcedureName = "Выбор пространства";
                    session.CurrentState = 0;
                }
                Context.SaveChanges();
            }
            if (session.LocationType == TFItemType.Space) 
            {
                var user = Context.Users.Find(id);
                if (user == null || user.token == null)
                    return;
                var space = Helper.GetSpaceById(session.Location, user.token);
                if (space == null)
                    return;
                var projects = space.GetDirectChilds(user.token);
                if (projects == null)
                    return;
                var buttons = new string[projects.Count];
                int i = 0;
                foreach(var elem in projects)
                {
                    buttons[i] = elem.Name;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите проект.", buttons);
                
                session.ProcedureName = "Выбор проекта";
                session.CurrentState = 0;
                
                Context.SaveChanges();
            }
            if (session.LocationType == TFItemType.Project) 
            {
                var user = Context.Users.Find(id);
                if (user == null || user.token == null)
                    return;
                if (session.Location == null)
                    return;
                var project = Helper.GetProjectById(session.Location, user.token);
                if (project == null)
                    return;
                
                var desks = project.GetDirectChilds(user.token);
                if (desks == null)
                    return;
                var buttons = new string[desks.Count];
                int i = 0;
                foreach(var elem in desks)
                {
                    buttons[i] = elem.Name;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите таблицу.", buttons);
                
                session.ProcedureName = "Выбор таблицы";
                session.CurrentState = 0;
                
                Context.SaveChanges();
            }
            if (session.LocationType == TFItemType.Desk) 
            {
                var user = Context.Users.Find(id);
                if (user == null || user.token == null)
                    return;
                if (session.Location == null)
                    return;
                var desk = Helper.GetDeskById(session.Location, user.token);
                if (desk == null)
                    return;
                
                var collumns = desk.GetDirectChilds(user.token);
                if (collumns == null)
                    return;
                var buttons = new string[collumns.Count];
                int i = 0;
                foreach(var elem in collumns)
                {
                    buttons[i] = elem.Name;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите колонку.", buttons);
                
                session.ProcedureName = "Выбор колонки";
                session.CurrentState = 0;
                
                Context.SaveChanges();
            }
            if (session.LocationType == TFItemType.Column) 
            {
                var user = Context.Users.Find(id);
                if (user == null || user.token == null)
                    return;
                if (session.Location == null)
                    return;
                var desk = Helper.GetColumnById(session.Location, user.token);
                if (desk == null)
                    return;
                
                var tasks = desk.GetDirectChilds(user.token);
                if (tasks == null) 
                {
                    await Bot.Send(id, "Нет доступных задач.");
                    return;
                }
                    
                var buttons = new string[tasks.Count];
                int i = 0;
                foreach(var elem in tasks)
                {
                    buttons[i] = elem.Name;
                }
                await Bot.SendButtonsIfPossible(id, "Выберите задачу.", buttons);
                
                session.ProcedureName = "Выбор задачи";
                session.CurrentState = 0;
                
                Context.SaveChanges();
            }
        }

        static Dictionary<string, string> Discriptions = new Dictionary<string, string> {
            /*{ "/start", "Запрос на авторизацию" },
            { "/help", "Выводит все команды" },
            { "/location", "Выводит ваше рабочее пространство в TeamFlame" },*/
        }; 
        static Dictionary<string, CommandDelegate>? Commands;

        public async Task Update(string id, string messageText) 
        {
            var messsageTrim = MessageToFormat(messageText);
            Console.WriteLine(messsageTrim);

            var user = Context.Users.Find(id);
            var session = Context.Sessions.Find(id);
            if (session == null && user == null)
            {
                Context.Sessions.Add(new Session(){MediaUserId=id, CurrentState=0, ProcedureName="Авторизоваться"});
                await Bot.Send(id, "Логин:");
                Context.SaveChanges();
                return;
            }
            if (session != null && session.ProcedureName != null)
            {
                await FindAndExecProcedure(id, messsageTrim, session.ProcedureName);
                return;
            }
            await FindAndExecDelegate(id, messsageTrim);
            var ses = Context.Sessions.Find(id);
            if (ses == null || ses.ProcedureName == null)
            {
                await FindAndExecProcedure(id, messsageTrim, "Меню");
                return;
            }
        }
        private async Task Authorize(string id, string messageText)
        {
            var session = Context.Sessions.Find(id);
            if (session == null)
                return;
            if (session.CurrentState == 0)
            {
                Context.Procedures.Add(new Procedure(){MediaUserId=id, Answer=messageText, State=0});
                session.CurrentState = 1;
                Context.SaveChanges();
                await Bot.Send(id, "Пароль:");
                return;
            }
            if (session.CurrentState == 1)
            {
                var login = Context.Procedures.Find(id, 0);
                if (login == null)
                    return;
                if (login.Answer == null)
                    return;
                var responce = Helper.GetLogInResponse(login.Answer, messageText);
                Context.Sessions.Remove(session);
                Context.Procedures.Remove(login);
                Context.SaveChanges();
                if (responce == null)
                {
                    await Bot.Send(id, "Неверный логин или пароль.");
                    return;
                }
                Context.Users.Add(new User(){MediaUserId=id, token=responce.tokens.accessToken.token});
                await Bot.Send(id, "Успешно авторизованы.");
                Context.SaveChanges();
            }
        }
        private async Task DefaultMenu(string id, string messageText)
        {
            await Bot.SendButtonsIfPossible(id, "Выберите действие", new string[] {"Вперед", "Назад", "Создание задачи", "Изменение статуса", "Добавить комментарий", "Просмотреть информацию о задаче", "Посмотреть информацию о доске"});
        }
        private async Task Location(string id, string messageText)
        {
        }

        private async Task Help(string id, string messageText)
        {
        }
        public async Task Start(string id, string messageText)
        {

        }
        public async Task FindAndExecDelegate(string id, string messageText) 
        {
            if (Commands.TryGetValue(messageText, out CommandDelegate command)) 
            {
                await command(id, messageText);
                return;
            }
        }
        public async Task FindAndExecProcedure(string id, string messageText, string procedure) 
        {
            if (Commands.TryGetValue(procedure, out CommandDelegate command)) 
            {
                await command(id, messageText);
                return;
            }
        }

        public string MessageToFormat(string str)
        {
            str = str.Trim();
            return System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ");
        }
        public delegate Task CommandDelegate(string id, string messageText);
    }
}