
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// Inbox menu
    /// </summary>
	[Activity (Label = "ViewInbox")]			
	public class ViewInbox : Activity
	{
		
		protected MsgListAdapter taskList;
		protected IList<Message> tasks;
		protected ListView taskListView = null;

        /// <summary>
        /// When the activity is created
        /// </summary>
        /// <param name="bundle">Any data passed to the activity</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);


			MainActivity.references.Put ("Inbox", this);

			SetContentView(Resource.Layout.messaging);

			taskListView = (ListView)FindViewById (Resource.Id.listView);

			tasks = MessageRepository.GetUsersMessage(MessageRepository.GetMessagedUsers().ToArray<string>()).ToList<Message>();
			System.Diagnostics.Debug.WriteLine (tasks.Count);

			// create our adapter
			taskList = new MsgListAdapter(this, tasks);


			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;

			// wire up view msg click handler
			//user only needs to click on message to display pop up
			/*
			taskListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				System.Diagnostics.Debug.WriteLine("Clicked");
				int msgID = tasks[e.Position].Id;
				var taskDetails = new Intent (this, typeof (ViewMsg));
				taskDetails.PutExtra ("MsgIDin", tasks[e.Position].Id);
				StartActivity (taskDetails);
			};
			if(taskListView != null) {
				
			}*/
		}

		public void viewMessage(int position){
			/*System.Diagnostics.Debug.WriteLine("Clicked");
			int msgID = tasks[position].Id;
			var taskDetails = new Intent (this, typeof (ViewMsg));
			taskDetails.PutExtra ("MsgIDin", tasks[position].Id);
			StartActivity (taskDetails);*/

			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			ViewMsg viewMsgDialog = new ViewMsg (tasks[position].Id);
			viewMsgDialog.Show (transaction, "Dialog Fragment");

			// Subscribing to Signin Event

		}

		public void removeMessage(int position){
			//tasks.RemoveAt (position);
			taskListView.Adapter = taskList;
		}

		public void newMessage(Message m){
			tasks.Add (m);
			this.RunOnUiThread (() => {
				taskListView.Adapter = taskList;
			});

		}

        /// <summary>
        /// Executed if the activity is resumed after it is paused
        /// </summary>
		protected override void OnResume ()
		{
			base.OnResume ();

			tasks = MessageRepository.GetMessages ().ToList();
			// create our adapter
			taskList = new MsgListAdapter(this, tasks);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}

	}
}

