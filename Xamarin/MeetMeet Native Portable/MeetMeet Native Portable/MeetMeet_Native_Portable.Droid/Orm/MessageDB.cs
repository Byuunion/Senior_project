
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SQLite;

namespace MeetMeet_Native_Portable.Droid
{

	//db for message objects
	//a message contains an id(self increasing), a timestamp, a username, and the message itself

	public class MessageDB : SQLiteConnection
	{
		//MUST LOCK WHEN ACCESSING DB
		//protects from other threads accessing it at the same time
		static object locker = new object ();

		public static string DatabaseFilePath {
			get { 
				//name of DB stored on device
				var sqliteFilename = "MsgDB.db3";

				#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
				#else

				#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
				#else

				#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "../Library/"); // Library folder
				#endif
				var path = Path.Combine (libraryPath, sqliteFilename);
				#endif		

				#endif
				return path;	
			}
		}

		public MessageDB (string path) : base (path)
		{
			// create the tables
			CreateTable<Message> ();
		}
			
		public IEnumerable<Message> GetMessages () 
		{
			lock (locker) {
				return (from i in Table<Message> () select i).ToList ();
			}
		}

		public Message GetMessage (int id)
		{
			lock (locker) {
				return Table<Message>().FirstOrDefault(x => x.Id == id);
			}
		}

		public int SaveMessage (Message msg) 
		{
			lock (locker) {
				if (msg.Id != 0) {
					Update (msg);
					return msg.Id;
				} else {
					return Insert (msg);
				}
			}
		}

	
		public int DeleteMessage(Message msg) 
		{
			lock (locker) {
				return Delete<Message> (msg.Id);
			}
		}
	}
}