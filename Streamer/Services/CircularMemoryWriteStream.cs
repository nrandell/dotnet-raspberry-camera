using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Streamer.Services
{
    public class CircularMemoryWriteStream : Stream
    {
        internal long _length;
        internal long _position;
        internal readonly byte[] _buffer;

        public ILogger Logger { get; }

        public CircularMemoryWriteStream(int size, ILogger logger)
        {
            _buffer = new byte[size];
            Logger = logger;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position { get => _position; set => throw new InvalidOperationException("Cannot set position"); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException("Cannot read");

        public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException("Cannot seek");

        public override void SetLength(long value) => throw new InvalidOperationException("Cannot set length");

        public override void Write(byte[] buffer, int offset, int count)
        {
            var writeBuffer = _buffer;
            if (count > writeBuffer.Length)
            {
                throw new ArgumentOutOfRangeException("Too much data to write");
            }
            var length = _length;
            var position = _position;
            var writePosition = (int)(position % writeBuffer.Length);
            while (count > 0)
            {
                var space = writeBuffer.Length - writePosition;
                var amountToWrite = count > space ? space : count;
                Array.Copy(buffer, offset, writeBuffer, writePosition, amountToWrite);
                writePosition = (writePosition + amountToWrite) % writeBuffer.Length;
                count -= amountToWrite;
                length += amountToWrite;
                position += amountToWrite;
            }
            _position = position;
            _length = length;
            Logger.LogInformation("Write to {Position}/{Length}", _position, _length);
        }
    }
}
