using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

namespace TelegramBotExperiments
{
    public class TFSpace : TFItem<TFProject>
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        public override List<TChild> GetChildsOfType<TChild>(string token)
        {
            throw new NotImplementedException();
        }

        public override List<TFProject> GetDirectChilds(string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/project/projectsBySpace";
            var fullUrl = $"{urlNoParam}/{this.Id}";

            return ApiHelper<List<TFProject>>.GetObjectFromGetRequest(fullUrl, token);
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

        [JsonProperty("space")]
        public string space { get; set; }

        public override List<TChild> GetChildsOfType<TChild>(string token)
        {
            throw new NotImplementedException();
        }

        public override List<TFDesk> GetDirectChilds(string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/board/boardsByProject";
            var fullUrl = $"{urlNoParam}/{this.Id}";

            return ApiHelper<List<TFDesk>>.GetObjectFromGetRequest(fullUrl, token);
        }

        public bool AddDesk(TFDesk desk, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddDesk(string deskName, out string id, string token)
        {
            Helper.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Barrier", token);

            var url = "https://api.test-team-flame.ru/board/create";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"name",deskName },
                {"projectId",this.Id},
                {"spaceId", this.space },
                {"location", this.space }
            };
            HttpContent content = new FormUrlEncodedContent(param);

            var res = Helper.client.PostAsync(url, content);

            var statusCode = res.Result.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                id = "-1";
                return false;
            }

            var result = res.Result.Content.ReadAsStringAsync().Result;

