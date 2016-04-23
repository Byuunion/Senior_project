using System;
using System.Collections.Generic;


namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageRepository
    {
        MessageDB db = null;

        protected static MessageRepository me;

        /// <summary>
        /// Static constructor
        /// </summary>
        static MessageRepository()
        {
            me = new MessageRepository();
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        protected MessageRepository()
        {
            db = new MessageDB(MessageDB.DatabaseFilePath);
        }

        /// <summary>
        /// Get all the unique usernames that appear in the database
        /// </summary>
        /// <returns>A list of unique usernames that this user has messaged</returns>
        public static IEnumerable<string> GetMessagedUsers()
        {
            return me.db.GetMessagedUsers();
        }

        /// <summary>
        /// Get a specific message from the database
        /// </summary>
        /// <param name="id">The id of the message to get</param>
        /// <returns>The message belonging to the given id</returns>
        public static Message GetMessage(int id)
        {
            return me.db.GetMessage(id);
        }

        /// <summary>
        /// Get all the messages from the specified usernames
        /// </summary>
        /// <param name="usernames">The usernames to find the messages for</param>
        /// <returns>All the messages to or from the given users</returns>
        public static IEnumerable<Message> GetUsersMessage(string[] usernames)
        {
            return me.db.GetUsersMessage(usernames);
        }

        /// <summary>
        /// Get all the messages stored in the database
        /// </summary>
        /// <returns> all the messages stored int the database</returns>
        public static IEnumerable<Message> GetMessages()
        {
            return me.db.GetMessages();
        }

        /// <summary>
        /// Save the given message to the database. First check to see if the generated ID is already in the DB, 
        /// if it is, call the update method and then adds the msg.
        /// </summary>
        /// <param name="msg">The message to add</param>
        /// <returns>The id of the message</returns>
        public static int SaveMessage(Message msg)
        {
            return me.db.SaveMessage(msg);
        }

        /// <summary>
        /// Delete the given message from the database
        /// </summary>
        /// <param name="msg">The message to delete</param>
        /// <returns></returns>
        public static int DeleteMessage(Message msg)
        {
            return me.db.DeleteMessage(msg);
        }

    }
}


