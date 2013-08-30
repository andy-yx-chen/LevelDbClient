using System;

namespace AndyTech.LevelDbClient
{
    public abstract class Command
    {
        internal abstract DbCommands Code
        {
            get;
        }

        public virtual bool SupportBatch
        {
            get
            {
                return false;
            }
        }

        protected abstract byte[] Data
        {
            get;
        }

        protected virtual bool BatchMode
        {
            get
            {
                return false;
            }
        }

        public virtual byte[] ToBuffer()
        {
            int dataLength = Data.Length;
            byte[] codeBuffer = BitConverter.GetBytes((int)Code);
            byte[] lengthBuffer = BitConverter.GetBytes(dataLength);
            byte[] commandBuffer = null;
            int cursor = 0;

            if (!BatchMode)
            {
                commandBuffer = new byte[codeBuffer.Length + lengthBuffer.Length + dataLength];
            }
            else
            {
                commandBuffer = new byte[codeBuffer.Length + dataLength];
            }

            Array.Copy(codeBuffer, 0, commandBuffer, cursor, codeBuffer.Length);
            cursor += codeBuffer.Length;

            if (!BatchMode)
            {
                Array.Copy(lengthBuffer, 0, commandBuffer, cursor, lengthBuffer.Length);
                cursor += lengthBuffer.Length;
            }

            if (dataLength != 0)
            {
                Array.Copy(Data, 0, commandBuffer, cursor, dataLength);
            }

            return commandBuffer;
        }
    }
}
