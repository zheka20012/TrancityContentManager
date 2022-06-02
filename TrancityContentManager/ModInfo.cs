using System;
using System.Drawing;

namespace TrancityContentManager
{
    public struct ModInfo
    {
        public string Id;
        public ModType Type;
        public Bitmap Image;
        public string Name;
        public string Version;
        public string DateUploaded;
        public string DownloadLink;
        public string FileSize;
        public string Uploader;
    }
}