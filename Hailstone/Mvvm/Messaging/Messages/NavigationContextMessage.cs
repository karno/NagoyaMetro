
namespace Hailstone.Mvvm.Messaging.Messages
{
    public class NavigationContextMessage : Message
    {
        public NavigationContextMessage(string query)
            : base()
        {
            this.Query = query;
        }

        public NavigationContextMessage(string key, string query)
            : base(key)
        {
            this.Query = query;
        }

        public string Query { get; set; }

        public string Response { get; set; }
    }
}
