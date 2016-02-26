var app = require('express')();
var mysql = require('mysql'); //import mysql package.
var bodyParser = require('body-parser');

var mysqlConfig = {
        host: 'userdb.cydfufqp5imu.us-east-1.rds.amazonaws.com',
        user: 'overlord',
        password: 'jaypeg55',
		port: '3306',
        database: 'userdb',
    };
	
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

app.set('port', (process.env.PORT || 8800));

app.get('/user', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
	if(!err) {
		console.log("Database is connected ... nn");    
	} else {
		console.log("Error connecting database ... nn");    
	}
	});  
	
	var response = {
        success: null,
        success_message: "All users retrieved onto Database"
    };

    connection.query('SELECT * FROM user_profile', function(err, data) {
        if (err) throw err;
        res.json(data);
    });
    connection.end();
});

app.get('/user/username/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);
	
    connection.connect(function(err){
	if(!err) {
		console.log("Database is connected ... nn");    
	} else {
		console.log("Error connecting database ... nn");    
	}
	});  
	
	var response = {
        success: null,
        success_message: "User retrieved onto Database: " + req.params.username
    };

    connection.query('SELECT * FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data) {
        if (err) throw err;
        res.json(data);
    });
    connection.end();
});

app.post('/user', function(req, res) {
	
    var connection = mysql.createConnection(mysqlConfig);

    connection.connect(function(err){
	if(!err) {
		console.log("Database is connected ... nn");    
	} else {
		console.log("Error connecting database ... nn");    
	}
	});

    var data = {
        username: req.body.username,
		first_name: req.body.first_name,    
		last_name: req.body.last_name,     
		positive_votes: req.body.positive_votes,
		negative_votes: req.body.negative_votes,
		current_lat: req.body.current_lat,
		current_long: req.body.current_long
    };

    var response = {
        success: null,
        success_message: "User written onto Database"
    };

    connection.query('INSERT INTO user_profile SET ?', data, function(err, rows) {
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
    connection.end();
});

app.listen(app.get('port'), function(){
   console.log("Listening on Port: " + app.get('port'));
});


/***** REMEMBER TO INCLUDE RESPONSE CODES AND LINKS TO OTHER API URIS
LOOKUP RESTFUL RESPONSE CODES AND HATEOAS ***/



/**
 * Invokes RESTful call to add an event
 * @param {Object} req    The JSON of the event 
 * returns JSON with status
 */
 
 connection.query('INSERT INTO user_profile SET ?', data, function(err, rows) {
