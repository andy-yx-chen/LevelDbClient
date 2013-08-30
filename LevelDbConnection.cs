using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AndyTech.LevelDbClient
{
    public class LevelDbConnection : IDisposable
    {
        private const int DefaultPort = 4406;

        private Socket clientSocket;
        private AsyncSemaphore socketLock;
        private bool opened;

        public LevelDbConnection()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketLock = new AsyncSemaphore(1);
            opened = false;
        }

        public async Task<bool> Connect(string host, int port)
        {
            try
            {
                await Task.Factory.FromAsync(clientSocket.BeginConnect, clientSocket.EndConnect, host, port, null);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Connect(string host)
        {
            return await Connect(host, DefaultPort);
        }

        public async Task<DbResults> Create(string dbName)
        {
            Response response = await ExecuteCommandInTemplate(new CreateCommand(dbName));
            return response.Result;
        }

        public async Task<DbResults> Open(string dbName)
        {
            Response response = await ExecuteCommandInTemplate(new OpenCommand(dbName));
            if (response.Result == DbResults.OK)
            {
                this.opened = true;
            }
            return response.Result;
        }

        public Batch CreateBatch()
        {
            return new Batch(this);
        }

        public async Task<DbResults> Put(byte[] key, byte[] value)
        {
            Response response = await ExecuteCommandInTemplate(new PutCommand(key, value));
            return response.Result;
        }

        public async Task<byte[]> Get(byte[] key)
        {
            Response response = await ExecuteCommandInTemplate(new GetCommand(key));
            if (response.Result == DbResults.OK)
            {
                return response.Data;
            }
            else if (response.Result == DbResults.NotFound)
            {
                return null;
            }

            throw new ApplicationException("suffer db error " + response.Result);
        }

        public async Task<bool> Delete(byte[] key)
        {
            Response response = await ExecuteCommandInTemplate(new DeleteCommand(key));
            return response.Result == DbResults.OK || response.Result == DbResults.NotFound;
        }

        public async Task Close()
        {
            if (!this.opened)
            {
                return;
            }

            await ExecuteCommandInTemplate(new CloseCommand());
        }

        private async Task<Response> ExecuteCommandInTemplate(Command command)
        {
            try
            {
                await socketLock.Acquire();
                return await ExecuteCommand(command);
            }
            finally
            {
                socketLock.Release();
            }
        }

        private async Task<Response> ExecuteCommand(Command command)
        {
            EnsureSocketConnected();
            byte[] cmdData = command.ToBuffer();
            IAsyncResult ar = clientSocket.BeginSend(cmdData, 0, cmdData.Length, SocketFlags.None, null, null);
            await Task.Factory.FromAsync<int>(ar, clientSocket.EndSend);
            EnsureSocketConnected();
            byte[] header = new byte[8];
            ar = clientSocket.BeginReceive(header, 0, header.Length, SocketFlags.None, null, null);
            await Task.Factory.FromAsync<int>(ar, clientSocket.EndReceive);
            EnsureSocketConnected();
            int status = BitConverter.ToInt32(header, 0);
            int dataLength = BitConverter.ToInt32(header, 4);
            Response response = new Response();
            response.Result = (DbResults)status;
            if (dataLength > 0)
            {
                byte[] data = new byte[dataLength];
                EnsureSocketConnected();
                ar = clientSocket.BeginReceive(data, 0, data.Length, SocketFlags.None, null, null);
                await Task.Factory.FromAsync<int>(ar, clientSocket.EndReceive);
                response.Data = data;
            }
            return response;
        }

        private void EnsureSocketConnected()
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                throw new InvalidOperationException("no connection");
            }
        }

        public void Dispose()
        {
            if (clientSocket != null)
            {
                clientSocket.Dispose();
            }
        }

        internal class Response
        {
            internal DbResults Result { get; set; }
            internal byte[] Data { get; set; }
        }

        public class Batch
        {
            private LevelDbConnection connection;
            private BatchCommand command;

            public Batch(LevelDbConnection connection)
            {
                this.connection = connection;
                command = new BatchCommand();
            }

            public Batch Put(byte[] key, byte[] value)
            {
                this.command.AddCommand(new PutCommand(key, value, true));
                return this;
            }

            public Batch Delete(byte[] key)
            {
                this.command.AddCommand(new DeleteCommand(key, true));
                return this;
            }

            public async Task<DbResults> Execute()
            {
                return (await this.connection.ExecuteCommandInTemplate(this.command)).Result;
            }
        }
    }
}
