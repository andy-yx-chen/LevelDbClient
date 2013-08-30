using System;

namespace AndyTech.LevelDbClient
{
    public class PutCommand : Command
    {
        private bool batchMode;
        private byte[] key;
        private byte[] value;

        public PutCommand(byte[] key, byte[] value)
            : this(key, value, false)
        {
        }

        public PutCommand(byte[] key, byte[] value, bool batchMode)
        {
            this.key = key;
            this.value = value;
            this.batchMode = batchMode;
        }

        public override bool SupportBatch
        {
            get
            {
                return true;
            }
        }

        internal override DbCommands Code
        {
            get { return DbCommands.Put; }
        }

        protected override bool BatchMode
        {
            get
            {
                return this.batchMode;
            }
        }

        protected override byte[] Data
        {
            get 
            {
                byte[] commandData = new byte[8 + key.Length + value.Length];
                int cursor = 0;
                Array.Copy(BitConverter.GetBytes(key.Length), 0, commandData, cursor, 4);
                cursor += 4;
                Array.Copy(BitConverter.GetBytes(value.Length), 0, commandData, cursor, 4);
                cursor += 4;
                Array.Copy(key, 0, commandData, cursor, key.Length);
                cursor += key.Length;
                Array.Copy(value, 0, commandData, cursor, value.Length);
                return commandData;
            }
        }
    }
}
