using System;
using Android.App;
using Android.Views;
using Android.Widget;
using HideSnapv2;
using Java.Lang;
using static Android.Resource;
using Android.Content;
using Java.IO;
using System.Collections.Generic;
using static Android.Views.View;

public class CustomAdapter : BaseAdapter
{

    private Activity activity;
    private static LayoutInflater inflater = null;
    public ImageLoader imageLoader;
    List<File> listFiles;

    public override int Count
    {
        get
        {
            return listFiles.Count;
        }
    }
    public CustomAdapter(Activity a, List<File> listFiles)
    {
        activity = a;
        this.listFiles = listFiles;
        inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
        imageLoader = new ImageLoader(activity);
    }

    public override Java.Lang.Object GetItem(int position)
    {
        return position;
    }

    public override long GetItemId(int position)
    {
        return position;
    }

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        View vi = convertView;
        if (convertView == null)
            vi = inflater.Inflate(Resource.Layout.ItemList, null);        
        ImageView image = (ImageView)vi.FindViewById(Resource.Id.imgView);
        ImageView play = (ImageView)vi.FindViewById(Resource.Id.playView);        
        
        play.Click += delegate { onClickPlayVideo(listFiles[position]); };        
        imageLoader.DisplayImage(listFiles[position], image);
        if (!imageLoader.isVideo(listFiles[position]))
        {
            play.Visibility = ViewStates.Invisible;
        }
        else
            play.Visibility = ViewStates.Visible;
        return vi;
    }

    private void onClickPlayVideo(File file)
    {
        var uri = Android.Net.Uri.FromFile(file);
        var intent = new Android.Content.Intent(Intent.ActionView, uri);
        intent.SetDataAndType(uri, "video/*");
        activity.StartActivityForResult(intent, 0);
    }
}
