using HideSnapv2;
using System;
namespace HideSnapv2
{
    public class Image
    {

        private String pathFile { set; get; }
        private bool isVideo   { set; get; }

    public Image(String name,bool isVideo)
        {
            this.pathFile = name;
            this.isVideo = isVideo;
        }
    }
}