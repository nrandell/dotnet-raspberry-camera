using Microsoft.AspNetCore.Mvc;
using Streamer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamer.Controllers
{
    [ApiController]
    [Route("/api/video")]
    public class VideoStreamController :Controller
    {
        private readonly IVideoStreamer _streamer;

        public VideoStreamController(IVideoStreamer streamer)
        {
            _streamer = streamer;
        }

        [HttpGet()]
        public async Task<FileStreamResult> Get()
        {
            var stream = await _streamer.GetVideoStream();
            return new FileStreamResult(stream, "video/mp4");
        }
    }
}
