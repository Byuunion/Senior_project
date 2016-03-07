using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable.Droid
{
    [Activity(Label = "MeetMeet_Native_Portable.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		private Button mButtonSignUp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Person test = new Person();
            System.Diagnostics.Debug.WriteLine("hello");
            PostUser("", test);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
			mButtonSignUp = FindViewById<Button> (Resource.Id.SignUpButton);
			mButtonSignUp.Click += mButtonSignUp_Click;

			// Option: If you wanted click to open new Layout

			//Button buttonsignup = FindViewById<Button> (Resource.Id.SignUpButton);
			//buttonsignup.Click += delegate {
				//SetContentView (Resource.Layout.Signup);
			//};

        }

		void mButtonSignUp_Click (object sender, EventArgs e)
		{
			//Pull up dialog
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			SignUp signUpDialog = new SignUp();
			signUpDialog.Show(transaction, "Dialog Fragment");
		}
        

        private async Task PostUser(string url, Person person)
        {
            System.Diagnostics.Debug.WriteLine("Username " + person.username);
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            string RestURL = "http://52.91.212.179:8800/user/";

            var uri = new Uri(string.Format(RestURL, person.username));

            var json = JsonConvert.SerializeObject(person);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;

            System.Diagnostics.Debug.WriteLine("Waiting for response");

            //response = client.PostAsync(uri, content);

            var task = client.PostAsync(uri, content);

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                System.Diagnostics.Debug.WriteLine(task.Status);

            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
            }


            System.Diagnostics.Debug.WriteLine("Done waiting for response");

            if (response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine(@"             TodoItem successfully saved.");

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Not Successful");
            }

        }

    }

    public class Person
    {
        public string username = "notChris";
        public string first_name = "Christopher";
        public string last_name = "Houze";
        public int positive_votes = 1;
        public int negative_votes = 1;
        public int current_lat = 12;
        public int current_long = 21;
        public string gender = "Male";
        public string bio = "This is a bio";

    }
}


