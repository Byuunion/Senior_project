using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
    public class MessageSender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="username_to"></param>
        /// <param name="this_user"></param>
        /// <param name="url"></param> http://52.91.212.179:8800/user/message
        /// <returns></returns>
        public async Task<Boolean> SendSingleMessage(String message, string username_to, Credentials this_user, string url)
        {
            return await Poster.PostObject(new { username_to = username_to, username_from = this_user.username, token = this_user.token, message_code = "1", message_text = message}, url);
        }

        public async Task<Boolean> SendGroupMessage(String message, Credentials this_user, string url)
        {
            return await Poster.PostObject(new { username_from = this_user.username, token = this_user.token, message_code = "2", message_text = message}, url);
        }

        public async Task<Boolean> SendUtilSingleMessage(String message, string username_to, Credentials this_user, string url, string message_code)
        {
            return await Poster.PostObject(new { username_to = username_to, username_from = this_user.username, token = this_user.token, message_code = message_code, message_text = message}, url);
        }

        
    }
}