            id = "1";
            return true;
        }

        public bool SetDescription(string description)
        {
            throw new NotImplementedException();
        }
    }

    public class TFDesk : TFItem<TFColumn>
    {
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
        public override List<TChild> GetChildsOfType<TChild>(string token)
        {
            throw new NotImplementedException();
        }

        public override List<TFColumn> GetDirectChilds(string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/column/getByBoard";
            var fullUrl = $"{urlNoParam}/{this.Id}";

            return ApiHelper<List<TFColumn>>.GetObjectFromGetRequest(fullUrl, token);
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
        [JsonProperty("boardId")]
        public string boardId { get; set; }
        public override List<TChild> GetChildsOfType<TChild>(string token)
        {
            throw new NotImplementedException();
        }

        public override List<TFTask> GetDirectChilds(string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/column/getByBoard";
            var fullUrl = $"{urlNoParam}/{this.Id}";

            return ApiHelper<List<TFTask>>.GetObjectFromGetRequest(fullUrl, token);
        }

        public bool AddTask(string taskName, string description, out string id, string token)
        {
            Helper.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Barrier", token);

            var url = "https://api.test-team-flame.ru/task/create";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"name",taskName },
                {"description",description },
                {"columnId",this.Id },
                {"location",this.Id}
            };
            HttpContent content = new FormUrlEncodedContent(param);

            var res = Helper.client.PostAsync(url, content);

            var statusCode = res.Result.StatusCode;

            /*if (statusCode != System.Net.HttpStatusCode.OK)
            {
                id = "-1";
                return false;
            }*/

            var result = res.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(res.Result.Content.ReadAsStringAsync().Result);
            var a = JsonConvert.DeserializeObject<TFTask>(result);

            id = "1";
            return true;
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

        [JsonProperty("taskNumber")]
        public string taskNumber { get; set; }

        [JsonProperty("subTasks")]
        public List<TFTask> listSubTasks { get; set; }

        public override List<TChild> GetChildsOfType<TChild>(string token)
        {
            throw new NotImplementedException();
        }

        public override List<TFTask> GetDirectChilds(string token)
        {
            return this.listSubTasks;
        }

        public bool AddSubtask(TFTask task, out string id)
        {
            throw new NotImplementedException();
        }

        public bool AddSubtask(string subTaskName, string description, string token)
        {
            Helper.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Barrier", token);

            var url = "https://api.test-team-flame.ru/task/create";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"name",subTaskName },
                {"description",description },
                {"columnId",this.columnId },
                {"location",this.Id}
            };
            HttpContent content = new FormUrlEncodedContent(param);

            var res = Helper.client.PostAsync(url, content);

            var statusCode = res.Result.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var result = res.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(res.Result.Content.ReadAsStringAsync().Result);
            var createdTask = JsonConvert.DeserializeObject<TFTask>(result);

            return chooseSubTaskToTask(createdTask.Id);
        }

        private bool chooseSubTaskToTask(string idSubtask)
        {
            var urlNoParam = "https://api.test-team-flame.ru/task/addSubtasks";
            var fullUrl = $"{urlNoParam}/{this.Id}";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"subTasks",idSubtask }
            };

            HttpContent content = new FormUrlEncodedContent(param);

            var res = Helper.client.PostAsync(fullUrl, content);

            var statusCode = res.Result.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var result = res.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(res.Result.Content.ReadAsStringAsync().Result);
            var a = JsonConvert.DeserializeObject<List<TFTask>>(result);

            return true;
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

        public bool UpdateStatus(string idStatus, string token)
        {
            Helper.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Barrier", token);

            var urlNoParam = "https://api.test-team-flame.ru/task/change-column";

            var fullUrl = $"{urlNoParam}/{this.Id}";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"columnId",idStatus},
                {"location", idStatus }
            };
            HttpContent content = new FormUrlEncodedContent(param);

            var res = Helper.client.PostAsync(fullUrl, content);

            var statusCode = res.Result.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var result = res.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(res.Result.Content.ReadAsStringAsync().Result);
            var a = JsonConvert.DeserializeObject<List<TFTask>>(result);
            //DeserializeObject<TFTask>(result);

            return true;
        }

        public bool CreateComment(string comment, string token)
        {
            Helper.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Barrier", token);

            var url = "https://api.test-team-flame.ru/comment/create";

            Dictionary<string, string> param = new Dictionary<string, string> {
                {"task",this.Id},
                {"text", comment },
                {"location", this.Id }
            };
            HttpContent content = new FormUrlEncodedContent(param);

            var res = Helper.client.PostAsync(url, content);

            var statusCode = res.Result.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var result = res.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(res.Result.Content.ReadAsStringAsync().Result);
            var a = JsonConvert.DeserializeObject<List<TFTask>>(result);

            return true;
        }
    }

    public abstract class TFItem<TDefaultChild>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("location")]
        public string location { get; set; } = "";

        public abstract List<TDefaultChild> GetDirectChilds(string token);

        public abstract List<TChild> GetChildsOfType<TChild>(string token);
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
            //Console.WriteLine(result);
            var loginResponce = JsonConvert.DeserializeObject<LoginResponce>(result);
            return loginResponce;
        }

        /*public static List<TFSpace> GetSpaces(string token) 
        {
            var url = "https://api.test-team-flame.ru/space/spacesByUserId";
            Console.WriteLine(token);
            return ApiHelper<List<TFSpace>>.GetObjectFromGetRequest(url, token);
        }*/
        public static List<TFSpace> GetSpaces(string token)
        {
            var url = "https://api.test-team-flame.ru/space/spacesByUserId";
            return ApiHelper<List<TFSpace>>.GetObjectFromGetRequest(url, token);
        }

        public static TFSpace GetSpaceById(string idSpace, string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/space";

            var fullUrl = $"{urlNoParam}/{idSpace}";

            return ApiHelper<TFSpace>.GetObjectFromGetRequest(fullUrl, token);
        }

        public static TFProject GetProjectById(string idProject, string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/project";

            var fullUrl = $"{urlNoParam}/{idProject}";

            return ApiHelper<TFProject>.GetObjectFromGetRequest(fullUrl, token);
        }

        public static TFDesk GetDeskById(string idDesk, string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/board";

            var fullUrl = $"{urlNoParam}/{idDesk}";

            return ApiHelper<TFDesk>.GetObjectFromGetRequest(fullUrl, token);
        }
        public static TFColumn GetColumnById(string idColumn, string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/column";

            var fullUrl = $"{urlNoParam}/{idColumn}";

            return ApiHelper<TFColumn>.GetObjectFromGetRequest(fullUrl, token);
        }
        public static TFTask GetTaskById(string idTask, string token)
        {
            var urlNoParam = "https://api.test-team-flame.ru/task";

            var fullUrl = $"{urlNoParam}/{idTask}";

            return ApiHelper<TFTask>.GetObjectFromGetRequest(fullUrl, token);
        }
    }
    public class AccessToken
    {
        [JsonProperty("token")]
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
    public static class ApiHelper<T>
    where T : class
    {
        public static T GetObjectFromGetRequest(string url, string token)
        {
            Helper.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var res = Helper.client.GetAsync(url);

            var statusCode = res.Result.StatusCode;

            if (statusCode != System.Net.HttpStatusCode.OK)
                return null;

            var result = res.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}