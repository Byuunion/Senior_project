
using SQLite;

using System;


namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// Container class for user messages in the database
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Message ID for use in the database
        /// </summary>
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        /// <summary>
        /// The date that the message was received or sent by this user
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// The username of the person that this message is from or is going to
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The content of the message
        /// </summary>
        public string MsgText { get; set; }

        /// <summary>
        /// True if the message was received by the currently logged in user, false otherwise
        /// </summary>
        public Boolean incoming { get; set; }
    }
}

