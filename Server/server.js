var app = require('express')();
var mysql = require('mysql'); //import mysql package.
var bodyParser = require('body-parser');
var hat = require('hat');
var GOOGLE_API_KEY = 'AIzaSyAy3SrgunF-XyYESfgRfWjjIFiK8lajhaI';
var http = require('http');
var querystring = require('querystring');
var bcrypt = require('bcrypt');

//Config for database
var mysqlConfig = {
	host: 'userdb.cydfufqp5imu.us-east-1.rds.amazonaws.com',
	user: 'overlord',
	password: 'jaypeg55',
	port: '3306',
	database: 'userdb',
};

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

//This is the port for the general webapi
app.set('port', (process.env.PORT || 8800));

app.get('/user', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get Users.");
		else console.log("Error connecting database.");
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
});

app.get('/user/profile/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get User Profile.");
		else console.log("Error connecting database.");
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
});

app.get('/user/language/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get User Language.");
		else console.log("Error connecting database.");
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
});

app.get('/user/interest/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get User Interest.");
		else console.log("Error connecting database.");
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
});

/*
//Gets messages sent to username param
app.get('/user/message/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get User Message.");
		else console.log("Error connecting database.");
	});

	var response = {
		success: null,
		success_message: null,
		token: null
	};
    connection.query('SELECT * FROM user_message WHERE username_to = ' + connection.escape(req.params.username), function(err, data){
        if (err){
				response.success = false;
				response.success_message = "Failed to get messsages to: " + req.params.username + ".";
				res.json(response);
			
		}
        else res.json(data);
		
		connection.end();
    });
});
*/
//Pass in username and password from the app's url
app.get('/user/login/:username/:password', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get User Login.");
		else console.log("Error connecting database.");
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
});

app.post('/user/profile', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Post User Profile.");
		else console.log("Error connecting database.");
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
});

app.post('/user/interest', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Post User Interest.");
		else console.log("Error connecting database.");
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
});

app.post('/user/language', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.  Post User Language.");
		else console.log("Error connecting database.");
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
});

//Create account
app.post('/user/login', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
		if(!err) console.log("Database is connected. Post User Login.");
		else console.log("Error connecting database.");
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
});

