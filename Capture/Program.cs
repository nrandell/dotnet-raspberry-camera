using MMALSharp;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Threading.Tasks;

namespace Capture
{
    internal static class Program
    {
#pragma warning disable RCS1163 // Unused parameter.
        private static async Task Main(string[] args)
#pragma warning restore RCS1163 // Unused parameter.
        {
            Console.WriteLine("Hello World!");
            try
            {
                var cam = MMALCamera.Instance;
                try
                {
                    using (var imgCaptureHandler = new ImageStreamCaptureHandler("images/", "jpg"))
                    {
                        await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                    }
                }
                finally
                {
                    cam.Cleanup();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
