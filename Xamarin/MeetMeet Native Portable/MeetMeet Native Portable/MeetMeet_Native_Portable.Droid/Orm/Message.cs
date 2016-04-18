
using SQLite;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace MeetMeet_Native_Portable.Droid
{			
	public class Message 
	{
	[PrimaryKey, AutoIncrement, Column("_id")]
	public int Id { get; set; }
	
	public string Date { get; set;}

	public string UserName { get; set; }
	
	public string MsgText { get; set; }

		//String date = DateTime.Now.ToString;
		//String date = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
	}
}

