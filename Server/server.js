var app = require('express')();
var mysql = require('mysql'); //import mysql package.
var bodyParser = require('body-parser');
var hat = require('hat');
var GOOGLE_API_KEY = 'AIzaSyAy3SrgunF-XyYESfgRfWjjIFiK8lajhaI';
var http = require('http');
var querystring = require('querystring');

//This is the config for the mysql connection, not the general webapi
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
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    connection.query('SELECT * FROM user_profile', function(err, data) {
        if (err) throw err;
        res.json(data);
	connection.end();
    });
});

app.get('/user/profile/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    connection.query('SELECT * FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        res.json(data);
	connection.end();
    });
});

app.get('/user/language/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    connection.query('SELECT * FROM user_language WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        res.json(data);
	connection.end();
    });
});

app.get('/user/interest/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    connection.query('SELECT * FROM user_interest WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        res.json(data);
	connection.end();
    });
});

app.get('/user/message/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    connection.query('SELECT * FROM user_message WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        res.json(data);
	connection.end();
    });
});

//Pass in username and password from the app's url
//Hash not implemented yet
app.get('/user/login/:username/:password', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);
    var password = req.params.password;

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
                else console.log("Error connecting database.");
        });

    var response = {
        success: null,
        success_message: null,
        token: null
    };

    connection.query('SELECT password_hash FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
	if (err) throw err;
	if(data.password_hash = password){ // Success
	    var token = hat();
	    response.success = true;
	    response.success_message = "User successfully logged in: " + req.params.username;
	    response.token = token;
	    connection.query('UPDATE user_login SET token = "' + token + '" WHERE username = ' + connection.escape(req.params.username));
	    res.json(response);
	    connection.end();
	    }
	else{
	    response.success = false;
	    response.success_message = "Username/Password pair not found";
	    res.json(response);
	    connection.end();
	    }
    });
});

app.post('/user/profile', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});
    
    var username = req.body.username;
    
    var response = {
	success: null,
	success_message: null
	};
    
    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
        if (err) throw err;
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

	    connection.query('INSERT INTO user_profile SET ?', data, function(err, rows) {
		if (err)throw err;
		
		response.success = true;
		response.success_message = "Successfully created profile: " + data.username + ".";
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

app.post('/user/interest/', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});
    
    var username = req.body.username;
    
    var response = {
	success: null,
	success_message: null
	};
    
    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
	if(req.body.token === token){
	    var data = {
		username: username,
		interest: req.body.interest
		};

	    connection.query('INSERT INTO user_interest SET ?', data, function(err, rows) {
		if (err) throw err;

		response.success = true;
		response.success_message = "Successfully created interest.";
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

app.post('/user/language', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});
    
    var username = req.body.username;
    
    var response = {
	success: null,
	success_message: null
	};
    
    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
	if(req.body.token === token){

	    var data = {
		username: username,
		language: req.body.language
		};

	    connection.query('INSERT INTO user_language SET ?', data, function(err, rows) {
		if (err)throw err;

		response.success = true;
		response.success_message = "Successfully added language.";
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

//Create account
app.post('/user/login', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});
    
    var token = hat();

    var data = {
        username: req.body.username,
        password_hash: req.body.password,
	token: token
    };

    var response = {
        success: null,
        success_message: null,
	token: token
    };

    connection.query('INSERT INTO user_login SET ?', data, function(err, rows) {
        if (err)throw err;

        response.success = true;
	response.success_message = "Successfully created user: " + data.username + ".";
        res.json(response);
	connection.end();
    });
});


app.post('/user/gcmregid', function(req, res) {
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

			connection.query('INSERT INTO user_gcm SET ?', data, function(err, rows) {
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


app.post('/user/message', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
                else console.log("Error connecting database.");
        });

    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.body.username_from), function(err, data){
	if (err) throw err;
	var token = data[0].token;

		var response = {
			success: null,
	 		success_message: null
		};

		if(req.body.token === token){
			connection.query('SELECT gcm_regid FROM user_gcm WHERE username = ' + connection.escape(req.body.username_to), function(err, gcm_user){
				if(err) throw err;
				
				var to = gcm_user[0].gcm_regid;

				var postData = JSON.stringify({
					'registration_ids': [ to ],
					'data': {
						'title': 'Test Title',
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
/*
app.post('/user/message', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});
    
    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.body.username_from), function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
	var response = {
	    success: null,
	    success_message: null
	    };
	
	if(req.body.token === token){
	    
	    var data = {
		username_to: req.body.username_to, 
		username_from: req.body.username_from, 
		message_time: req.body.message_time, 
		message_text: req.body.message_text,
		};

	    connection.query('INSERT INTO user_message SET ?', data, function(err, rows) {
		if (err) throw err;

		response.success = true;
		response.success_message = "Successfully inserted user message.";
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
*/
app.delete('/user', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    var username = req.body.username;
    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
        if (err) throw err;
        var token = data[0].token;
	
	var response = {
	    success: null,
	    success_message: null
	    };
    
	if(req.body.token === token){
	    //Delete Cascades through tables
	    connection.query('DELETE FROM user_login WHERE username = ' + connection.escape(username), function(err, data){
		if (err) throw err;
		response.success = true;
		response.success_message = "User information deleted from Database";
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

app.put('/user/profile/location/:username', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});

    connection.query('SELECT token FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
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

	    //Delete Cascades through tables
	    connection.query('UPDATE user_profile SET ? WHERE username = ' + connection.escape(req.params.username), data,  function(err, data){
		if (err) throw err;
		response.success = true;
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

app.get('/user/:min_lat/:max_lat/:min_long/:max_long/:yourLat/:yourLong', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
	else console.log("Error connecting database.");
	});
    
    var radius = 0.00126291; // 5 miles
    connection.query('SELECT username FROM (SELECT current_lat, current_long, username FROM user_profile WHERE (current_lat >=' + connection.escape(req.params.min_lat) + ' AND current_lat <= ' + connection.escape(req.params.max_lat) + ') AND (current_long >= ' + connection.escape(req.params.min_long) + ' AND current_long <= ' + connection.escape(req.params.max_long) + ')) AS reducedProfiles  WHERE acos(sin('  + connection.escape(req.params.yourLat) + ') * sin(current_lat) + cos(' + connection.escape(req.params.yourLat) + ') * cos(current_lat) * cos(current_long -' + connection.escape(req.params.yourLong) + ')) <=' + radius, function(err,data){
    // connection.query('SELECT username FROM user_profile WHERE (current_lat >= ' + connection.escape(req.params.min_lat) + ' AND current_lat <= ' + connection.escape(req.params.max_lat) + ') AND (current_long >= ' + connection.escape(req.params.min_long) + ' AND current_long <= ' + connection.escape(req.params.max_long) + ')', function(err, data){ 
   if (err) throw err;
        res.json(data);
    });
    connection.end();
});


app.listen(app.get('port'), function(){
   console.log("Listening on Port: " + app.get('port'));
});
