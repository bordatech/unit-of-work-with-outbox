namespace Borda.UnitOfWork
{
    public class EventKeyAttribute : System.Attribute
    {
        public EventKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}