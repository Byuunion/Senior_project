
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SQLite;

namespace MeetMeet_Native_Portable.Droid
{

	//db for message objects
	//a message contains an id(self increasing), a timestamp, a username, and the message itself

        /// <summary>
        /// Database for holding user messages
        /// </summary>
	public class MessageDB : SQLiteConnection
	{
		//MUST LOCK WHEN ACCESSING DB
		//protects from other threads accessing it at the same time
		static object locker = new object ();

        /// <summary>
        /// The name of the file storing the database
        /// </summary>
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

        /// <summary>
        /// Constructor for the message table in the database
        /// </summary>
        /// <param name="path">The path to the database </param>
		public MessageDB (string path) : base (path)
		{
			CreateTable<Message> ();
		}
		
        /// <summary>
        /// Get all the messages stored in the database
        /// </summary>
        /// <returns> all the messages stored int the database</returns>
		public IEnumerable<Message> GetMessages () 
		{
			lock (locker) {
				return (from i in Table<Message> () select i).ToList ();
			}
		}

        /// <summary>
        /// Get a specific message from the database
        /// </summary>
        /// <param name="id">The id of the message to get</param>
        /// <returns>The message belonging to the given id</returns>
		public Message GetMessage (int id)
		{
			lock (locker) {
				return Table<Message>().FirstOrDefault(x => x.Id == id);
			}
		}

        /// <summary>
        /// Get all the messages from the specified usernames
        /// </summary>
        /// <param name="usernames">The usernames to find the messages for</param>
        /// <returns>All the messages to or from the given users</returns>
		public IEnumerable<Message> GetUsersMessage(string[] usernames){
            lock (locker)
            {
                return Query<Message> ("select * from Message where username = ?", usernames);
            }
			
		}

        /// <summary>
        /// Get all the unique usernames that appear in the database
        /// </summary>
        /// <returns>A list of unique usernames that this user has messaged</returns>
		public IEnumerable<string> GetMessagedUsers(){

            lock (locker)
            {
                //Select the all the usernames. The QueryString class is used because the Query function
                //requires a type with an empty constructor, which regular strings do not have
                var returns = Query<Message>("select distinct username from Message");

                List<string> strings = new List<string>();

                //Convert the data to regular strings
                foreach (Message qs in returns)
                {
                    if (qs.UserName != default(string))
                        System.Diagnostics.Debug.WriteLine(qs.UserName);
                    strings.Add(qs.UserName);
                }
                return strings;
            }
            
           
            
        }

        /// <summary>
        /// Save the given message to the database. First check to see if the generated ID is already in the DB, 
        /// if it is, call the update method and then adds the msg.
        /// </summary>
        /// <param name="msg">The message to add</param>
        /// <returns>The id of the message</returns>
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

	
        /// <summary>
        /// Delete the given message from the database
        /// </summary>
        /// <param name="msg">The message to delete</param>
        /// <returns></returns>
		public int DeleteMessage(Message msg) 
		{
			lock (locker) {
				return Delete<Message> (msg.Id);
			}
		}


        public Message GetMostRecentMessageFrom(string username)
        {
            lock (locker)
            {
                List<Message> messages = Query<Message>("select * from Message where UserName = ?", username);
                return messages.LastOrDefault();
            }
           
        }
	}

    /// <summary>
    /// This is a container class to allow the extraction of strings from the database
    /// </summary>
	public class QueryString{
		public string s;

		public QueryString(){
			//Do nothing
		}
	}
}