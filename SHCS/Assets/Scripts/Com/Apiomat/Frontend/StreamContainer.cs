using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.Apiomat.Frontend
{
	internal class StreamContainer : IDataContainer
    {
		private readonly Stream data;

		private long _size = Int64.MinValue;

		public long Size
		{
			get
			{
				/* Might throw a NotSupportedException if the Stream implementation doesn't support it.
				 * We could check its CanSeek attribute, which indicates if Length is supported,
				 * but returning some value like `Datastore.Instance.MaxOfflineFileBytes + 1` or Int64.MaxValue would be wrong.
				 * Also, trying to manually start buffering the stream to count ourselves,
				 * with a maximum cap of Datastore.Instance.MaxOfflineFileBytes doesn't work either,
				 * because all data up until that point would be lost then, because we can't reset the stream
				 * (due to CanSeek false also indicating that Position can't be changed).
				 * So throwing an exception and handling that exception is the best way. */
				return data.Length;
			}
		}

        public StreamContainer(Stream stream)
        {
            this.data = stream;
        }

		public HttpContent ToHttpContent()
        {
			return new StreamContent(data);
        }

		public async Task<byte[]> ToByteArrayAsync()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				await this.data.CopyToAsync(ms).ConfigureAwait(false);
				return ms.ToArray();
			}
		}
    }
}
