using System;
using System.Collections.Generic;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.App;
using Android;
using Newtonsoft.Json;

namespace MeetMeet_Native_Portable.Droid
{

    /// <summary>
    /// Adapter for user messages
    /// </summary>
    public class MsgListAdapterInbox : BaseAdapter<string>
    {
        ViewInbox context = null;
        IList<string> users = new List<string>();


        //the other text views and the delete button need to be in here
        //utilizing getitemid one can port over that info to generate the write message in the view msg pop up
        private TextView mTextUsername;
        private TextView mTextRecentMessage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msgs"></param>
		public MsgListAdapterInbox(ViewInbox context, IList<string> users) : base()
        {
            this.context = context;
            this.users = users;
        }

        public override string this[int position]
        {
            get { return users[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return users.Count; }
        }

        //this was the default method xamarin uses for list view.
        //this needs to be changed because my view needs to display username and timestamp
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get our object for position
            var item = users[position];
            System.Diagnostics.Debug.WriteLine(item);

            // Inflate convertView from our item layout
            var view = context.LayoutInflater.Inflate(Resource.Layout.msg_adapter_inbox, parent, false);
            view.Clickable = true;

            mTextUsername = view.FindViewById<TextView>(Resource.Id.textUsername);
            mTextRecentMessage = view.FindViewById<TextView>(Resource.Id.textRecentMessage);
           
            mTextRecentMessage.Text = MessageRepository.GetMostRecentMessageFrom(item).MsgText;
            mTextUsername.Text = MessageRepository.GetMostRecentMessageFrom(item).UserName;
            
            view.Click += (object sender, EventArgs e) => {
                context.viewConversation(position);
            };

            //Finally return the view
            return view;
        }

    }
}
