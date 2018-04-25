using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace YoutubeToMp3
{
  public class VideoViewModel
  {
    public string Description { get; set; }
    public VideoViewModel(YouTubeVideo ytv)
    {
      this.Description = $"Bitrate: {ytv.AudioBitrate} | {ytv.FileExtension} | {ytv.Resolution}p";
    }
    public override string ToString()
    {
      return this.Description;
    }
  }
}
