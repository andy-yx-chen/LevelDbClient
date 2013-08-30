using System;

namespace AndyTech.LevelDbClient
{
    public class DeleteCommand : Command
    {
        private byte[] key;
        private bool batchMode;

        public DeleteCommand(byte[] key, bool batchMode)
        {
            this.key = key;
            this.batchMode = batchMode;
        }

        public DeleteCommand(byte[] key)
            : this(key, false)
        {
        }

        internal override DbCommands Code
        {
            get { return DbCommands.Delete; }
        }

        protected override bool BatchMode
        {
            get
            {
                return this.batchMode;
            }
        }

        public override bool SupportBatch
        {
            get
            {
                return true;
            }
        }

        protected override byte[] Data
        {
            get
            {
                if (this.batchMode)
                {
                    byte[] data = new byte[4 + this.key.Length];
                    Array.Copy(BitConverter.GetBytes(key.Length), 0, data, 0, 4);
                    Array.Copy(key, 0, data, 4, key.Length);
                    return data;
                }
                else
                {
                    return this.key;
                }

            }
        }
    }
}
