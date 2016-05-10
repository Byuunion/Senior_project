var express = require("express");
var router = express.Router();
var app = express();
var mysql = require('mysql');
var bodyParser = require('body-parser');
var hat = require('hat');
var http = require('http');
var bcrypt = require('bcrypt');

// middleware that is specific to this router
router.use(function timeLog(req, res, next) {
  console.log('Time: ', Date.now());
  next();
});

//Config for database
var mysqlConfig = {
	host: 'userdb.cydfufqp5imu.us-east-1.rds.amazonaws.com',
	user: 'overlord',
	password: 'jaypeg55',
	port: '3306',
	database: 'userdb',
};

//Get all the users in the database
router.route('/user')
	.get(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get Users.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		connection.query('SELECT * FROM user_profile', function(err, data) {
			if (err){
				var response;
				response.success = false;
				response.success_message = "This should never fail.";
				res.json(response);
			}
			else res.json(data);
			
			connection.end();
		});
	})

//Delete a user's account 
router.route('/user/:username/:token')	
	.delete(function(req,res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Delete User.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.params.username;
		
		var response = {
					success: null,
					success_message: null
		};
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.params.token === token){
					//Delete Cascades through tables
					connection.query('DELETE FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
						if (err){
							response.success = false;
							response.success_message = "Failed to delete user: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "User information deleted from Database";
							res.json(response);
						}
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})
	
//Get the specified user's profile information
router.route('/user/profile/:username')
	.get(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get User Profile.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		connection.query('SELECT * FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err){
				var response;
				response.success = false;
				response.success_message = "Failed to get profile of: " + req.params.username + ".";
				res.json(response);
			}
			else res.json(data);
			
			connection.end();
		});
	})

//Currently unused. Get any languages associated with this user
router.route('/user/language/:username')
	.get(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get User Language.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		connection.query('SELECT * FROM user_language WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err){
				var response;
				response.success = false;
				response.success_message = "Failed to get language(s) of: " + req.params.username + ".";
				res.json(response);
			}
			else res.json(data);
			
			connection.end();
		});
	})
	
//Currently unused. Get any interests associated with this user
router.route('/user/interest/:username')
	.get(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get User Interest.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		connection.query('SELECT * FROM user_interest WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err){
				var response;
				response.success = false;
				response.success_message = "Failed to get interest(s) of: " + req.params.username + ".";
				res.json(response);
			}
			else res.json(data);
			
			connection.end();
		});
	})

//Log a user in to the system. Create a new token and return it to them
router.route('/user/login/:username')
	.put(function(req, res){
		//Pass in username and password from the app's url
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Put User Login.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var response = {
			success: null,
			success_message: null,
			token: null
		};

		//Check the password
		connection.query('SELECT password_hash FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed password hash selection of user: " + req.params.username + ".";
				res.json(response);
				connection.end();
			}	
			else{
				//Hash the password and check it against the stored password hash
				var password = req.body.password;
				var hashedPassword = data[0].password_hash;
				bcrypt.compare(password, hashedPassword, function(err, match) {
					if (err){
						response.success = false;
						response.success_message = "Failed to compare password with hash.";
						res.json(response);
					}
					if(match){
						//Create a new token to be returned to the app
						var token = hat();
						response.success = true;
						response.success_message = "User successfully logged in: " + req.params.username;
						response.token = token;
						
						//Store the token to check in the future
						connection.query('UPDATE user_login SET token = ' + connection.escape(token) + ' WHERE username = ' + connection.escape(req.params.username), function(err, data){
							if (err){
								response.success = false;
								response.success_message = "Failed update token.";
								res.json(response);
							}
						});
						res.json(response);
					}
					else{
						response.success = false;
						response.success_message = "Username/Password pair not found";	
						res.json(response);
					}
					connection.end();
				});
			}
		});
	})

