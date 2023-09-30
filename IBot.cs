using System.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace TelegramBotExperiments 
{
    abstract class IBot
    {
        public abstract void StartReceiving();
        public abstract Task Send(string id, string s);
        public event UpdateDelegate UpdateDelegate;
        protected virtual void RaiseSampleEvent(string id, string s)
        {
            UpdateDelegate?.Invoke(id, s);
        }  
    }
    public delegate Task UpdateDelegate(string id, string s);
}