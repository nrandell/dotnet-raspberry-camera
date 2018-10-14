using Microsoft.AspNetCore.Hosting;
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
        public IApplicationLifetime Lifetime { get; }
        private readonly Task _runner;
        private readonly CircularMemoryWriteStream _stream;

        public MMALVideoStreamer(ILogger<MMALVideoStreamer> logger, IApplicationLifetime lifetime)
        {
            Logger = logger;
            Lifetime = lifetime;
            _runner = Task.Factory.StartNew(Runner, TaskCreationOptions.LongRunning).Unwrap();
            _stream = new CircularMemoryWriteStream(4 * 1024 * 1024, Logger);
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
                    Logger.LogInformation("Ready to stream");
                    await camera.ProcessAsync(camera.Camera.VideoPort, Lifetime.ApplicationStopping);
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

        public Task<Stream> GetVideoStream() => Task.FromResult((Stream)new CircularMemoryReadStream(_stream, Logger));
    }
}
