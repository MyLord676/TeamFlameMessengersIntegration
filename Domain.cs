using System.Net;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

namespace TelegramBotExperiments 
{
    public class TFSpace : TFItem<TFProject>
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        public override List<TChild> GetChildsOfType<TChild>()
        {
            throw new NotImplementedException();
        }

        public override List<TFProject> GetDirectChilds()
        {
            throw new NotImplementedException();
        }

        public bool AddProject(TFProject project, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddProject(string projectName, out string id)
        {
            throw new NotImplementedException();
        }

        public bool SetDescription(string description)
        {
            throw new NotImplementedException();
        }
    }

    public class TFProject : TFItem<TFDesk>
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("projectKey")]
        public string ProjectKey { get; set; }

        public override List<TChild> GetChildsOfType<TChild>()
        {
            throw new NotImplementedException();
        }

        public override List<TFDesk> GetDirectChilds()
        {
            throw new NotImplementedException();
        }

        public bool AddDesk(TFDesk desk, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddDesk(string deskName, out string id)
        {
            throw new NotImplementedException();
        }

        public bool SetDescription(string description)
        {
            throw new NotImplementedException();
        }
    }

    public class TFDesk : TFItem<TFColumn>
    {
        public override List<TChild> GetChildsOfType<TChild>()
        {
            throw new NotImplementedException();
        }

        public override List<TFColumn> GetDirectChilds()
        {
            throw new NotImplementedException();
        }

        public bool AddColumn(TFColumn column, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddColumn(string columnName, out string id)
        {
            throw new NotImplementedException();
        }
    }

    public class TFColumn : TFItem<TFTask>
    {
        public override List<TChild> GetChildsOfType<TChild>()
        {
            throw new NotImplementedException();
        }

        public override List<TFTask> GetDirectChilds()
        {
            throw new NotImplementedException();
        }

        public bool AddTask(string taskName, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddTask(TFTask task, out string id)
        {
            throw new NotImplementedException();
        }
    }

    public class TFTask : TFItem<TFTask>
    {
        [JsonProperty("taskNumber")]
        public string TaskNumber { get; set; }

        [JsonProperty("priority")]
        public string PriorityString { get; set; }

        public TFTaskPriority Priority { get => throw new NotImplementedException(); }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("columnId")]
        public string columnId { get; set; }

        public override List<TChild> GetChildsOfType<TChild>()
        {
            throw new NotImplementedException();
        }

        public override List<TFTask> GetDirectChilds()
        {
            throw new NotImplementedException();
        }

        public bool AddSubtask(TFTask task, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddSubtask(string name, out string id)
        {
            throw new NotImplementedException();
        }

        public bool ChangeColumn(string newColumnId)
        {
            throw new NotImplementedException();
        }

        public bool SetDescription(string description)
        {
            throw new NotImplementedException();
        }

        public bool SetPriority(TFTaskPriority priority)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TFItem<TDefaultChild>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public abstract List<TDefaultChild> GetDirectChilds();

        public abstract List<TChild> GetChildsOfType<TChild>();
    }

    public static class Helper
    {
        internal static readonly HttpClient client = new HttpClient();
        public static TFItemType GetTFItemTypeById(string id)
        {
            throw new NotImplementedException();
        }
        public static LoginResponce? GetLogInResponse(string email, string password)
        {
            var url = "https://auth-api.test-team-flame.ru/auth/sign-in";
            var queryUrl = $"?email={email}&password={password}";

            //  var fullUrl = $"{url}{queryUrl}";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"email",email },
                {"password",password }
            };
            HttpContent content = new FormUrlEncodedContent(param);
            
            var res = client.PostAsync(url, content).Result;
            if (res.StatusCode != HttpStatusCode.OK)
                return null;
            var result = res.Content.ReadAsStringAsync().Result;
            var loginResponce = JsonConvert.DeserializeObject<LoginResponce>(result);
            return loginResponce;
        }
    }
    public class AccessToken
    {
        [JsonProperty("accessToken")]
        public string token { get; set; }
        [JsonProperty("expiresIn")]
        public long expiresIn { get; set; }
    }
    public class LoginResponce 
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("tokens")]
        public Tokens tokens { get; set; }
    }
    public class Tokens
    {
        [JsonProperty("accessToken")]
        public AccessToken accessToken { get; set; }
    }

    public enum TFItemType
    {
        Space,
        Project,
        Desk,
        Column,
        Task
    }

    public enum TFTaskPriority
    {
        None,
        Low,
        Medium,
        High
    }
}