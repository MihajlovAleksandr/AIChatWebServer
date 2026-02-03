namespace AIChatWebServer.Utils.Interfaces
{
    public interface IStringChanger
    {
        string Replace(string text, string oldValue, string newValue, int count);
    }
}
