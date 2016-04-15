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


router.route('/user/login/:username/:password')
	.get(function(req, res){
		//Pass in username and password from the app's url
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get User Login.");
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

		connection.query('SELECT password_hash FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed password hash selection of user: " + req.params.username + ".";
				res.json(response);
				connection.end();
			}	
			else{
				var password = req.params.password;
				var hashedPassword = data[0].password_hash;
				bcrypt.compare(password, hashedPassword, function(err, match) {
					if (err){
						response.success = false;
						response.success_message = "Failed to compare password with hash.";
						res.json(response);
					}
					if(match){
						var token = hat();
						response.success = true;
						response.success_message = "User successfully logged in: " + req.params.username;
						response.token = token;
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

	
router.route('/user/login')
	.post(function(req, res){	
		//Create account
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Login.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username;
		var token = hat();
		var hashedPassword;
		const saltRounds = 10;
		
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
					
					connection.query('UPDATE user_profile SET current_lat = null, current_long = null WHERE username = ' + connection.escape(req.params.username), data, function(err, data){
						if (err){
							console.log(err);
							response.success = false;
							response.success_message = "Failed to remove user location: " + username + ".";
							res.json(response);
							connection.end();
						}
						else{
							connection.query('DELETE FROM user_gcm WHERE username = ' + connection.escape(req.params.username), data, function(err, data1){
								if(err){
									console.log(err);
									response.success = false;
									response.success_message = "Failed to remove GCM token: " + username + ".";
									res.json(response);
									connection.end();
								}
								else{
									connection.query('DELETE FROM user_invite WHERE username_from = ' + connection.escape(req.params.username) + ' OR username_to = ' + connection.escape(req.params.username), data, function(err, data2){
										if(err){
											console.log(err);
											response.success = false;
											response.success_message = "Failed to remove user invites: " + username + ".";
											res.json(response);
											connection.end();
										}
										else{
											connection.query('DELETE FROM user_group WHERE username = ' + connection.escape(req.params.username), data, function(err, data3){
												if(err){
													console.log(err);
													response.success = false;
													response.success_message = "Failed to remove from group: " + username + ".";
													res.json(response);
													connection.end();
												}
												else{
													connection.query('UPDATE user_login SET token = null WHERE username = ' + connection.escape(req.params.username), data, function(err, data4){
														if(err){
															console.log(err);
															response.success = false;
															response.success_message = "Failed to remove token: " + username + ".";
															res.json(response);
															connection.end();
														}
														else{
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

				
router.route('/user/:min_lat/:max_lat/:min_long/:max_long/:yourLat/:yourLong')
	.get(function(req, res){
		console.log("Attempting to find users around " + req.params.yourLat + " " + req.params.yourLong );
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Get Geolocation Users");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var radius = 0.00126291; // 5 miles

		var response = {
								success: null,
								success_message: null
		};
		
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
								') <= 5 ', function(err,data){
					if (err){
							response.success = false;
							response.success_message = "Failed to get nearby users.";
					}
					else{
							response.success = true;
							response.success_message = "Here are all of the users near lat: " + req.params.yourLat +  " long: " + req.params.yourLong + ".";
					}
					res.json(data);
			});
		connection.end();
	})

	
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

					connection.query('INSERT INTO user_gcm SET ?', data, function(err, rows) {
						if (err){
							response.success = false;
							response.success_message = "Failed to post interest for: " + username + ".";
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
	
	.put(function(req, res){
		//Updates user's gcmregid
		var connection = mysql.createConnection(mysqlConfig);

		connection.connect(function(err){
			if(!err) console.log("Database is connected. Put gcmregid.");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
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

				connection.query('UPDATE user_gcm SET gcm_regid = ' + connection.escape(req.body.gcm_regid) + 'WHERE username = ' + connection.escape(req.body.username), data, function(err, rows) {
					if (err) throw err;

					response.success = true;
					response.success_message = "Successfully created user gcm";
					res.json(response);
					connection.end();
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
		
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(rating_user), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.body.token === token){
					
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
		
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(rating_user), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.body.token === token){
					
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
		
		if(username !== req.body.block_username){
			console.log("trying to block user " + req.body.block_username);
			connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
				if (err || data.length === 0){
					response.success = false;
					response.success_message = "Failed to find token from: " + username + ".";
					res.json(response);
				}
				else{
					var token = data[0].token;
					
					if(req.body.token === token){
						console.log("tokens matched " + req.body.block_username);
						var data = {
							username: username,
							block_username: req.body.block_username
						};
						console.log("attempting to insert " + data.username + " " + data.block_username);
						connection.query('INSERT INTO user_blacklist SET ?', data, function(err) {
							if (err){
								console.log("errored attempting to insert user");
								response.success = false;
								console.log("errored attempting to insert user");
								response.success_message = "Failed to block user: " + data.block_username + ".";
								console.log("errored attempting to insert user");
							}
							else{
								console.log("successfully inserted user");
								response.success = true;
								response.success_message = "Successfully blocked user: " + data.block_username + ".";
								
							}
							console.log("finished insert attemp " + data.username + " " + data.block_username);
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