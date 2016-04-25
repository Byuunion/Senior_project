
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// Inbox menu
    /// </summary>
	[Activity (Label = "ViewInbox")]			
	public class ViewInbox : Activity
	{

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

			SetContentView(Resource.Layout.messaging_inbox);

			taskListView = (ListView)FindViewById (Resource.Id.listView);
            
            users = MessageRepository.GetMessagedUsers().ToArray<string>();
            tasks = new List<Message>();

            //Get the most recent message from each user to display in the "main" messaging list
            foreach (string s in users)
            {
                tasks.Add(MessageRepository.GetMostRecentMessageFrom(s));
            }
            
            // create our adapter
            taskList = new MsgListAdapterInbox(this, users.ToList());

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}

        /// <summary>
        /// Open the conversation window
        /// </summary>
        /// <param name="position">The position of the user in the list of usernames that the conversation is with</param>
        public void viewConversation(int position)
        {
            //Get the elements on the screen
            SetContentView(Resource.Layout.messaging);
            Button mButton = FindViewById<Button>(Resource.Id.btnSendMsg);
            EditText mText = FindViewById<EditText>(Resource.Id.sendMessageTxt);
            taskListView = (ListView)FindViewById(Resource.Id.listView);

            //Get the messages in the conversation
            msgs = MessageRepository.GetUsersMessage(new string[] { users[position] }).ToList();

            //Set the adapter to use messages
            taskList = new MsgListAdapter(this, msgs);
            taskListView.Adapter = taskList;
            
            //When the send button is clicked
            mButton.Click += async (object sender, EventArgs e) =>
            {
                if (!mText.Text.Equals(""))
                {
                    //If the message is sent successfully, store it and display it in the list
                    if(await MessageSender.SendSingleMessage(mText.Text, users[position], MainActivity.credentials, MainActivity.serverURL + MainActivity.single_message))
                    {
                        Message m = new Message();
                        m.MsgText = mText.Text;
                        m.UserName = users[position];
                        m.Date = DateTime.Now.ToString();
                        m.incoming = false;

                        newMessage(m);
                        MessageRepository.SaveMessage(m);
                        mText.Text = "";
                    }
                    else
                    {
                        Toast.MakeText(this, "Sending message was unsuccessful", ToastLength.Short).Show();
                    }   
                }
            };
        }

        /// <summary>
        /// Remove the message from the list on the screen
        /// </summary>
        /// <param name="position">The position of the message to remove</param>
		public void removeMessage(int position){
			taskListView.Adapter = taskList;
		}

        /// <summary>
        /// Add a new message to the list on the screen
        /// </summary>
        /// <param name="m">The message to add</param>
		public void newMessage(Message m){
			msgs.Add (m);
			RunOnUiThread (() => {
				taskListView.Adapter = taskList;
			});
		}

        /// <summary>
        /// Executed if the activity is resumed after it is paused
        /// </summary>
		protected override void OnResume ()
		{
			base.OnResume ();

            // create our adapter
            taskList = new MsgListAdapterInbox(this, users);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}

	}
}

