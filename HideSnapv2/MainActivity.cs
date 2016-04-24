using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Media;
using Android.Runtime;
using System.Threading.Tasks;
using Android.Content;
using static Android.Views.View;
using Java.IO;
using System.Collections.Generic;

namespace HideSnapv2
{
    [Activity(Label = "HideSnapv2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button btnAdd;
        private EditText editName;
        private List<string> foldersName = new List<string>();
        private LinearLayout layoutObjects;
        private const int maxButtons = 8;
        private int countButtons = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            GetViewById();
            btnAdd.Click += (object sender, EventArgs e) =>
            {
                btnAddClickListener();
            };
        }

        private void btnAddClickListener()
        {
            if (countButtons == maxButtons)
            {
                Toast.MakeText(this, "The maximum number of objects", ToastLength.Short).Show();
            }
            else
            {
                CreateNewButton(editName.Text.ToString());                
            }
        }

        private void CreateNewButton(string buttonName)
        {
            int currentButtons = countButtons;
            countButtons++;
            Button btnNew = new Button(this);
            btnNew.Text = (buttonName);            
            foldersName.Add(buttonName);
            btnNew.Click += (object sender2, EventArgs e2) =>
            {
                StartNewSession(buttonName);
            };
            layoutObjects.AddView(btnNew);
            btnNew.LongClick += (object sender3, LongClickEventArgs w) =>
            {
                NewButtonLongClickListener(btnNew);               
            };
        }

        private void NewButtonLongClickListener(Button button)
        {
            Toast.MakeText(this, "Object " + button.Text +
                   "was deleted", ToastLength.Short).Show();
            DeleteFolder(getFolder(button.Text));
            layoutObjects.RemoveView(button);
            countButtons--;
        }

        private void StartNewSession(string v)
        {
            var activitySession = new Intent(this, typeof(Session));
            activitySession.PutExtra("object", v);
            StartActivity(activitySession);
        }

        private void GetViewById()
        {
            btnAdd = FindViewById<Button>(Resource.Id.buttonAddObject);
            editName = FindViewById<EditText>(Resource.Id.EditTextName);
            layoutObjects = FindViewById<LinearLayout>(Resource.Id.linearLayoutObjects);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            foreach (var f in foldersName) {
                var fileOrDirectory = getFolder(f);
                DeleteFolder(fileOrDirectory);
                
            }
            Finish();
        }
        File getFolder(string f)
        {
            return new File(Android.OS.Environment.
                GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures) + "_" + f);
        }
        void DeleteFolder(File fileOrDirectory)
        {
            if (fileOrDirectory.IsDirectory)
            {
                string[] children = fileOrDirectory.List();
                for (int i = 0; i < children.Length; i++)
                {
                    new File(fileOrDirectory, children[i]).Delete();
                }
                fileOrDirectory.Delete();
            }

        }
    }


}

