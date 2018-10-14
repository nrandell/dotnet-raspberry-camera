using MMALSharp.Handlers;

namespace Streamer.Services
{
    public class CircularStreamHandler : StreamCaptureHandler<CircularMemoryWriteStream>
    {
        public CircularStreamHandler(CircularMemoryWriteStream stream)
        {
            CurrentStream = stream;
        }
    }
}
