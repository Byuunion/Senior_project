var express = require("express");
var router = express.Router();
var app = express();
var bodyParser = require('body-parser');

app.use(router);

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

var everythingelselib = require('./everythingelse');
var gcmlib = require('./requiresgcm');

app.use('/', everythingelselib)
app.use('/', gcmlib);

//This is the port for the general webapi
app.set('port', (process.env.PORT || 8800));

app.listen(app.get('port'), function(err){
	console.log("Listening on Port: " + app.get('port'));
});

process.on('uncaughtException', function(err) {
    if(err.errno === 'EADDRINUSE') console.log("Port already occupied by host: " + app.get('port'));
});  