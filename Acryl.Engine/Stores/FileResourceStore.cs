using System;
using System.IO;
using System.Threading.Tasks;

namespace Acryl.Engine.Stores
{
    public class FileResourceStore : ResourceStore<byte[]>
    {
        public override byte[] Get(string key)
        {
            var uri = new Uri(key);

            return File.ReadAllBytes(uri.LocalPath);
        }

        public override Task<byte[]> GetAsync(string key)
        {
            var uri = new Uri(key);
            
            return File.ReadAllBytesAsync(uri.LocalPath);
        }

        public override Stream GetStream(string key)
        {
            var uri = new Uri(key);
            
            return File.Open(uri.LocalPath, FileMode.OpenOrCreate);
        }

        public override void Dispose()
        {
        }
    }
}