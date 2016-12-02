using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooky
{
    /// <summary>
    /// An <see cref="IRpcTransport"/> implementation that uses <see cref="System.IO.Pipes"/> to transmit RPC messages.
    /// </summary>
    public class NamedPipesTransport : IRpcTransport
    {
        private string _namedPipesAddress = string.Empty;
        
        /// <summary>
        /// Creates a new <see cref="NamedPipesTransport"/> instance using the specified Named Pipes address.
        /// </summary>
        /// <param name="namedPipesAddress"></param>
        public NamedPipesTransport(string namedPipesAddress)
        {
            _namedPipesAddress = namedPipesAddress;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public async Task<Stream> SendRequest(Stream requestContent)
        {
            string response = string.Empty;

            try
            {
                using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream(_namedPipesAddress))
                {
                    namedPipeClient.Connect();

                    var messageBytes = await ReadFullyAsync(requestContent);

                    // sending our request
                    namedPipeClient.Write(messageBytes, 0, messageBytes.Length);

                    var ss = new StreamString(namedPipeClient);

                    // retrieving response
                    response = await ss.ReadStringAsync();
                }
            }
            catch
            {
                throw;
            }

            return await GenerateStreamFromString(response).ConfigureAwait(false);
        }

        public static async Task<Stream> GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            await writer.WriteAsync(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static async Task<byte[]> ReadFullyAsync(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    } // class
} // namespace
