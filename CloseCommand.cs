namespace AndyTech.LevelDbClient
{
    public class CloseCommand : Command
    {
        internal override DbCommands Code
        {
            get { return DbCommands.Close; }
        }

        protected override byte[] Data
        {
            get { return new byte[0]; }
        }
    }
}
