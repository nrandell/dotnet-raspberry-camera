using MMALSharp.Handlers;

namespace Streamer.Services
{
    public class CircularStreamHandler : StreamCaptureHandler<CircularMemoryStream>
    {
        public CircularStreamHandler(CircularMemoryStream stream)
        {
            CurrentStream = stream;
        }
    }
}
