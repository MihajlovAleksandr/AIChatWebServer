namespace AIChatWebServer.Models.AI
{
    public enum TokenOperation
    {
        SendMessage = 1,
        Translate = 2,
        CompressMessage = 3,
        CompressDialog = 4,
        SystemCompressMessage = 5,
        SystemCompressDialog = 6,
        GeneratePersona = 7, 
        ClassifyQuery = 8
    }
}
