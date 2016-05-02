using System;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class contains the username and token of the currently signed in user.
	/// This class also contains methods for signing up and loggin in
	/// </summary>
    public class Credentials
    {
        private string mUsername;
        public string username { get { return mUsername; } }
        private string mToken;
        public string token { get { return mToken; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="MeetMeet_Native_Portable.Credentials"/> class.
		/// </summary>
		/// <param name="username"> the username of the user currently using the app. </param>
        public Credentials(string username)
        {
            this.mUsername = username;
            this.mToken = default(String);
        }

		/// <summary>
		/// Attempts to log the user into the system and get a token to use for the
		/// duration of this session
		/// </summary>
		/// <returns>Whether or not the login was successful</returns>
		/// <param name="password">The user's password</param>
		/// <param name="url">The URL of the server</param>
        public async Task<Boolean> doLogin(string password, string url)
        {
			var resource = URLs.login_ext + "/" +  username + "/" + password;
            var tempToken = await LoginUpdater<Token>.LoginUpdate(new { username = username, password = password }, URLs.serverURL + URLs.login_ext + "/" + username);

            if (tempToken != default(Token))
            {
                mToken = tempToken.token;
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// Attempts to sign the user up for the service.
		/// </summary>
		/// <returns>Whether or not the sign up was successful</returns>
		/// <param name="password">The password that the user wishes to set their account to</param>
		/// <param name="url">The URL of the server</param>
        public async Task<Boolean> doSignUp(string password, string url)
        {
			if(await Poster.PostObject(new { username = this.username, password = password }, url + URLs.login_ext))
            {
                System.Diagnostics.Debug.WriteLine("Successfully signed up user: " + this.username);
                return await doLogin(password, url);
            }

            return false;
        }

    }

	/// <summary>
	/// Utility class, used to get the token from the server's response
	/// </summary>
    public class Token
    {
        public string token { get; set; }
    }

}