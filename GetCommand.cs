
namespace AndyTech.LevelDbClient
{
    public class GetCommand : Command
    {
        private byte[] key;

        public GetCommand(byte[] key)
        {
            this.key = key;
        }

        internal override DbCommands Code
        {
            get { return DbCommands.Get; }
        }

        protected override byte[] Data
        {
            get { return key; }
        }
    }
}
