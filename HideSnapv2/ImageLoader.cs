using HideSnapv2;
using Android.Widget;
using Java.Lang;
using Android.Graphics;
using Java.IO;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Util;
using Android.Media;
using Android.Provider;
using System;

namespace HideSnapv2
{
    public class ImageLoader
    {
        FileCache fileCache;
        File directory;
        Context context;

        public ImageLoader(Context context)
        {
            this.context = context;
        }

        public int countFiles()
        {
          
            return fileCache.length();
        }
        public void DisplayImage(File file, ImageView imageView)
        {
            Bitmap bitmap = getBitmap(file);
            if (bitmap != null)
                imageView.SetImageBitmap(bitmap);
        }

        public bool isVideo(File file)
        {
            var filename = file.Path;
            if (file.Name.EndsWith(".mp4"))
            {
                return true;
            }
            else
                return false;
        }
        private Bitmap getBitmap(File f)
        {            
            Bitmap b = decodeFile(f);
            if (b != null)
            {                
                return b;
            }
            return null;
        }
        
        private Bitmap decodeFile(File f)
        {
            var filename = f.Path;
            if (!f.Name.EndsWith(".jpg"))
            {
                return GetThunbailFromVideo(filename, ThumbnailKind.FullScreenKind);
            }
            else
                return GetImage(filename, 1);            
        }

        private Bitmap GetImage(string filename, int scale)
        {
            var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = scale;
            Bitmap bitmap = BitmapFactory.DecodeStream(fs, null, options);
            fs.Close();
            return bitmap;
        }

        private Bitmap GetThunbailFromVideo(string filename, ThumbnailKind ScreenKind)
        {
            Bitmap thumb = ThumbnailUtils.CreateVideoThumbnail(filename,
                                ScreenKind);            
            return RotateBitmap(thumb,90);
        }
        public static Bitmap RotateBitmap(Bitmap source, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(angle);
            return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
        }
    }
}

