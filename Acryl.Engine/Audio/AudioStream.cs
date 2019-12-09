using System;
using System.IO;
using ManagedBass;

namespace Acryl.Engine.Audio
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
        
        public double Position
        {
            get => Bass.ChannelBytes2Seconds(stream, Bass.ChannelGetPosition(stream)) * 1000f;
            set => Bass.ChannelSetPosition(stream, Bass.ChannelSeconds2Bytes(stream, value / 1000f));
        }
        
        public float LeftChannel => Bass.ChannelGetLevelLeft(stream) / 32768f;
        public float RightChannel => Bass.ChannelGetLevelRight(stream) / 32768f;

        public float Maximum => Math.Max(LeftChannel, RightChannel);

        public float[] LowFreqSampleRate
        {
            get
            {
                var data = new float[256/4];
                Bass.ChannelGetData(stream, data, 256);
                
                return data;
            }
        }

        public float Average => (LeftChannel + RightChannel) / 2;

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