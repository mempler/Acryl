using System;
using System.IO;
using ManagedBass;

namespace Acryl.Audio
{
    public class AudioStream : IDisposable
    {
        private int stream;
        
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }

        public double Volume
        {
            get => Bass.ChannelGetAttribute(stream, ChannelAttribute.Volume);
            set => Bass.ChannelSetAttribute(stream, ChannelAttribute.Volume, value);
        }
        
        public long Position
        {
            get => Bass.ChannelGetPosition(stream);
            set => Bass.ChannelSetPosition(stream, value);
        }
        
        internal AudioStream(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Audio File not found!", path);

            stream = Bass.CreateStream(path);
            if (stream == 0)
                throw new BassException(Bass.LastError);
        }

        public void Play()
        {
            Bass.ChannelPlay(stream);
            IsPlaying = true;
            IsPaused = false;
        }
        
        public void Stop()
        {
            Bass.ChannelStop(stream);
            IsPlaying = false;
        }

        public void Pause()
        {
            Bass.ChannelPause(stream);
            IsPaused = true;
            IsPlaying = false;
        }

        public void Dispose()
        {
            Bass.StreamFree(stream);
        }
    }
}