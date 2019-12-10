using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Acryl.Engine.Stores
{
    public class OnlineStore : ResourceStore<byte[]>
    {
        public override byte[] Get(string url)
        {
            using var s = GetStream(url);

            var b = new byte[s.Length];
            s.Read(b);

            return b;
        }

        public override async Task<byte[]> GetAsync(string url)
        {
            await using var s = GetStream(url);

            var b = new byte[s.Length];
            await s.ReadAsync(b);

            return b;
        }

        public override Stream GetStream(string url)
        {
            var req = WebRequest.Create(url);
            using var response = req.GetResponse();
            using var rs = response.GetResponseStream();

            if (rs == null)
                return null;
            
            var s = new MemoryStream();
            rs.CopyTo(s);

            return s;
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}