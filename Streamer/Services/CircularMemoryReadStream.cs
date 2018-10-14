using System;
using System.IO;

namespace Streamer.Services
{
    public class CircularMemoryReadStream : Stream
    {
        private long _position;
        private readonly CircularMemoryWriteStream _stream;

        public CircularMemoryReadStream(CircularMemoryWriteStream stream)
        {
            _stream = stream;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _stream._length;

        public override long Position { get => _position; set => throw new InvalidOperationException("Cannot set position"); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var readBuffer = _stream._buffer;

            var length = (int)(_stream._length - _position);
            var position = _position;
            var readPosition = (int)(position % readBuffer.Length);
            var readLength = count > length ? length : count;
            var remaining = readLength;
            while (remaining > 0)
            {
                var space = readBuffer.Length - readPosition;
                var amountToRead = remaining > space ? space : remaining;
                Array.Copy(readBuffer, readPosition, buffer, offset, amountToRead);
                readPosition = (readPosition + amountToRead) % readBuffer.Length;
                position += amountToRead;
                offset += amountToRead;
                remaining -= amountToRead;
            }

            _position = position;
            return readLength;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException("Cannot seek");

        public override void SetLength(long value) => throw new InvalidOperationException("Cannot set length");

        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException("Cannot write");
    }
}
