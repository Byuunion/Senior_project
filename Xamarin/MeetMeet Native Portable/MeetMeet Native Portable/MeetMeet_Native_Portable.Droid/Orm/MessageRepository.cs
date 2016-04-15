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
	

