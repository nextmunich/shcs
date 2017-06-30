using System.Net.Http;
using System.Threading.Tasks;

namespace Com.Apiomat.Frontend
{
    public interface IDataContainer
    {
		long Size { get; }

        HttpContent ToHttpContent();

		/// <summary>
		/// Potentially converts the data source into a byte Array.
		/// Note: The data might need to be buffered in memory, so be careful when using big files!
		/// </summary>
		/// <returns>The byte array</returns>
		Task<byte[]> ToByteArrayAsync();
    }
}