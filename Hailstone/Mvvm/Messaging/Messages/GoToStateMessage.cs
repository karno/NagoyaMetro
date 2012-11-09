
namespace Hailstone.Mvvm.Messaging.Messages
{
    public class GoToStateMessage : Message
    {
        public GoToStateMessage(string stateName, bool useTransitions = true)
            : base()
        {
            this.StateName = stateName;
            this.UseTransisions = useTransitions;
        }

        public GoToStateMessage(string key, string stateName, bool useTransitions = true)
            : base(key)
        {
            this.StateName = stateName;
            this.UseTransisions = useTransitions;
        }

        public string StateName { get; set; }

        public bool UseTransisions { get; set; }

        public bool Result { get; set; }
    }
}
