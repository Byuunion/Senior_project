var express = require("express");
var router = express.Router();
var app = express();
var bodyParser = require('body-parser');
var fs = require('fs');
var http = require('http');
var https = require('https');

var options = {
  key: fs.readFileSync('./key.pem', 'utf8'),
  cert: fs.readFileSync('./server.crt', 'utf8')
};

app.use(router);

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

var everythingelselib = require('./everythingelse');
var gcmlib = require('./requiresgcm');

app.use('/', everythingelselib)
app.use('/', gcmlib);

// Create an HTTP service.
http.createServer(app).listen(80);
// Create an HTTPS service identical to the HTTP service.
https.createServer(options, app).listen(443);

//This is the port for the general webapi
app.set('port', (process.env.PORT || 8800));

app.listen(app.get('port'), function(err){
	console.log("Listening on Port: " + app.get('port'));
});

process.on('uncaughtException', function(err) {
    if(err.errno === 'EADDRINUSE') console.log("Port already occupied by host: " + app.get('port'));
});  