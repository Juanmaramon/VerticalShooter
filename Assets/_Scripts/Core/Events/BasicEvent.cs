namespace Core
{
    // Data received on event
    public class BasicEvent : GameEvent
    {
        private object data;

        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        public BasicEvent(object data)
        {
            this.data = data;
        }

        public BasicEvent() { }
    }
}