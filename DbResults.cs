
namespace AndyTech.LevelDbClient
{
    public enum DbResults : int
    {
        OK = 0,
        DataError = 400,
        UnAuth = 401,
        NoDb = 402,
        NoDbSelected,
        BadCommand,
        NotFound,
        IoError = 501,
        CreateFailed,
        DbError,
    }
}
