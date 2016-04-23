
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

        //protected MsgListAdapter taskList;
        protected BaseAdapter taskList;
		protected IList<Message> tasks;
		protected ListView taskListView = null;
        protected IList<string> users;
        IList<Message> msgs;

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

            //tasks = MessageRepository.GetUsersMessage(MessageRepository.GetMessagedUsers().ToArray<string>()).ToList<Message>();
            //System.Diagnostics.Debug.WriteLine (tasks.Count);
            
            users = MessageRepository.GetMessagedUsers().ToArray<string>();
            tasks = new List<Message>();
            foreach (string s in users)
            {
                tasks.Add(MessageRepository.GetMostRecentMessageFrom(s));
            }
            
            // create our adapter
            //taskList = new MsgListAdapter(this, tasks);
            taskList = new MsgListAdapterInbox(this, users.ToList());


			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}

		public void viewMessage(int position){
			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			ViewMsg viewMsgDialog = new ViewMsg (msgs[position].Id);
			viewMsgDialog.Show (transaction, "Dialog Fragment");
		}

        public void viewConversation(int position)
        {
           msgs = MessageRepository.GetUsersMessage(new string[] { users[position] }).ToList() ;
            taskList = new MsgListAdapter(this, msgs);
            taskListView.Adapter = taskList;
        }

		public void removeMessage(int position){
			taskListView.Adapter = taskList;
		}

		public void newMessage(Message m){
			msgs.Add (m);
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

            //tasks = MessageRepository.GetMessages ().ToList();
            // create our adapter
            //taskList = new MsgListAdapter(this, tasks);
            taskList = new MsgListAdapterInbox(this, users);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}

	}
}

