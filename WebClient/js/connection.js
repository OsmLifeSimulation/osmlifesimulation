var wsUri = "ws://localhost:8080/";
var websocket = new WebSocket(wsUri);
  
websocket.onopen = function(evt)
{
	console.log("CONNECTED");
}

websocket.onclose = function(evt)
{
	console.log("DISCONNECTED");
}

websocket.onmessage = function(evt)
{
	var msg = evt.data;
	if(msg[0] == "["){
		//console.log("packet");
		SetCharacters(JSON.parse(evt.data));
	}
	else {
		console.log('RESPONSE: ' + msg);
	}
}

websocket.onerror = function(evt)
{
	console.log('ERROR:' + evt.data);
	
	SetCharacters([[44.4766846, 48.8045959]]);
}

function SendToServer(message)
{
	console.log("SENT: " + message);
	websocket.send(message);
}