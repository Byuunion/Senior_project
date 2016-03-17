using System;

namespace MeetMeet_Native_Portable.Droid
{
	public class DeveloperAuthenticatedCredentials : CognitoAWSCredentials
	{
		const string PROVIDER_NAME = "login.rowan.meetmeet";
		const string IDENTITY_POOL = "us-east-1:9fc7967a-222c-4b86-8c09-36ca25e76a72";
		static readonly RegionEndpoint REGION = RegionEndpoint.USEast1;
		private string login = null;

		public DeveloperAuthenticatedCredentials(string loginAlias)
			: base(IDENTITY_POOL, REGION)
		{
			login = loginAlias;
		}

		protected override async Task<IdentityState> RefreshIdentityAsync()
		{
			IdentityState state = null;
			//get your identity and set the state
			return state;
		}
	}
}

