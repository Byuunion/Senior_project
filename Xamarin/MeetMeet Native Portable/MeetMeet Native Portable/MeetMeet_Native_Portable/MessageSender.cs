﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
    public class MessageSender
    {
        /// <param name="url"></param> http://52.91.212.179:8800/user/message
        public static async Task<Boolean> SendSingleMessage(String message, string username_to, Credentials this_user, string url)
        {
            return await Poster.PostObject(new { username_to = username_to, username_from = this_user.username, token = this_user.token, message_code = "1", message_text = message}, url);
        }

        /// <param name="url"></param> http://52.91.212.179:8800/user/group/message
        public static async Task<Boolean> SendGroupMessage(String message, Credentials this_user, string url)
        {
            return await Poster.PostObject(new { username_from = this_user.username, token = this_user.token, message_code = "2", message_text = message}, url);
        }

        /// <param name="url"></param> http://52.91.212.179:8800/user/message
        public static async Task<Boolean> SendUtilSingleMessage(string username_to, Credentials this_user, string url, string message_code)
        {
            return await Poster.PostObject(new { username_inviter = username_to, username_responder = this_user.username, token = this_user.token, message_code = message_code}, url);
        }

        /// <param name="url"></param> http://52.91.212.179:8800/user/message/user/group/invite
        public static async Task<Boolean> SendGroupInvite(string username_to, Credentials this_user, string url)
        {
			return await Poster.PostObject(new { username_inviter = this_user.username, username_responder = username_to, token = this_user.token, message_text = "You have been invited to a group", message_code = 2 }, url);
        }

        /// <param name="response"></param> should be true or false
        /// <param name="url"></param> http://52.91.212.179:8800/user/message/user/group/invite
        public static async Task<Boolean> RespondGroupInvite(string username_to, Credentials this_user, string url, string response)
        {
            return await Updater.UpdateObject(new { username_inviter = username_to, username_responder = this_user.username, token = this_user.token, response = response }, url, "");
        }

    }
}
