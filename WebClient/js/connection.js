var wsUri = "ws://localhost:61104/";
var websocket = new WebSocket(wsUri);

websocket.onopen = function (evt) {
	console.log("CONNECTED");
}

websocket.onclose = function (evt) {
	alert("Unable to establish a connection to the server.");
}

websocket.onmessage = function (evt) {
	SetCharacters(JSON.parse(evt.data));
}

websocket.onerror = function (evt) {
	console.log('ERROR:' + evt.data);
}

function SendToServer(message) {
	console.log("SENT: " + message);
	websocket.send(message);
}