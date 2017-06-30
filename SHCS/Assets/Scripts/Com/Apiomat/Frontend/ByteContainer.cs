using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Com.Apiomat.Frontend
{
	internal class ByteContainer : IDataContainer
    {
		private readonly byte[] data;

		public long Size { get { return data.LongLength; } }

        public ByteContainer(byte[] byteArray)
        {
			this.data = byteArray;
        }

		public HttpContent ToHttpContent()
        {
			return new ByteArrayContent(data);
        }

		public async Task<byte[]> ToByteArrayAsync()
		{
			return await Task.Run<byte[]>(() => this.data).ConfigureAwait(false);
		}
    }
}
