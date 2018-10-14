using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Components;
using MMALSharp.Native;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streamer.Services
{
    public class MMALVideoStreamer : IVideoStreamer
    {
        public ILogger Logger { get; }
        private Task _runner;
        private CircularMemoryStream _stream = new CircularMemoryStream(64 * 1024 * 1024);

        public MMALVideoStreamer(ILogger<MMALVideoStreamer> logger)
        {
            Logger = logger;
            _runner = Task.Factory.StartNew(Runner, TaskCreationOptions.LongRunning).Unwrap();
        }

        private async Task Runner()
        {
            Logger.LogInformation("Starting video stream");
            var camera = MMALCamera.Instance;
            try
            {
                using (var streamHandler = new CircularStreamHandler(_stream))
                using (var videoEncoder = new MMALVideoEncoder(streamHandler))
                {
                    camera.ConfigureCameraSettings();
                    videoEncoder.ConfigureOutputPort(0, MMALEncoding.H264, MMALEncoding.I420, 0);
                    camera.Camera.VideoPort.ConnectTo(videoEncoder);
                    await Task.Delay(2000); // warm up
                    await camera.ProcessAsync(camera.Camera.VideoPort);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting video stream: {Exception}", ex.Message);
                throw;
            }
            finally
            {
                camera.Cleanup();
            }

        }

        public Task<Stream> GetVideoStream() => Task.FromResult((Stream)_stream);
    }
}
