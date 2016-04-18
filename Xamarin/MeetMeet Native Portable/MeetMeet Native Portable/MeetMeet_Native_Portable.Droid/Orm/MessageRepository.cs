using System;
using System.Collections.Generic;


namespace MeetMeet_Native_Portable.Droid
{
				
	public class MessageRepository 
	{
		MessageDB db = null;

		protected static MessageRepository me;

		static MessageRepository ()
		{
			me = new MessageRepository();
		}
		protected MessageRepository()
		{
			db = new MessageDB(MessageDB.DatabaseFilePath);

			//insert default values for testing 
			Message msg1 = new Message();
			//0, "23:14:31 PM", "aa", "Here is the first message I sent to user aa");
			msg1.Id = 0;
			msg1.Date = "23:14:31 PM";
			msg1.UserName = "aa";
			msg1.MsgText = "Here is the first message I sent to user aa";
			Message msg2 = new Message();
			//1, "23:15:00 PM", "aa", "Here is the second message I sent to user aa");
			msg1.Id = 1;
			msg1.Date = "23:15:14 PM";
			msg1.UserName = "aa";
			msg1.MsgText = "Here is the second message I sent to user aa";
			db.SaveMessage (msg1);
			db.SaveMessage (msg2);

		}

		public static Message GetMessage(int id)
		{
			return me.db.GetMessage(id);
		}

		public static IEnumerable<Message> GetMessages ()
		{
			return me.db.GetMessages();
		}

		public static int SaveMessage (Message msg)
		{
			return me.db.SaveMessage(msg);
		}

		public static int DeleteMessage(Message msg)
		{
			return me.db.DeleteMessage(msg);
		}

	}
}
	

