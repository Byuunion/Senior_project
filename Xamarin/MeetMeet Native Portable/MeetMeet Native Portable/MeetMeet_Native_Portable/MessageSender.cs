using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class provides functionality for sending all the different types of messages used by our application
	/// </summary>
    public class MessageSender
    {
		/// <summary>
		/// Send a message to a single user
		/// </summary>
		/// <returns>Whether or not the message was successful</returns>
		/// <param name="message">The message to send to the user</param>
		/// <param name="username_to">The user that the message is going to</param>
		/// <param name="this_user">The user that sent the message</param>
		/// <param name="url">http://52.91.212.179:8800/user/message</param>
        public static async Task<Boolean> SendSingleMessage(String message, string username_to, Credentials this_user, string url)
        {
            return await Poster.PostObject(new { username_to = username_to, username_from = this_user.username, token = this_user.token, message_code = "1", message_text = message}, url);
        }

		/// <summary>
		/// Sends a message to a group (if applicable)
		/// </summary>
		/// <returns>Whether or not the message was successful</returns>
		/// <param name="message">The message to send to the group</param>
		/// <param name="this_user">The user that sent the message</param>
		/// <param name="url"> http://52.91.212.179:8800/user/group/message</param>
        public static async Task<Boolean> SendGroupMessage(String message, Credentials this_user, string url)
        {
            return await Poster.PostObject(new { username_from = this_user.username, token = this_user.token, message_code = 3, message_text = message}, url);
        }

        /// <summary>
        /// Sends a message to a single user, with a special utility code
		/// This particular call is not used, but could be useful in the future
        /// </summary>
        /// <returns>Whether or not the message was successful</returns>
        /// <param name="username_to">The user that the message is going to</param>
        /// <param name="this_user">The user that sent the message</param>
		/// <param name="url">http://52.91.212.179:8800/user/message</param>
        /// <param name="message_code">An integer code representing some event</param>
        public static async Task<Boolean> SendUtilSingleMessage(string username_to, Credentials this_user, string url, string message_code)
        {
            return await Poster.PostObject(new { username_inviter = username_to, username_responder = this_user.username, token = this_user.token, message_code = message_code}, url);
        }

        /// <summary>
        /// Sends a group invite to a single user.
        /// </summary>
        /// <returns>Whether or not the invite was successfully sent</returns>
        /// <param name="username_to">The user that the invite is going to</param>
        /// <param name="this_user">The user that sent the invite</param>
		/// <param name="url"> http://52.91.212.179:8800/user/message/user/group/invite</param>
        public static async Task<Boolean> SendGroupInvite(string username_to, Credentials this_user, string url)
        {
			return await Poster.PostObject(new { username_inviter = this_user.username, username_responder = username_to, token = this_user.token, message_text = "You have been invited to a group", message_code = 2 }, url);
        }

        /// <summary>
        /// Send a response to a group invite
        /// </summary>
        /// <returns>Whether or not the invite was successful</returns>
        /// <param name="username_to">The user that invited this user</param>
		/// <param name="this_user">The user resonding to the invite (this user)</param>
		/// <param name="response"> should be true or false</param>
		/// <param name="url"> http://52.91.212.179:8800/user/message/user/group/invite</param>
        public static async Task<Boolean> RespondGroupInvite(string username_to, Credentials this_user, string url, string response)
        {
            return await Updater.UpdateObject(new { username_inviter = username_to, username_responder = this_user.username, token = this_user.token, response = response }, url);
        }

    }
}
