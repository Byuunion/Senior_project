using System;

namespace MeetMeet_Native_Portable 
{
	public class Profile : Postable
	{
		public string username;
		public string first_name;
		public string last_name;
		public int positive_votes;
		public int negative_votes;
		public double current_lat;
		public double current_long;
		public int age;
		public string gender;
		public string bio;


		public Profile(string username, string first_name, string last_name, string gender, int age){
			this.username = username;
			this.first_name = first_name;
			this.last_name = last_name;
			this.gender = gender;
			this.age = age;
			this.positive_votes = 0;
			this.negative_votes = 0;
			this.current_lat = 0;
			this.current_long = 0;
			this.bio = "User has not set up a bio yet.";
		}

		public string getName(){
			return this.username;
		}

		public void setName(string username){
			this.username = username;
		}

		public string getFirstName(){
			return this.first_name;
		}

		public void setFirstName(string first_name){
			this.first_name = first_name;
		}

		public string getLastName(){
			return this.last_name;
		}

		public void setLastName(string last_name){
			this.last_name = last_name;
		}

		public string getGender(){
			return this.gender;
		}

		public void setGender(string gender){
			this.gender = gender;
		}

		public int getAge(){
			return age;
		}

		public void setAge(int age){
			this.age = age;
		}

		public string getBio(){
			return this.bio;
		}

		public void setBio(string bio){
			this.bio = bio;
		}

		public int getPosVotes(){
			return this.positive_votes;
		}

		public void setPosVotes(int positive_votes){
			this.positive_votes = positive_votes;
		}

		public int getNegVotes(){
			return this.negative_votes;
		}

		public void setNegVotes(int negative_votes){
			this.negative_votes = negative_votes;
		}

		public double getCurrLong(){
			return this.current_long;
		}

		public void setCurrLong(double current_long){
			this.current_long = current_long;
		}

		public double getCurrLat(){
			return this.current_lat;
		}

		public void setCurrLat(double current_lat){
			this.current_lat = current_lat;
		}
	}
}