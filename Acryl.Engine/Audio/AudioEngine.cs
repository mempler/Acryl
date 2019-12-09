using System;
using ManagedBass;

namespace Acryl.Engine.Audio
{
    public class AudioEngine : IDisposable
    {
        public AudioEngine()
        {
            if (!Bass.Init())
                throw new BassException(Bass.LastError);
        }
        
        public void Dispose()
        {
            Bass.Free();
        }

        public AudioStream FromFile(string path)
            => new AudioStream(path);
    }
}