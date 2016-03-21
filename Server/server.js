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

app.get('/user/profile/:username', function(req, res) {
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


    connection.query('SELECT * FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
        res.json(data);
    });
    connection.end();
});

//Pass in JSON format of entered username and password from the app
//Hash not implemented yet
app.get('/user/login/:username', function(req, res) {
    var connection = mysql.createConnection(mysqlConfig);
		var password = req.body.username;

    connection.connect(function(err){
        if(!err) {
            console.log("Database is connected ... nn");
            } else {
                console.log("Error connecting database ... nn");
                }
        });

    var response = {
        success: null,
        success_message: "User successfully logged in?: " + req.params.username
    };


    connection.query('SELECT password_hash FROM user_login WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
		if(data.password_hash = password){ // Success
			var token = hat();
			//Cognito shit here================================================================================================================================

			var cognitoidentity = new AWS.CognitoIdentity();
			
			//Params for making the API call
			var params = {
				AccountId: 267911792901, // AWS account Id
				RoleArn: arn:aws:iam::267911792901:role/Cognito_MeetMeetAuth_Role, // IAM role that will be used by authentication
				IdentityPoolId: us-east-1:9fc7967a-222c-4b86-8c09-36ca25e76a72, //ID of the identity pool
				Logins: {'login.rowan.meetmeet': TOKEN} 
			};
				 
			//Initialize the Credentials object
			AWS.config.credentials = new AWS.CognitoIdentityCredentials(params);
			 
			//Call to Amazon Cognito, get the credentials for our user
			AWS.config.credentials.get(err,data){…}
		}
    });
	
    connection.end();
});

app.post('/user/profile', function(req, res) {

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
        current_long: req.body.current_long,
        gender: req.body.gender,
        bio: req.body.bio
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

app.post('/user/interest', function(req, res) {

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
    connection.end();
});

app.post('/user/language', function(req, res) {

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
        language: req.body.language
    };

    var response = {
        success: null,
        success_message: "User written onto Database"
    };

    connection.query('INSERT INTO user_language SET ?', data, function(err, rows) {
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

app.post('/user/login', function(req, res) {

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
        password_hash: req.body.password_hash,
        password_salt: req.body.password_salt
    };

    var response = {
        success: null,
        success_message: "User written onto Database"
    };

    connection.query('INSERT INTO user_login SET ?', data, function(err, rows) {
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

app.post('/user/messages', function(req, res) {

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
        current_long: req.body.current_long,
        gender: req.body.gender,
        bio: req.body.bio
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

app.delete('/user/:username', function(req, res) {

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
        success_message: "User information deleted from Database"
    };

	//Delete Cascades through tables
    connection.query('DELETE FROM user_profile WHERE username = ' + connection.escape(req.params.username), function(err, data){
        if (err) throw err;
    });
    connection.end();
});

app.listen(app.get('port'), function(){
   console.log("Listening on Port: " + app.get('port'));
});

function getCognitoID(){

  var params = {
    AccountId: 267911792901, // AWS account Id
	RoleArn: arn:aws:iam::267911792901:role/Cognito_MeetMeetAuth_Role, // IAM role that will be used by authentication
	IdentityPoolId: us-east-1:9fc7967a-222c-4b86-8c09-36ca25e76a72, //ID of the identity pool
    Logins: {'login.rowan.meetmeet': TOKEN} 
  };

  AWS.config.region = AWS_Region;

  /* initialize the Credentials object */
  AWS.config.credentials = new AWS.CognitoIdentityCredentials(params);

  /* Get the credentials for our user */
  AWS.config.credentials.get(function(err) {
    if (err) console.log("credentials.get: ".red + err, err.stack); /* an error occurred */
      else{
	&AWS_TEMP_CREDENTIALS = AWS.config.credentials.data.Credentials;
        COGNITO_IDENTITY_ID = AWS.config.credentials.identityId;
        console.log("Cognito Identity Id: ".green + COGNITO_IDENTITY_ID);

      }
  });
}
