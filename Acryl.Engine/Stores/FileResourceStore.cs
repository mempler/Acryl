using System;
using System.IO;
using System.Threading.Tasks;

namespace Acryl.Engine.Stores
{
    public class FileResourceStore : ResourceStore<byte[]>
    {
        public override byte[] Get(string key)
        {
            return File.ReadAllBytes(key);
        }

        public override Task<byte[]> GetAsync(string key)
        {
            return File.ReadAllBytesAsync(key);
        }

        public override Stream GetStream(string key)
        {
            return File.Open(key, FileMode.OpenOrCreate);
        }

        public override void Dispose()
        {
        }
    }
}