
namespace AndyTech.LevelDbClient
{
    internal enum DbCommands : int
    {
        Login = 1,
        Open,
        Close,
        Put,
        Batch,
        Get,
        Delete,
        List,
        Create
    }
}
