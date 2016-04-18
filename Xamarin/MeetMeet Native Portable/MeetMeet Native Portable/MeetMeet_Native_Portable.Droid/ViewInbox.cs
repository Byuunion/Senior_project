
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

namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "ViewInbox")]			
	public class ViewInbox : Activity
	{
		
		protected MsgListAdapter taskList;
		protected IList<Message> tasks;
		protected ListView taskListView = null;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.messaging);

			taskListView = (ListView)FindViewById (Resource.Id.listView);

			foreach (string s in MessageRepository.GetMessagedUsers()) {
				System.Diagnostics.Debug.WriteLine (s + "fdslkj");
			}

			System.Diagnostics.Debug.WriteLine ("finished with that");
			tasks = MessageRepository.GetUsersMessage(MessageRepository.GetMessagedUsers().ToArray<string>()).ToList<Message>();
			System.Diagnostics.Debug.WriteLine (tasks.Count);
			// create our adapter
			taskList = new MsgListAdapter(this, tasks);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;


			// wire up view msg click handler
			//user only needs to click on message to display pop up
			if(taskListView != null) {
				taskListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
				{
					int msgID = tasks[e.Position].Id;
					var taskDetails = new Intent (this, typeof (ViewMsg));
					taskDetails.PutExtra ("MsgIDin", tasks[e.Position].Id);
					StartActivity (taskDetails);
				};
			}
		}


		protected override void OnResume ()
		{
			base.OnResume ();

			//tasks = MessageRepository.GetUsersMessage(MessageRepository.GetMessagedUsers().ToArray<string>()).ToList<Message>();

			tasks = MessageRepository.GetMessages ().ToList();
			// create our adapter
			taskList = new MsgListAdapter(this, tasks);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}

	}
}

