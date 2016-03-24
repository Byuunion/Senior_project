var app = require('express')();
var mysql = require('mysql'); //import mysql package.
var bodyParser = require('body-parser');
var hat = require('hat');

var mysqlConfig = {
        host: 'userdb.cydfufqp5imu.us-east-1.rds.amazonaws.com',
        user: 'overlord',
        password: 'jaypeg55',
		port: '3306',
        database: 'userdb',
    };

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

app.set('port', (process.env.PORT || 8801));

app.get('/user', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});

    connection.query('SELECT * FROM user_profile', function(err, data) {
        if (err) throw err;
        res.json(data);
    });
    connection.end();
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
    });
    connection.end();
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
    });
    connection.end();
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
    });
    connection.end();
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
    });
    connection.end();
});

//Pass in JSON format of entered username and password from the app
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
        success_message: "User successfully logged in: " + req.params.username,
        token: null
    };


    connection.query('SELECT password_hash FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
	if (err) throw err;
			if(data.password_hash = password){ // Success
					var token = hat();
					response.success = true;
					response.token = token;
					connection.query('UPDATE user_login SET token = "' + token + '" WHERE username = ' + connection.escape(req.params.username));
					res.json(response);
			}
			else{
					response.success = false;
					response.success_message = "Username/Password pair not found";
					res.json(response);
			}
		connection.end();	
    });
});


app.post('/user/profile', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});
	
	var username = req.body.username;
	
	connection.query('SELECT token FROM user_profile WHERE username = ' + username, function(err, data){
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

			var response = {
				success: null,
				success_message: "User written onto Database"
			};

			connection.query('INSERT INTO user_profile SET ?', data, function(err, rows) {
				if (err)throw err;

				if (rows.affectedRows === 1) {
				  response.success = true;
					response.success_message = "Successfully created user: " + data.username + ".";
				} else if (!!rows.affectedRows) {
					response.success = false;
					response.success_message = "Improper INSERT executed: Multiple inserts.";
				} else {
					response.success = false;
					response.success_message = "Improper INSERT executed: INSERT failed.";
				}
				res.json(response);
			});
		}	
		else{
			response.success = false;
			response.success_message = "Token didn't match";
			res.json(response);
		}
	});
	connection.end();
});

app.post('/user/interest/', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});
	
	var username = req.body.username;
		
	connection.query('SELECT token FROM user_profile WHERE username = ' + username, function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
		if(req.body.token === token){
			var data = {
				username: username,
				interest: req.body.interest
			};

			var response = {
				success: null,
				success_message: "User written onto Database"
			};

			connection.query('INSERT INTO user_interest SET ?', data, function(err, rows) {
				if (err) {
					console.log(data);
					throw err;
					}

				if (rows.affectedRows === 1) {
				  response.success = true;
					response.success_message = "Successfully created user: " + data.username + ".";
				} else if (!!rows.affectedRows) {
					response.success = false;
					response.success_message = "Improper INSERT executed: Multiple inserts.";
				} else {
					response.success = false;
					response.success_message = "Improper INSERT executed: INSERT failed.";
				}
				res.json(response);
			});
		}	
		else{
			response.success = false;
			response.success_message = "Token didn't match";
			res.json(response);
		}
	});
	connection.end();
});