app.post('/user/gcmregid', function(req, res) {
	var connection = mysql.createConnection(mysqlConfig);
	
	connection.connect(function(err){
        if(!err) console.log("Database is connected. Post User Gcmregid");
		else console.log("Error connecting database.");
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
});

//Updates user's gcmregid
app.put('/user/gcmregid', function(req, res) {
	var connection = mysql.createConnection(mysqlConfig);
	
	connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
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
});

//Token from sender needed
app.post('/user/message', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.  Post User Message.");
		else console.log("Error connecting database.");
	});
	
	var sender = req.body.username_from;

    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(sender), function(err, data){
		if (err || data.length === 0){
			response.success = false;
			response.success_message = "Failed to find existing token from: " + sender + ".";
			res.json(response);
		}
		else{
			var token = data[0].token;

			var response = {
				success: null,
				success_message: null
			};

			if(req.body.token === token){
				connection.query('SELECT gcm_regid FROM user_gcm WHERE username = ' + connection.escape(sender), function(err, gcm_user){
					if (err || data.length === 0){
						response.success = false;
						response.success_message = "Failed to find existing gcm_regid from: " + sender + ".";
						res.json(response);
					}
					else{
						var to = gcm_user[0].gcm_regid;

						var postData = JSON.stringify({
							'registration_ids': [ to ],
							'data': {
								'message_code': '1',
								'username_from': req.body.username_from,
								'message': req.body.message_text
							}
						});
						
						console.log('JSON string: ' + postData);

						var options = {
							hostname: 'gcm-http.googleapis.com',
							path: '/gcm/send',
							method: 'POST',
							headers: {
								'Content-Type': 'application/json',
								'Content-Length': postData.length,
								'Authorization': 'key=' + GOOGLE_API_KEY
							}
						};
						
						var myReq = http.request(options, function (myRes) {
							console.log('STATUS: ' + myRes.statusCode);
							
							console.log('HEADERS: ' + JSON.stringify(myRes.headers));
							myRes.setEncoding('utf8');
							myRes.on('data', function (chunk) {
								console.log('BODY: ' + chunk);
							});
							myRes.on('end', function (){
								console.log('No more data in response.')
							})
						});
						
						myReq.on('error', function (e) {
							console.log('problem with request: ' + e.message);
						});
						
						myReq.write(postData);
						myReq.end();

						response.success = true;
						response.success_message = 'something happened';	
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
});

//Posting an invite
app.post('/user/group/invite',function(req, res) {
	
	var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Post User Invite.");
		else console.log("Error connecting database.");
	});
    
    var username = req.body.username_from;
	
	var response = {
			success: null,
			success_message: null
		};
		
    if(username !== req.body.username_to){
		connection.query('SELECT username_from, username_to FROM user_invite WHERE username_from = ' + connection.escape(username) + ' AND username_to = ' + connection.escape(req.body.username_to), function(err, data1){
			if(data1.length === 0){ //Not yet invited
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
								username_from: username,
								username_to: req.body.username_to
							};
							
							connection.query('INSERT INTO user_invite SET ?', data, function(err, rows) {
								if (err){
									response.success = false;
									response.success_message = "Failed to post invite from: " + username + ".";
								}
								else{
									response.success = true;
									response.success_message = "Successfully created invite.";	
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
			else{ // Already invited by you
				response.success = false;
				response.success_message = "You have already invited that user";
				res.json(response);
				connection.end();
			}
		});
		
	}
	else{
		response.success = false;
		response.success_message = "Silly rabbit. You can't add invite yourself";
		res.json(response);
	}
	
});

//Responding to an invite
app.put('/user/group/invite' , function(req, res){
	
	// username_to's token
	var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Put User Invite.");
		else console.log("Error connecting database.");
	});
    
    var username = req.body.username_to;
    
    var response = {
		success: null,
		success_message: null
	};
    
	// Accept group invite
	if(req.body.response == true){
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				//res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.body.token === token){
					
					var data = {
						username_from: username,
						username_to: req.body.username_to
					};
					
					var sql = 'SELECT username_from, username_to FROM user_invite WHERE username_from = ? AND username_to = ?';
					var inserts = [req.body.username_from, username];
					sql = mysql.format(sql, inserts);
					//connection.query('SELECT username_from, username_to FROM user_invite WHERE username_from = ' + connection.escape(req.body.username_from) + ', username_to = ' + connection.escape(username), function(err, data){
					connection.query(sql, function(err, data){
						if(err || data.length === 0){
							response.success = false;
							response.success_message = "Invite does not exist";
							res.json(response);
						}
						else{
							connection.query('SELECT * FROM user_group WHERE username = ' + connection.escape(username), function(err, receiver_data) {
								if(err){
									response.success = false;
									response.success_message = "Error in query";
									res.json(response);
								}
								else{
									if(receiver_data.length !== 0){ // Receiver is already in a group 
										response.success = false;
										response.success_message = "Receiver is already in a group";
										res.json(response);
										
										var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
										var inserts = [req.body.username_from, username];
										sql = mysql.format(sql, inserts);
										
										connection.query(sql, function(err, data){	
											if(err){
												response.success = false;
												response.success_message = "Error inserting into group";
											}
											else{
												response.success = true;
											}
											connection.end();
										});
									}
									else{ // Receiver not in a group
										connection.query('SELECT * FROM user_group WHERE username = ' + connection.escape(req.body.username_from), function(err, sender_data) {
											if(err){
												response.success = false;
												response.success_message = "Error in query";
												res.json(response);
											}
											else{
												if(sender_data.length !== 0){ // Sender is in a group 
													// Add receiver to sender's group
													var groupData = {
														id: sender_data[0].id,
														username: username
													}
													connection.query('INSERT INTO user_group SET ?', groupData, function(err) {
														if(err){
															response.success = false;
															response.success_message = "Error inserting into group";
														}
														else{
															response.success = true;
															response.success_message = "Receiver added to sender's group";
														}
														res.json(response);
														
														var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
														var inserts = [req.body.username_from, username];
														sql = mysql.format(sql, inserts);
														
														connection.query(sql, function(err, data){	
															if(err){
																response.success = false;
																response.success_message = "Error inserting into group";
															}
															else{
																response.success = true;
															}
															connection.end();
														});
													});
												}
												else{ // Sender is not in a group
													// Create a new group and add sender to it.
													connection.query('INSERT INTO user_group (username) VALUES (' + connection.escape(req.body.username_from) + ')', function(err, groupData) {
													console.log(groupData);
														if(err){
															response.success = false;
															response.success_message = "Error inserting into group";
															res.json(response);
															connection.end();
														}
														else{ //Find group id of sender
															connection.query('SELECT id FROM user_group WHERE username = ' + connection.escape(req.body.username_from), function(err, data2) {
																// Add receiver to the same group
																connection.query('INSERT INTO user_group (id, username) VALUES (' + data2[0].id + ',' + connection.escape(username) + ')', function(err) {
																	if(err){
																		response.success = false;
																		response.success_message = "Error inserting into group";
																	}
																	else{
																		response.success = true;
																		response.success_message = "Created new group and added to the sender";
																	}
																	res.json(response);
																	// End of scenarios. Can delete the invite.
																	var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
																	var inserts = [req.body.username_from, username];
																	sql = mysql.format(sql, inserts);
																	
																	//connection.query('DELETE FROM user_invite WHERE username_from = ? , username_to = ?', [req.body.username_from, username], function(err, data){
																	connection.query(sql, function(err, data){	
																		if(err){
																			response.success = false;
																			response.success_message = "Error inserting into group";
																		}
																		else{
																			response.success = true;
																		}
																		connection.end();
																	});
																});
															});
														}
													});	
												}						
											}
										});
									}
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
	}
	else{ // Decline invite
		// Delete the invite
		var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
		var inserts = [req.body.username_from, username];
		sql = mysql.format(sql, inserts);
		//connection.query('DELETE FROM user_invite WHERE username_from = ? , username_to = ?', [req.body.username_from, username], function(err, data){
		connection.query(sql, function(err, data){	
			if(err || data.affectedRows == 0){
				response.success = false;
				response.success_message = "Error deleting from invites";
			}
			else{
				response.success = true;
				response.success_message = "User Declined Invite. Deleting Invite.";
			}
			res.json(response);
			connection.end();
		});
	}
});

app.delete('/user', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Delete User.");
		else console.log("Error connecting database.");
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
});

app.put('/user/profile/', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);
	
    connection.connect(function(err){
        if(!err) console.log("Database is connected. Update User Profile Location");
		else console.log("Error connecting database.");
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
});

app.put('/user/profile/location/:username', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);
	
    connection.connect(function(err){
        if(!err) console.log("Database is connected. Update User Profile Location");
		else console.log("Error connecting database.");
	});

	var username = req.param.username;

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
});

app.get('/user/:min_lat/:max_lat/:min_long/:max_long/:yourLat/:yourLong', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Get Geolocation Users");
		else console.log("Error connecting database.");
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
});



app.post('/user/group/message', function(req, res) {
	var connection = mysql.createConnection(mysqlConfig);
	
    connection.connect(function(err){
        if(!err) console.log("Database is connected.  Post Group Message.");
		else console.log("Error connecting database.");
	});
	
	var sender = req.body.username_from;

    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(sender), function(err, data){
		if (err || data.length === 0){
			response.success = false;
			response.success_message = "Failed to find existing token from: " + sender + ".";
			res.json(response);
		}
		else{
			var token = data[0].token;
			console.log('Got here');
			var response = {
				success: null,
				success_message: null
			};

			if(req.body.token === token){
				
				
				
				connection.query(	'SELECT gcm_regid ' +
									'FROM user_gcm ' + 
									'WHERE username IN (SELECT username ' + 
														'FROM user_group ' + 
														'WHERE id IN (	SELECT id ' +  
																		'FROM user_group ' + 
																		'WHERE username = ' + connection.escape(sender) + ') ' + 
																		'AND username <> ' + connection.escape(sender) + ')', function(err, gcm_users){
					if (err || data.length === 0){
						response.success = false;
						response.success_message = "Failed to find existing gcm_regid from: " + sender + ".";
						res.json(response);
					}
					
					else{
						var to = gcm_users.map(function(item){return item.gcm_regid;});
						console.log(to);
						var postData = JSON.stringify({
							'registration_ids': to,
							'data': {
								'message_code': '2',
								'username_from': req.body.username_from,
								'message': req.body.message_text
							}
						});
						
						console.log('JSON string: ' + postData);

						var options = {
							hostname: 'gcm-http.googleapis.com',
							path: '/gcm/send',
							method: 'POST',
							headers: {
								'Content-Type': 'application/json',
								'Content-Length': postData.length,
								'Authorization': 'key=' + GOOGLE_API_KEY
							}
						};
						
						var myReq = http.request(options, function (myRes) {
							console.log('STATUS: ' + myRes.statusCode);
							
							console.log('HEADERS: ' + JSON.stringify(myRes.headers));
							myRes.setEncoding('utf8');
							myRes.on('data', function (chunk) {
								console.log('BODY: ' + chunk);
							});
							myRes.on('end', function (){
								console.log('No more data in response.')
							})
						});
						
						myReq.on('error', function (e) {
							console.log('problem with request: ' + e.message);
						});
						
						myReq.write(postData);
						myReq.end();

						response.success = true;
						response.success_message = 'something happened';	
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
});


app.delete('/user/group', function(req, res) {
	var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected. Delete User from group.");
		else console.log("Error connecting database.");
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
				//Delete Cascades through tables
				connection.query('DELETE FROM user_group WHERE username = ' + connection.escape(username), function(err, data){
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
});

app.put('/user/pos_rating/:username', function(req, res) {
	var connection = mysql.createConnection(mysqlConfig);
	
	connection.connect(function(err){
        if(!err) console.log("Database is connected. Increase user rating.");
		else console.log("Error connecting database.");
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
});


app.put('/user/neg_rating/:username', function(req, res) {
	var connection = mysql.createConnection(mysqlConfig);
	
	connection.connect(function(err){
        if(!err) console.log("Database is connected. Decrease user rating.");
		else console.log("Error connecting database.");
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
});
	
app.listen(app.get('port'), function(){
   console.log("Listening on Port: " + app.get('port'));
});
