using System.IO;
using System.Threading.Tasks;

namespace Streamer.Services
{
    public interface IVideoStreamer
    {
        Task<Stream> GetVideoStream();
    }
}
