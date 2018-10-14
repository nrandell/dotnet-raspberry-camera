using System;
using System.IO;

namespace Streamer.Services
{
    public class CircularMemoryStream : Stream
    {
        private int _length;
        private int _writePosition;
        private int _readPosition;
        private readonly byte[] _buffer;

        public CircularMemoryStream(int size)
        {
            _buffer = new byte[size];
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position { get => _length; set => throw new NotImplementedException(); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var readBuffer = _buffer;
            var length = _length;
            var readPosition = _readPosition;
            var readLength = count > length ? length : count;
            var amountToRead = readLength;
            while (amountToRead > 0)
            {
                var space = readBuffer.Length - readPosition;
                var amount = amountToRead > space ? space : amountToRead;
                Array.Copy(readBuffer, readPosition, buffer, offset, amount);
                readPosition = (readPosition + amount) % readBuffer.Length;
                offset += amount;
                amountToRead -= amount;
            }

            _readPosition = readPosition;
            return readLength;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var writeBuffer = _buffer;
            var length = _length;
            if (count > writeBuffer.Length - length)
            {
                throw new EndOfStreamException();
            }
            var writePosition = _writePosition;
            while (count > 0)
            {
                var space = writeBuffer.Length - writePosition;
                var amountToWrite = count > space ? space : count;
                Array.Copy(buffer, offset, writeBuffer, writePosition, amountToWrite);
                writePosition = (writePosition + amountToWrite) % writeBuffer.Length;
                count -= amountToWrite;
                length += amountToWrite;
            }
            _writePosition = writePosition;
            _length = length;
        }
    }
}
