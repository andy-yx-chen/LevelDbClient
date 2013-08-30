using System.Text;

namespace AndyTech.LevelDbClient
{
    public class OpenCommand : Command
    {
        private string dbName;

        public OpenCommand(string dbName)
        {
            this.dbName = dbName;
        }

        internal override DbCommands Code
        {
            get { return DbCommands.Open; }
        }

        protected override byte[] Data
        {
            get
            {
                return Encoding.ASCII.GetBytes(dbName);
            }
        }
    }
}
