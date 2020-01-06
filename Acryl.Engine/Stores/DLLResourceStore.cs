using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acryl.Engine.Stores
{
    public class DllResourceStore : ResourceStore<byte[]>
    {
        private readonly Assembly[] _assembly;

        public DllResourceStore(Assembly[] assembly)
        {
            _assembly = assembly;
        }
        
        
        public override byte[] Get(string key)
        {
            using var s = GetStream(key);

            if (s == null)
                return null;
            
            var data = new byte[s.Length];
            s.Read(data);

            return data;
        }

        public override Stream GetStream(string key)
        {
            return (
                from asm in _assembly
                    let keyName = asm.GetName().Name + "." + key.Replace("/", ".")
                    select asm.GetManifestResourceStream(keyName)).FirstOrDefault(s => s != null);
        }

        public override async Task<byte[]> GetAsync(string key)
        {
            await using var s = GetStream(key);
            
            var data = new byte[s.Length];
            await s.ReadAsync(data);

            return data;
        }

        public override void Dispose()
        {
            
        }
    }
}