//Create a user profile entry in the database {username: , positive_votes: , negative_votes: , current_lat: , current_long: , gender: , bio: , token: }
router.route('/user/profile')
	.post(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Profile.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		
		var response = {
			success: null,
			success_message: null
		};
		
		//Check to make sure that this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;

				if(req.body.token === token){

					var data = {
						username: username,
						positive_votes: req.body.positive_votes,
						negative_votes: req.body.negative_votes,
						current_lat: req.body.current_lat,
						current_long: req.body.current_long,
						gender: req.body.gender,
						bio: req.body.bio
					};

					//Create an entry in the profile table
					connection.query('INSERT INTO user_profile SET ?', data, function(err) {
						if (err){
							response.success = false;
							response.success_message = "Failed to post profile of: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully created profile: " + username + ".";
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";	
					res.json(response);
				}
			}
			connection.end();
		});
	})
	
	//Update the profile entry to the values passed in {username: , xxx: , xx: , x: , token: } USERNAME AND TOKEN WILL NOT BE UPDATED BUT NEEDED
	.put(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Update User Profile Location.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		var response = {
					success: null,
					success_message: null
		};

		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
			
				var token = data[0].token;

				if(req.body.token === token){
					// Clones req.body without using a reference
					var data = JSON.parse(JSON.stringify(req.body));
					delete data.username;
					delete data.token;
					
					//Create the timestamp to be inserted into the database
					var date;
					date = new Date();
					date = date.getUTCFullYear() + '-' +
						('00' + (date.getUTCMonth()+1)).slice(-2) + '-' +
						('00' + date.getUTCDate()).slice(-2) + ' ' + 
						('00' + date.getUTCHours()).slice(-2) + ':' + 
						('00' + date.getUTCMinutes()).slice(-2) + ':' + 
						('00' + date.getUTCSeconds()).slice(-2);
					
					data.time = date;

					//Update the database with whatever information was passed in and the current time
					//The time is used when selecting nearby users, it allows us to avoid finding inactive users
					connection.query('UPDATE user_profile SET ? WHERE username = ' + connection.escape(username), data, function(err, data){
						if (err){
							response.success = false;
							response.success_message = "Failed to update profile of user: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully updates profile of user: " + username + ".";
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})

//Currently unused. Enter a new user interest
router.route('/user/interest')
	.post(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Interest.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		
		var response = {
			success: null,
			success_message: null
		};
		
		//Check to make sure that this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.body.token === token){
				
					var data = {
						username: username,
						interest: req.body.interest
					};
					
					//Create a new entry in the interests table
					connection.query('INSERT INTO user_interest SET ?', data, function(err, rows) {
						if (err){
							response.success = false;
							response.success_message = "Failed to post interest for: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully created interest.";	
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})

//Currently unused. Enter a new user language
router.route('/user/language')
	.post(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Language.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		
		var response = {
			success: null,
			success_message: null
		};
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.body.token === token){
				
					var data = {
						username: username,
						language: req.body.language
					};
					
					//Create a new entry in the language table
					connection.query('INSERT INTO user_language SET ?', data, function(err) {
						if (err){
							response.success = false;
							response.success_message = "Failed to post language for: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully added language.";
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})

//Account creation {username: , password: }
router.route('/user/login')
	.post(function(req, res){	
	
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Login.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		
		//Token to be used in place of the password for the current session
		var token = hat();
		var hashedPassword;
		
		const saltRounds = 10;
		
		//Hash the password through bcrypt
		bcrypt.hash(req.body.password, saltRounds, function(err, hash) {
			
			var data = {
				username: username,
				password_hash: hash,
				token: token
			};
			
			var response = {
				success: null,
				success_message: null,
				token: token
			};

			//Create an entry in the login table with the username, hashed password and token
			connection.query('INSERT INTO user_login SET ?', data, function(err, rows) {
				if (err){
					response.success = false;
					response.success_message = "Failed to create user: " + username + ".";
					response.token = null;
				}
				else{
					response.success = true;
					response.success_message = "Successfully created user: " + username + ".";
				}
				res.json(response);
				connection.end();
			});		
		});
	})

//Possibly unused, should use /user/profile (PUT) instead. Updates user profile's lat/long values {current_lat: , current_long: , token: }
router.route('/user/profile/location/:username')
	.put(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Update User Profile Location.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.params.username;
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err || data.length === 0){
				var response;
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
			
				var token = data[0].token;
			
				var response = {
					success: null,
					success_message: "User location updated"
				};

				if(req.body.token === token){
					var data = {
						current_lat: req.body.current_lat,
						current_long: req.body.current_long
					};
					
					//Update the latitude and longitude of the user
					connection.query('UPDATE user_profile SET ? WHERE username = ' + connection.escape(req.params.username), data, function(err, data){
						if (err){
							response.success = false;
							response.success_message = "Failed to update location of user: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully updates location of user: " + username + ".";
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})

//Log the user out of the system 
router.route('/user/login/:username/:token')
	.delete(function(req, res){	
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. User Logout.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.params.username;

		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err || data.length === 0){
				console.log(err);
				var response;
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
				connection.end();
			}
			else{
			
				var token = data[0].token;
			
				var response = {
					success: null,
					success_message: "User logout successful"
				};
			
				if(req.params.token === token){
					var data = {
						current_lat: "null",
						current_long: "null"
					};
					
					//Set the location of the user to null, to avoid them showing up in future matches
					connection.query('UPDATE user_profile SET current_lat = null, current_long = null WHERE username = ' + connection.escape(req.params.username), data, function(err, data){
						if (err){
							console.log(err);
							response.success = false;
							response.success_message = "Failed to remove user location: " + username + ".";
							res.json(response);
							connection.end();
						}
						else{
							//Remove the user's message id
							connection.query('UPDATE user_gcm SET gcm_regid = null WHERE username = ' + connection.escape(req.params.username), data, function(err, data1){
								if(err){
									console.log(err);
									response.success = false;
									response.success_message = "Failed to remove GCM token: " + username + ".";
									res.json(response);
									connection.end();
								}
								else{
									//Remove any outstanding invites from this user
									connection.query('DELETE FROM user_invite WHERE username_from = ' + connection.escape(req.params.username) + ' OR username_to = ' + connection.escape(req.params.username), data, function(err, data2){
										if(err){
											console.log(err);
											response.success = false;
											response.success_message = "Failed to remove user invites: " + username + ".";
											res.json(response);
											connection.end();
										}
										else{
											//Remove this user from any groups they are currently in
											connection.query('DELETE FROM user_group WHERE username = ' + connection.escape(req.params.username), data, function(err, data3){
												if(err){
													console.log(err);
													response.success = false;
													response.success_message = "Failed to remove from group: " + username + ".";
													res.json(response);
													connection.end();
												}
												else{
													//Remove the token from the database
													connection.query('UPDATE user_login SET token = null WHERE username = ' + connection.escape(req.params.username), data, function(err, data4){
														if(err){
															console.log(err);
															response.success = false;
															response.success_message = "Failed to remove token: " + username + ".";
															res.json(response);
															connection.end();
														}
														else{
															//Logout successful
															console.log("User successfully logged out: " + username + ".");
															response.success = true;
															response.success_message = "User successfully logged out: " + username + ".";
															res.json(response);
															connection.end();
														}
													});
												}
											});
										}
									});
								}
							});
						}
						
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
					connection.end();
				}
			}
			
		});
	})

//Get the users around the given location and within the given bounding box
router.route('/user/:min_lat/:max_lat/:min_long/:max_long/:yourLat/:yourLong')
	.get(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get Geolocation Users");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var radius = 5; 	// miles
		var timeout = 30; 	// minutes

		var response = {
			success: null,
			success_message: null
		};
		//Find all the active users around the given location 
		//Active is defined as having updated their location within the last 30 minutes
			connection.query('SELECT * ' +
								'FROM (SELECT * ' + 
										' FROM user_profile ' + 
										' WHERE (current_lat >= ' + connection.escape(req.params.min_lat) + 
										' AND current_lat <= ' + connection.escape(req.params.max_lat) + ')' + 
										' AND (current_long >= ' + connection.escape(req.params.min_long) + 
										' AND current_long <= ' + connection.escape(req.params.max_long) + '))' + 
								' AS reducedProfiles ' + 
								'WHERE 3959 * acos ' + 
								'( cos ( radians (' + connection.escape(req.params.yourLat) + ')) ' + 
								' * cos ( radians (current_lat)) ' + 
								' * cos ( radians (current_long) - radians( ' + connection.escape(req.params.yourLong) + ')) ' + 
								' + sin ( radians ( ' + connection.escape(req.params.yourLat) + ')) ' + 
								' * sin ( radians (current_lat)) ' + 
								') <= ' + connection.escape(radius) +   
								' AND TIMESTAMPDIFF(MINUTE, time, NOW()) < ' + connection.escape(timeout), function(err,data){
					if (err){
							response.success = false;
							response.success_message = "Failed to get nearby users.";
							console.log(err);
					}
					else{
							response.success = true;
							response.success_message = "Here are all of the users near lat: " + req.params.yourLat +  " long: " + req.params.yourLong + ".";
					}
					res.json(data);
			});
		connection.end();
	})

//Set the initial Google id for a user {username: , token: , gcm_regid: }
router.route('/user/gcmregid')
	.post(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Gcmregid.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var response = {
				success: null,
				success_message: null,
		};
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.body.username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.body.token === token){
					
					var data = {
						username: req.body.username, 
						gcm_regid: req.body.gcm_regid, 
					};

					//Create a new entry in the GCM id table with the username and gcm_regid
					connection.query('INSERT INTO user_gcm SET ?', data, function(err, rows) {
						if (err){
							response.success = false;
							response.success_message = "Failed to post gcm_regid for: " + username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully created user gcm";
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})
	
	//Update the Google id for a user {username: , token: , gcm_regid: }
	.put(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Put gcmregid.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.body.username), function(err, data){
			if (err) throw err;
			var token = data[0].token;
		
			var response = {
					success: null,
					success_message: null
			};
			
			if(req.body.token === token){
				
				var data = {
					username: req.body.username, 
					gcm_regid: req.body.gcm_regid, 
				};

				//Update the GCM id for the user
				connection.query('UPDATE user_gcm SET gcm_regid = ' + connection.escape(req.body.gcm_regid) + 'WHERE username = ' + connection.escape(req.body.username), data, function(err, rows) {
					if (err) {
						response.success = false;
						response.success_message = "Failed to update gcm_regid for: " + username + ".";
					}
					else{
						response.success = true;
						response.success_message = "Successfully updated user gcm";
						res.json(response);
						connection.end();
					}
				});
			}
			else{
				response.success = false;
				response.success_message = "Token didn't match";
				res.json(response);
				connection.end();
			}
		});
	})

//Send a positive rating to a user {rating_username: , token: }
router.route('/user/pos_rating/:username')
	.put(function(req, res){		
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Increase user rating.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var rating_user = req.body.rating_username;
		var rated_user = req.params.username;
		
		var response = {
					success: null,
					success_message: null
		};
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(rating_user), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				//Make sure that a user isn't trying to rate themself
				if(req.body.token === token && rating_user !== rated_user){
					
					//Increment the rated user's positive votes
					connection.query('UPDATE user_profile SET positive_votes = positive_votes + 1 WHERE username = ' + connection.escape(rated_user), function(err, data){
						if (err){
							response.success = false;
							response.success_message = "Failed add positive vote for: " + rated_user + ".";
						}
						else{
							response.success = true;
							response.success_message = "User rating increased";
							res.json(response);
						}
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})

//Send a negative rating to a user {rating_username: , token: }
router.route('/user/neg_rating/:username')
	.put(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Decrease user rating.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});

		var rating_user = req.body.rating_username;
		var rated_user = req.params.username;
		
		var response = {
					success: null,
					success_message: null
		};
		
		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(rating_user), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				//Make sure that a user isn't trying to rate themself
				if(req.body.token === token && rating_user !== rated_user){
					
					//Increment the rated user's negative votes
					connection.query('UPDATE user_profile SET negative_votes = negative_votes + 1 WHERE username = ' + connection.escape(rated_user), function(err, data){
						if (err){
							response.success = false;
							response.success_message = "Failed add negative vote for: " + rated_user + ".";
						}
						else{
							response.success = true;
							response.success_message = "User rating decreased";
							res.json(response);
						}
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		});
	})
	
//Block a user {username: , token: , block_username: }
router.route('/user/blacklist')
	.post(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Blacklist.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		
		var response = {
			success: null,
			success_message: null
		};
		
		//Check to make sure a user isn't trying to block themself
		if(username !== req.body.block_username){
			
			//Check to make sure this is a valid request
			connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
				if (err || data.length === 0){
					response.success = false;
					response.success_message = "Failed to find token from: " + username + ".";
					res.json(response);
				}
				else{
					var token = data[0].token;
					
					if(req.body.token === token){
						var data = {
							username: username,
							block_username: req.body.block_username
						};
						
						//Create a new entry in the blacklist table where the current user is blocking the user with block_username
						connection.query('INSERT INTO user_blacklist SET ?', data, function(err) {
							if (err){
								response.success = false;
								response.success_message = "Failed to block user: " + data.block_username + ".";
							}
							else{
								response.success = true;
								response.success_message = "Successfully blocked user: " + data.block_username + ".";
								
							}
							res.json(response);
						});
					}
					else{
						response.success = false;
						response.success_message = "Token didn't match";	
						res.json(response);
					}
				}
				connection.end();
			});
		}
		else{
			response.success = false;
			response.success_message = "Silly rabbit. You can't block yourself. What's wrong with you?";
			res.json(response);
		}	
	})

//Unblock a user
router.route('/user/blacklist/:username/:block_username/:token')
	.delete(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Blacklist.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.params.username;
		
		var response = {
			success: null,
			success_message: null
		};

		//Check to make sure this is a valid request
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
				var data = {
					username: username,
					block_username: req.params.block_username
				}
				if(req.params.token === token){
					//Delete the entry in the blacklist table where the current user is blocking the user with block_username
					connection.query('DELETE FROM user_blacklist WHERE username = ? AND block_username = ?', [username,req.params.block_username], function(err, data){
						if (err){
							response.success = false;
							response.success_message = "Failed to remove block on user: " + req.params.username + ".";
						}
						else{
							response.success = true;
							response.success_message = "Successfully removed black on user: " + req.params.block_username + ".";
						}
						res.json(response);
					});
				}
				else{
					response.success = false;
					response.success_message = "Token didn't match";
					res.json(response);
				}
			}
			connection.end();
		})
	})
	
module.exports = router; // HAS TO BE AT THE BOTTOM