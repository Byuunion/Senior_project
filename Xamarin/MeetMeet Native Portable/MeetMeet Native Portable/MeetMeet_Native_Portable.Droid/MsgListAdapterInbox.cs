using System;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;

namespace MeetMeet_Native_Portable.Droid
{

    /// <summary>
    /// Adapter for the user's conversations
    /// </summary>
    public class MsgListAdapterInbox : BaseAdapter<string>
    {
        ViewInbox context = null;
        IList<string> users = new List<string>();

        //The gui components that need to be modified
        private TextView mTextUsername;
        private TextView mTextRecentMessage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">In this case ViewInbox</param>
        /// <param name="msgs">The list of users that this user has had a conversation with</param>
		public MsgListAdapterInbox(ViewInbox context, IList<string> users) : base()
        {
            this.context = context;
            this.users = users;
        }

        /// <summary>
        /// Get the message at the position
        /// </summary>
        /// <param name="position">The position to get the message at</param>
        /// <returns>The username to return</returns>
        public override string this[int position]
        {
            get { return users[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Get the number of users this user has had conversations with
        /// </summary>
        /// <returns>The number of conversations this user has had</returns>
        public override int Count
        {
            get { return users.Count; }
        }

        /// <summary>
        /// Get the view for an item in the list
        /// </summary>
        /// <param name="position">The position of the item in the list</param>
        /// <param name="convertView">Unused</param>
        /// <param name="parent">The list view that this item is in</param>
        /// <returns>The view for the item at the given position</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get our object for position
            var item = users[position];

            // Inflate convertView from our item layout
            var view = context.LayoutInflater.Inflate(Resource.Layout.msg_adapter_inbox, parent, false);
            view.Clickable = true;

            //Find and set the gui components
            mTextUsername = view.FindViewById<TextView>(Resource.Id.textUsername);
            mTextRecentMessage = view.FindViewById<TextView>(Resource.Id.textRecentMessage);
           
            mTextRecentMessage.Text = MessageRepository.GetMostRecentMessageFrom(item).MsgText;
            mTextUsername.Text = MessageRepository.GetMostRecentMessageFrom(item).UserName;
            
            //Bring up the message list for the select user
            view.Click += (object sender, EventArgs e) => {
                context.viewConversation(position);
            };

            //Return the created view
            return view;
        }

    }
}
