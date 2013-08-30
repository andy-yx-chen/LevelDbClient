using System;
using System.Collections.Generic;

namespace AndyTech.LevelDbClient
{
    public class BatchCommand : Command
    {
        private List<Command> commands = new List<Command>();

        internal override DbCommands Code
        {
            get { return DbCommands.Batch; }
        }

        public BatchCommand AddCommand(Command command)
        {
            if (!command.SupportBatch)
            {
                throw new NotSupportedException(string.Format("{0} command does not support batch mode", command.GetType().Name));
            }
            this.commands.Add(command);
            return this;
        }

        protected override byte[] Data
        {
            get
            {
                List<byte[]> commandBuffers = new List<byte[]>();
                int totalBufferSize = 0;
                for (int i = 0; i < commands.Count; ++i)
                {
                    byte[] item = commands[i].ToBuffer();
                    totalBufferSize += item.Length;
                    commandBuffers.Add(item);
                }

                byte[] commandData = new byte[totalBufferSize + 4];
                int cursor = 0;

                Array.Copy(BitConverter.GetBytes(commandBuffers.Count), 0, commandData, cursor, 4);
                cursor += 4;

                for (int i = 0; i < commandBuffers.Count; ++i)
                {
                    Array.Copy(commandBuffers[i], 0, commandData, cursor, commandBuffers[i].Length);
                    cursor += commandBuffers[i].Length;
                }

                return commandData;
            }
        }
    }
}
