window.DEBUG = 0;
var connected = false;

window.DebugLog = (component, message) => {
    if(window.DEBUG==1) console.log("%c[%s]%c %s", 'color:orange;', component, 'color:black;', message);
}


 function InitClient() {
    
    window.DebugLog("Client", "Connecting client");
    window.CollabSocket.connect("localhost:9090", () => {
        connected = true;
        window.DebugLog("Client", "Connected to server");
        window.CollabSocket.sendMessage(["connect", "test"]);
    });
    
}

(function () {
	// loads js dynamically without jquery
    function LoadJS(path, loadedCallback) {
		var script = document.createElement('script');
		script.onload = function () {
			// called when dep is ready
			loadedCallback();
		};
		script.src = path;
		document.head.appendChild(script);
	}

    // Initalize client
	window.DebugLog("Client", "Loading JS files required for the client");

    LoadJS("js/instc.js", () => {
        window.DebugLog("Client", "instc.js loaded");
		
        LoadJS("js/socket.js", () => {
            window.DebugLog("Client", "socket.js loaded, initalizing client");
            InitClient();
        });
		
    });

})();

function getMousePos(canvas, evt) {
    var rect = canvas.getBoundingClientRect();
    return {
        x: Math.round(evt.clientX - rect.left),
        y: Math.round(evt.clientY - rect.top)
    };
}

function canvasMove(e) {
    if (connected) {
        var pos = getMousePos(document.getElementById("screen"), e);
        window.CollabSocket.sendMessage(["mouse", pos.x.toString(), pos.y.toString(), e.buttons.toString()]);
    }
}
