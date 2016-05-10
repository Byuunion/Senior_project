var express = require("express");
var router = express.Router();
var app = express();
var mysql = require('mysql');
var bodyParser = require('body-parser');
var GOOGLE_API_KEY = 'AIzaSyAy3SrgunF-XyYESfgRfWjjIFiK8lajhaI';
var http = require('http');

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

//Send message to a single user {username_to: , username_from: , message_text: , token: , message_code: }
router.route('/user/message')
	.post(function(req, res) {
		//Token from sender needed
		var connection = mysql.createConnection(mysqlConfig);
		
		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Message");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var sender = req.body.username_from;
		var response = {
			success: null,
			success_message: null
		};

		//Check to make sure that the token given is valid for the user
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(sender), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + sender + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
				
				if(req.body.token === token){
					//Call the message sending function
					send_single_message(req.body.username_from, req.body.username_to, req.body.message_text, req.body.message_code, function(results) {
						res.json(results);
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

//Send message to the user's current group {username_from: , token: , message: , message_code: }
router.route('/user/group/message')
	.post(function(req, res) {
	
		var connection = mysql.createConnection(mysqlConfig);
		
		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post Group Message");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var sender = req.body.username_from;
		var response = {
			success: null,
			success_message: null
		};

		//Check to make sure that the token given is valid for the user
		connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(sender), function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + sender + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
				
				if(req.body.token === token){
					
					//Get all the GCM ids of the people in the sender's group
					connection.query(	'SELECT gcm_regid ' +
										'FROM user_gcm ' + 
										'WHERE username IN (SELECT username ' + 
															'FROM user_group ' + 
															'WHERE id IN (	SELECT id ' +  
																			'FROM user_group ' + 
																			'WHERE username = ' + connection.escape(sender) + ') ' + 
															'AND username <> ' + connection.escape(sender) + ')', function(err, gcm_users){
						if (err || gcm_users.length === 0){
							response.success = false;
							response.success_message = "Failed to find gcm_regid for group of: " + sender + ".";
							res.json(response);
						}
						
						else{
							//Create the reqest for Google's server
							var to = gcm_users.map(function(item){return item.gcm_regid;});
							var postData = JSON.stringify({
								'registration_ids': to,
								'data': {
									'message_code': req.body.message_code,
									'username_from': req.body.username_from,
									'message': req.body.message_text
								}
							});

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
							response.success_message = 'Message Sent';	
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
	
//Remove the user from their current group
router.route('/user/group/:username/:token')
	.delete(function(req, res){
		var connection = mysql.createConnection(mysqlConfig);
		connection.connect(function(err){
			if(!err) console.log("Database is connected. Delete User from group.");
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
		
		//Check to make sure that the token given is valid for the user
		var token_sql = 'SELECT token FROM user_login WHERE username = ' + connection.escape(username);
		connection.query(token_sql, function(err, data){
			if (err || data.length === 0){
				response.success = false;
				response.success_message = "Failed to find existing token from: " + username + ".";
				res.json(response);
			}
			else{
				var token = data[0].token;
			
				if(req.params.token === token){
					
					//Find the size of the user's group
					var sql = 'SELECT count(*) as group_size FROM user_group WHERE id IN (SELECT id FROM user_group WHERE username = ' + connection.escape(username) + ')';
					connection.query(sql, function(err, data1){
						if(err || data1.length === 0){
							response.success = false;
							response.success_message = "Failed to find group of: " + username + ".";
							res.json(response);
							connection.end();
						}
						else{
							//Check the size, if the group has two or less members, just delete the group
							if(data1[0].group_size <= 2 && data1[0].group_size > 0){
								connection.query('SELECT id FROM user_group WHERE username = ' + connection.escape(username) + '', function(err, db_id){
									if(err || db_id.length === 0){
										response.success = false;
										response.success_message = "Failed to find id to delete group for user: " + username + ".";
										res.json(response);
										connection.end();
									}
									else{
										//Find the name of the other user in the group
										var other_username_sql = 'SELECT username FROM user_group WHERE id = ' + db_id[0].id + ' AND username <> ' + connection.escape(username);
										connection.query(other_username_sql, function(err, other_username){
											if(err || other_username.length === 0){
												response.success = false;
												response.success_message = "Could not find the name of the other user in the group";
												res.json(response);
												connection.end();
											}
											else{
												//Delete the group
												connection.query('DELETE FROM user_group WHERE id = ' + db_id[0].id, function(err, data){
													if (err){
														console.log(err);
														response.success = false;
														response.success_message = "Failed to delete group for user: " + username + data2[0].id + ".";
														res.json(response);
														connection.end();
													}
													else{
														response.success = true;
														response.success_message = "User group deleted from database";
														//Send message to the other user in the group, notifying them that the group was disbanded
														console.log(other_username[0].username);
														send_single_message("MeetMeet", other_username[0].username, " all other members have left the group, group is disbanded", "1", function(results) {
															response = results
															console.log(response);
															res.json(response);
															connection.end();
														});
													}
												});
											}
										});
										
									}
								});
								
							}
							else{
								//If the group is larger than two, just remove the current user
								console.log(data[0].group_size);
								if(data1[0].group_size != 0){
									connection.query('DELETE FROM user_group WHERE username = ' + connection.escape(username), function(err, data){
										if (err){
											response.success = false;
											response.success_message = "Failed to delete user: " + username + ".";
											res.json(response);
											connection.end();
										}
										else{
											response.success = true;
											response.success_message = "User information deleted from Database";
											res.json(response);
											connection.end();
										}
									});
								}
								else{
									response.success = false;
									response.success_message = "User not in database: " + username + ".";
									res.json(response);
									connection.end();
								}
							}
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
	
//Send a group invite to another user {username_from: , token: , message: , message_code: }
router.route('/user/group/invite')
	.post(function(req, res) {
		//Posting an invite
		var connection = mysql.createConnection(mysqlConfig);
		
		connection.connect(function(err){
			if(!err) console.log("Database is connected. Post User Invite");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username_inviter;
		
		var response = {
			success: null,
			success_message: null
		};
		
		//Check to make sure that a user isn't inviting themselves
		if(username !== req.body.username_responder){
			
			console.log("sending invite to " + req.body.username_responder);
			
			//Check to make sure that the invite doesn't already exist
			connection.query('SELECT username_from, username_to ' + 
							 'FROM user_invite ' + 
							 'WHERE username_from = ' + connection.escape(username) + 
							' AND username_to = ' + connection.escape(req.body.username_responder), function(err, data1){
				if(data1.length === 0){ 
				
					//Check to make sure that the token given is valid for the user
					connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
						if (err || data.length === 0){
							response.success = false;
							response.success_message = "Failed to find existing token from: " + username + ".";
							res.json(response);
						}
						else{
							var token = data[0].token;
							console.log("sending invite to " + req.body.username_responder);
							if(req.body.token === token){
								var data = {
									username_from: username,
									username_to: req.body.username_responder
								};
								
								//Send the invite and save it for if/when the invitee responds
								connection.query('INSERT INTO user_invite SET ?', data, function(err, rows) {
									if (err){
										response.success_message = "Failed to post invite from: " + data.username + " to " + data.username_to + ".";
									}
									else{
										//Send an invite message to the invited user
										send_single_message(req.body.username_inviter, req.body.username_responder, req.body.message_text, req.body.message_code, function(results) {
											response = results
											console.log(response);
											res.json(response);
										});
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
				}
				else{ 
					// The invitation already exists
					response.success = true;
					response.success_message = "Reinviting user";
					send_single_message(req.body.username_inviter, req.body.username_responder, req.body.message_text, req.body.message_code, function(results) {
						response = results
						console.log(response);
						res.json(response);
					});
					connection.end();
				}
			});
			
		}
		else{
			response.success = false;
			response.success_message = "Silly rabbit. You can't add invite yourself";
			res.json(response);
		}	
	})
	
	//Responding to a group invitation {username_inviter: , username_responder: , token: , response: } 
	.put(function(req, res) {
		//Responding to an invite
		//Username_to's token
		var connection = mysql.createConnection(mysqlConfig);
		
		connection.connect(function(err){
			if(!err) console.log("Database is connected. Put User Invite");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
		var username = req.body.username_responder;
		var response = {
			success: null,
			success_message: null
		};

		// Accept group invite
		if(req.body.response == 'true'){
			//Check to make sure that the token given is valid for the user
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
							username_to: req.body.username_inviter
						};
						
						//Make sure that the invite exists
						var sql = 'SELECT username_from, username_to FROM user_invite WHERE username_from = ? AND username_to = ?';
						var inserts = [req.body.username_inviter, username];
						sql = mysql.format(sql, inserts);
						connection.query(sql, function(err, data){
							if(err || data.length === 0){
								response.success = false;
								response.success_message = "Invite does not exist";
								console.log("invite does not exist");
								res.json(response);
							}
							else{
								//Check to see if the invitee is already in a group, if so, don't do anything
								connection.query('SELECT * FROM user_group WHERE username = ' + connection.escape(username), function(err, receiver_data) {
									if(err){
										response.success = false;
										response.success_message = "Error in query";
										res.json(response);
									}
									else{
										if(receiver_data.length !== 0){
											response.success = false;
											response.success_message = "Receiver is already in a group";

											//Remove the invitation
											var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
											var inserts = [req.body.username_inviter, username];
											sql = mysql.format(sql, inserts);

											connection.query(sql, function(err, data){	
												if(err){
													response.success = false;
													response.success_message = "Error removing into group";
													res.json(response);
												}
												else{
													response.success_message = "Cannot add a grouped user to a group";
													response.success = true;
													res.json(response);
												}
												connection.end();
											});
										}
										else{ 
											// Receiver not in a group, check if the sender is in a group
											connection.query('SELECT * FROM user_group WHERE username = ' + connection.escape(req.body.username_inviter), function(err, sender_data) {
												if(err){
													response.success = false;
													response.success_message = "Error in query";
													res.json(response);
												}
												else{
													if(sender_data.length !== 0){ 
														// Sender is in a group 
														// Add invitee to inviter's group
														var groupData = {
															id: sender_data[0].id,
															username: username
														}
														//Give the invitee the same group id as the inviter
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
															
															//Remove the invitation
															var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
															var inserts = [req.body.username_inviter, username];
															sql = mysql.format(sql, inserts);
															
															connection.query(sql, function(err, data){	
																if(err){
																	response.success = false;
																	response.success_message = "Error removing invitation";
																}
																else{
																	response.success = true;
																}
																connection.end();
															});
														});
													}
													else{ 
														// Inviter and invitee are both not in groups, so create a new group and add both of them
														connection.query('INSERT INTO user_group (username) VALUES (' + connection.escape(req.body.username_inviter) + ')', function(err, groupData) {
														console.log(groupData);
															if(err){
																response.success = false;
																response.success_message = "Error inserting into group";
																res.json(response);
																connection.end();
															}
															else{ 
																//Find group id of the group that the was just created
																connection.query('SELECT id FROM user_group WHERE username = ' + connection.escape(req.body.username_inviter), function(err, data2) {
																	// Add receiver to the same group
																	connection.query('INSERT INTO user_group (id, username) VALUES (' + data2[0].id + ',' + connection.escape(username) + ')', function(err) {
																		if(err){
																			response.success = false;
																			response.success_message = "Error inserting into group";
																			res.json(response);
																		}
																		else{
																			//Send a message to notify the user that the invite was accepted
																			send_single_message(req.body.username_inviter, req.body.username_responder, req.body.message_text, req.body.message_code, function(results) {
																				response = results
																				res.json(response);
																			});
																		}

																		// End of scenarios. Can delete the invite.
																		var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
																		var inserts = [req.body.username_inviter, username];
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
		else{ 
			//Invite was declined, delete the invitation
			var sql = 'DELETE FROM user_invite WHERE username_from = ? AND username_to = ?';
			var inserts = [req.body.username_inviter, username];
			sql = mysql.format(sql, inserts);
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
	})

//Send a message to a single user
function send_single_message(username_from, username_to, message_text, message_code, callback){
	var connection = mysql.createConnection(mysqlConfig);
		
		connection.connect(function(err){
			if(!err) console.log("Database is connected. Send Single Message");
			else {
				console.log("Error connecting database.");
				connection.end();
			}
		});
		
	var response = {
		success: null,
		success_message: null
	};
	
	//Check to make sure that neither of the two users have blocked each other
	connection.query('SELECT * ' + 
					 'FROM user_blacklist ' + 
					 'WHERE (username = ' + connection.escape(username_to) + ' AND block_username = ' + connection.escape(username_from) + ' ) ' + 
					 'OR ( username = ' + connection.escape(username_from) + ' AND block_username = ' + connection.escape(username_to) + ')', function(err, blocks){
		if(err){
			response.success = false;
			response.success_message = "Error checking blacklist for users: " + username_to + " " + username_from + ".";
			callback(response);
		}
		else if (blocks.length > 0){
			response.success = false;
			response.success_message = "You have been blocked by that user.";
			callback(response);
		}
		else{
			//Get the Google id of the receiver
			connection.query('SELECT gcm_regid FROM user_gcm WHERE username = ' + connection.escape(username_to), function(err, gcm_user){
				if (err || gcm_user.length === 0){
					response.success = false;
					response.success_message = "Failed to find existing gcm_regid from: " + username_to + ".";
					console.log("Failed to find existing gcm_regid from: " + username_to + ".");
					callback(response);
				}
				else{
					var to = gcm_user[0].gcm_regid;

					//Create the reqest for Google's server
					var postData = JSON.stringify({
						'registration_ids': [ to ],
						'data': {
							'message_code': message_code,
							'username_from': username_from,
							'message': message_text
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
							
							var message_chunk = JSON.parse(chunk)
							
							if(message_chunk.success === 1){
								response.success = true;
								response.success_message = 'message sent to GCM';	
								callback(response);
							}
							else{
								response.success = false;
								response.success_message = 'message could not be sent';
								callback(response);
							}
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
					
					
				}
			});
		}
	});
	
}

module.exports = router; // HAS TO BE AT THE BOTTOM