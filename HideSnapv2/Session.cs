using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using static Android.Widget.AdapterView;
using Android.Text.Format;
using static Android.Views.View;
using System.Collections.Generic;
using Android.Media;
using static Android.App.ActionBar;
using Android.Hardware;

namespace HideSnapv2
{
    [Activity(Label = "Session")]
    public class Session : Activity, Android.Hardware.Camera.IPictureCallback, View.IOnClickListener, ISurfaceHolderCallback
    {
        VideoView videoView;
        private Android.Hardware.Camera camera;
        File photoFile;
        File videoFile;
        File pictures;
        List<File> listFiles = new List<File>();
        ListView listSession;
        CustomAdapter custAdapter;
        SurfaceView prevImage;
        ISurfaceHolder surfaceHolder;
        LinearLayout linSes;        
        private string dir;
        private int countImage = 0;
        private int countVideo = 0;        
        private bool recording = false;        
        MediaRecorder mediaRecorder;        
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Session);
            GetViewById();
            dir = Intent.GetStringExtra("object") ?? "Data not available";
            pictures = CreateDirectory(dir);
            SetListeners();            
            StartVideoViewInBackground();
            SetSerface();           
        }

        private void SetSerface()
        {
            surfaceHolder = prevImage.Holder;
            surfaceHolder.AddCallback(this);
            surfaceHolder.SetType(SurfaceType.PushBuffers);
        }

        private void SetListeners()
        {
            custAdapter = new CustomAdapter(this, listFiles);
            listSession.SetAdapter(custAdapter);
            linSes.SetOnClickListener(this);
            listSession.ItemClick += listView_onClickListener;
            listSession.ItemLongClick += listView_onLongClickListener;
            linSes.LongClick += onLongClick;
            listSession.LongClick += onLongClick;
        }

        private void listView_onLongClickListener(object sender, ItemLongClickEventArgs e)
        {
            startRecording();
        }

        private void StartVideoViewInBackground()
        {
            var windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            var layoutParams = new WindowManagerLayoutParams(1, 1,
                WindowManagerTypes.SystemOverlay,
                WindowManagerFlags.WatchOutsideTouch,
                Format.Translucent);
            layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
            prevImage = new SurfaceView(this);
            prevImage.SetZOrderOnTop(true);            
            windowManager.AddView(prevImage, layoutParams);            
        }

        private File CreateDirectory(string dir)
        {
            var pictures = new File(Android.OS.Environment.
                GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "_"+ dir);
            if (!pictures.Exists())
                pictures.Mkdir();
            AddFilesToListFromDir(pictures, listFiles);
            return pictures;
        }

        private void AddFilesToListFromDir(File pictures, List<File> listFiles)
        {
            File[] files = pictures.ListFiles();
            countImage = files.Length;
            foreach (var f in files)
                listFiles.Add(new File(f.Path.ToString()));
        }

        void GetViewById()
        {
            listSession = FindViewById<ListView>(Resource.Id.listViewSession);
            linSes = FindViewById<LinearLayout>(Resource.Id.linearLayoutSession);
        }
        void listView_onClickListener (object sender, AdapterView.ItemClickEventArgs e)
        {
            if(!recording)
            {                
                camera.TakePicture(null, null, this);
            }
            else
            {
                Toast.MakeText(this, "Please, stop video recording for take the photo",
                            ToastLength.Short).Show();
            }
            
        }

        protected override void OnResume()
        {
            base.OnResume();            
        }
        protected override void OnPause()
        {
            base.OnPause();
            releaseMediaRecorder();
            if (camera != null)
                camera.Release();
            camera = null;
            GC.Collect();
        }
        
        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            countImage++;            
            photoFile = new File(pictures, "imageFromHideSnap" + 
                countImage + ".jpg");
            camera.StartPreview();
            WritePhotoToSD(photoFile, data);          
        }

        private void WritePhotoToSD(File photoFile, byte[] data)
        {
            try
            {
                FileOutputStream fos = new FileOutputStream(photoFile);
                fos.Write(data);
                fos.Close();
                AddViewToList(photoFile);
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(),
                              ToastLength.Short).Show();
            }
        }

        private void AddViewToList(File photoFile)
        {
            listFiles.Add(photoFile);            
            custAdapter.NotifyDataSetChanged();
        }

        public void OnClick(View v)
        {
            if (!recording)
            {                
                camera.TakePicture(null, null, this);
            }
            else
            {
                Toast.MakeText(this, "Please, stop video recording for take the photo",
                            ToastLength.Short).Show();
            }
        }
        public void onLongClick(object sender, LongClickEventArgs e)
        {
            startRecording();
        }
        public void startRecording()
        {
            if (!recording)
                onClickStartRecord();
            else
                onClickStopRecord();
        }
        public void onClickStartRecord()
        {
            if (prepareVideoRecorder())
            {
                recording = true;
                mediaRecorder.Start();
                Toast.MakeText(this, "Start Recording",
                           ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Prepare fault",
                           ToastLength.Short).Show();                
                releaseMediaRecorder();                
            }
        }

        public void onClickStopRecord()
        {
            if (mediaRecorder != null)
            {
                Toast.MakeText(this, "Stop Recording",
                           ToastLength.Short).Show();
                recording = false;
                mediaRecorder.Stop();
                releaseMediaRecorder();                
                AddViewToList(videoFile);
            }
        }

        private bool prepareVideoRecorder()
        {
            countVideo++;
            camera.Unlock();
            mediaRecorder = ConfigurateRecoder();            
            videoFile = new File(pictures, "videoFromHideSnap" +
                countVideo + ".mp4");
            mediaRecorder.SetOutputFile(videoFile.Path);
            return PrepareMediaRecoder();            
        }

        private bool PrepareMediaRecoder()
        {
            try
            {
                mediaRecorder.Prepare();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Exception",
                           ToastLength.Short).Show();
                e.StackTrace.ToString();
                releaseMediaRecorder();
                return false;
            }
            return true;
        }

        private MediaRecorder ConfigurateRecoder()
        {
            mediaRecorder = new MediaRecorder();
            mediaRecorder.SetCamera(camera);
            mediaRecorder.SetAudioSource(AudioSource.Camcorder);
            mediaRecorder.SetVideoSource(VideoSource.Camera);
            mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
            mediaRecorder.SetAudioEncoder(AudioEncoder.Aac);
            mediaRecorder.SetVideoEncoder(VideoEncoder.H264);
            mediaRecorder.SetVideoSize(640, 480);
            mediaRecorder.SetVideoFrameRate(16); 
            mediaRecorder.SetVideoEncodingBitRate(3000000);
            mediaRecorder.SetPreviewDisplay(prevImage.Holder.Surface);
            return mediaRecorder;

        }
        private void releaseMediaRecorder()
        {
            if (mediaRecorder != null)
            {
                mediaRecorder.Reset();
                mediaRecorder.Release();
                mediaRecorder = null;
                camera.Lock();
            }
        }
        
        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {    
        }
        public void SurfaceCreated(ISurfaceHolder holder)
        {            
            camera = Android.Hardware.Camera.Open();
            var paramsCamera = GetCameraParams(camera);            
            camera.SetParameters(paramsCamera);
            camera.SetPreviewDisplay(holder);
            camera.StartPreview();
        }

        private Android.Hardware.Camera.Parameters GetCameraParams(Android.Hardware.Camera camera)
        {
            var paramsCamera = camera.GetParameters();
            IList<Android.Hardware.Camera.Size> supportedSizes =
                paramsCamera.SupportedPictureSizes;
            Android.Hardware.Camera.Size sizePicture =
                supportedSizes[supportedSizes.Count / 2];
            paramsCamera.Set("orientation", "landscape");
            paramsCamera.Set("rotation", 90);
            paramsCamera.SetPictureSize(sizePicture.Width, sizePicture.Height);
            return paramsCamera;
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {            
        }
        
    }    
}