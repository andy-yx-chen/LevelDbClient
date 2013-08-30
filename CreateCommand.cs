using System.Text;

namespace AndyTech.LevelDbClient
{
    public class CreateCommand : Command
    {
        private string dbName;

        public CreateCommand(string dbName)
        {
            this.dbName = dbName;
        }

        internal override DbCommands Code
        {
            get { return DbCommands.Create; }
        }

        protected override byte[] Data
        {
            get { return Encoding.ASCII.GetBytes(dbName); }
        }
    }
}
