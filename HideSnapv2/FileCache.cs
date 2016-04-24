using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace HideSnapv2
{
    public class FileCache
    {
        private static File cacheDir;
        private File[] fileList;
        private int listLength;

        public FileCache(Context context, File dir)
        {
            cacheDir = dir;
            fileList = cacheDir.ListFiles();
            listLength = fileList.Length;
        }
        public int length()
        {
            return listLength;
        }
        public File getFile(int position)
        {
            return fileList[position];
        }

        public void clear()
        {
            File[] files = cacheDir.ListFiles();
            if (files == null)
                return;
            foreach (var  f in files)
                f.Delete();
        }

    }
}