app.post('/user/language', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});
		
	var username = req.body.username;
	
	connection.query('SELECT token FROM user_profile WHERE username = ' + username, function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
		if(req.body.token === token){

			var data = {
				username: username,
				language: req.body.language
			};

			var response = {
				success: null,
				success_message: "User written onto Database"
			};

			connection.query('INSERT INTO user_language SET ?', data, function(err, rows) {
				if (err)throw err;

				if (rows.affectedRows === 1) {
				  response.success = true;
					response.success_message = "Successfully created user: " + data.username + ".";
				} else if (!!rows.affectedRows) {
					response.success = false;
					response.success_message = "Improper INSERT executed: Multiple inserts.";
				} else {
					response.success = false;
					response.success_message = "Improper INSERT executed: INSERT failed.";
				}
				res.json(response);
			});
		}
		else{
			response.success = false;
			response.success_message = "Token didn't match";
			res.json(response);
		}
	});
	connection.end();
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
        password_hash: req.body.password_hash,
        password_salt: req.body.password_salt,
		token: token
    };

    var response = {
        success: null,
        success_message: "User written onto Database"
    };

    connection.query('INSERT INTO user_login SET ?', data, function(err, rows) {
        if (err)throw err;

        if (rows.affectedRows === 1) {
          response.success = true;
            response.success_message = "Successfully created user: " + data.username + ".";
        } else if (!!rows.affectedRows) {
            response.success = false;
            response.success_message = "Improper INSERT executed: Multiple inserts.";
        } else {
            response.success = false;
            response.success_message = "Improper INSERT executed: INSERT failed.";
        }
        res.json(response);
    });
    connection.end();
});

app.post('/user/messages', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});
	
    connection.query('SELECT token FROM user_profile WHERE username = ' + req.body.username, function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
		if(req.body.token === token){
			
			var data = {
				username_to: req.body.username_to, 
				username_from: req.body.username_from, 
				message_time: req.body.message_time, 
				message_text: req.body.message_text,
			};

			var response = {
				success: null,
				success_message: null
			};

			connection.query('INSERT INTO user_message SET ?', data, function(err, rows) {
				if (err) throw err;

				if (rows.affectedRows === 1) {
				  response.success = true;
					response.success_message = "Successfully inserted user message.";
				} else if (!!rows.affectedRows) {
					response.success = false;
					response.success_message = "Improper INSERT executed: Multiple inserts.";
				} else {
					response.success = false;
					response.success_message = "Improper INSERT executed: INSERT failed.";
				}
				res.json(response);
			});
		}
		else{
			response.success = false;
			response.success_message = "Token didn't match";
			res.json(response);
		}
	});
    connection.end();
});

app.delete('/user/:username', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});

	connection.query('SELECT token FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
		if(req.body.token === token){
			var response = {
				success: null,
				success_message: "User information deleted from Database"
			};

			//Delete Cascades through tables
			connection.query('DELETE FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
				if (err) throw err;
				response.success = true;
				res.Json(response);
			});
		}
		else{
			response.success = false;
			response.success_message = "Token didn't match";
			res.json(response);
		}
	});
	connection.end();
});

app.put('/user/profile/location/:username', function(req, res) {

    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});

	connection.query('SELECT token FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        var token = data[0].token;
    
		if(req.body.token === token){
			var response = {
				success: null,
				success_message: "User location updated"
			};
			
			var data = {
				current_lat: req.body.current_lat,
				current_long: req.body.current_long
			};

			//Delete Cascades through tables
			connection.query('UPDATE user_profile SET ? WHERE username = ' + connection.escape(req.params.username), data,  function(err, data){
				if (err) throw err;
				response.success = true;
				res.Json(response);
			});
		}
		else{
			response.success = false;
			response.success_message = "Token didn't match";
			res.json(response);
		}
	});
	connection.end();
});

app.get('/user/location', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
        if(!err) console.log("Database is connected.");
		else console.log("Error connecting database.");
	});
	
	var radius = 0.00126291; // 5 miles
    connection.query('SELECT * FROM (SELECT * FROM user_profile WHERE (current_lat >=' + req.body.min_lat + 'AND current_lat <=' + req.body.max_lat + ') AND (current_long >=' + req.body.min_long + 'AND current_long <=' + req.body.max_long + ')) WHERE acos(sin('  + req.body.lat + ') * sin(' + current_lat + ') + cos(' + req.body.lat + ') * cos(' + current_lat + ') * cos(' + current_long + '-' + req.body.long + ')) <=' + radius, 
	
	
function(err, data) {
        if (err) throw err;
        res.json(data);
    });
    connection.end();
});


app.listen(app.get('port'), function(){
   console.log("Listening on Port: " + app.get('port'));
});
