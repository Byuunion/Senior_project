using System;
using System.Collections.Generic;
using Android.Widget;
using Android.Views;

namespace MeetMeet_Native_Portable.Droid
{
	
    /// <summary>
    /// Adapter for user messages
    /// </summary>
	public class MsgListAdapter : BaseAdapter<Message> {
		ViewInbox context = null;
		IList<Message> msgs = new List<Message>();


		//the other text views and the delete button need to be in here
		//utilizing getitemid one can port over that info to generate the write message in the view msg pop up
		private ImageButton mButtonDeleteMsg;
		private TextView mTextViewDate;
        private TextView mTextViewMessage;
            
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">In this case ViewInbox</param>
        /// <param name="msgs">The list of messages between this user and another user</param>
		public MsgListAdapter (ViewInbox context, IList<Message> msgs) : base ()
		{
			this.context = context;
			this.msgs = msgs;
		}

        /// <summary>
        /// Get the message at the position
        /// </summary>
        /// <param name="position">The position to get the message at</param>
        /// <returns>The message to return</returns>
		public override Message this[int position]
		{
			get { return msgs[position]; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
		public override long GetItemId (int position)
		{
            return position;
		}

        /// <summary>
        /// Get the number of messages between the two users
        /// </summary>
        /// <returns>The number of messages between the two users</returns>
		public override int Count
		{
			get { return msgs.Count; }
		}

        /// <summary>
        /// Get the view for an item in the list
        /// </summary>
        /// <param name="position">The position of the item in the list</param>
        /// <param name="convertView">Unused</param>
        /// <param name="parent">The list view that this item is in</param>
        /// <returns>The view for the item at the given position</returns>
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			// Get our object for position
			var item = msgs[position];			

			// Inflate convertView from our item layout
			var view = context.LayoutInflater.Inflate (Resource.Layout.msg_adapter, parent, false); 

			mButtonDeleteMsg = view.FindViewById<ImageButton> (Resource.Id.delButton);
			mTextViewDate = view.FindViewById<TextView> (Resource.Id.dateView);
            mTextViewMessage = view.FindViewById<TextView>(Resource.Id.messageText);

            //Set the fields to diplay the message information
			mTextViewDate.Text = item.Date;
            mTextViewMessage.Text = item.MsgText;

            //Display the message in a different color depending on who sent it
            if (item.incoming)
            {
                mTextViewMessage.SetTextColor(Android.Graphics.Color.Cyan);
            }
            else
            {
                mTextViewMessage.SetTextColor(Android.Graphics.Color.Green);
            }

            
            //Delete button clicked
			mButtonDeleteMsg.Click += (object sender, EventArgs e) => {
                //Delete the message from the phone
				MessageRepository.DeleteMessage(item);
				Toast.MakeText(context, "message with: " + item.UserName + " deleted", ToastLength.Short).Show();

                //Delete the message from the screen
				msgs.Remove(item);
				context.removeMessage(position);
			};

			//Finally return the view
			return view;
		}
	}
}
