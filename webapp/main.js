window.DEBUG = 0;

window.DebugLog = (component, message) => {
    if(window.DEBUG==1) console.log("%c[%s]%c %s", 'color:orange;', component, 'color:black;', message);
}

function InitClient() {
    window.DebugLog("Client", "Connecting client");
    window.CollabSocket.connect("localhost:9090");
	// Right now the client can only techinically see one vm.
	// It also autoconnects to said VM.
	// We'll need to have a list and connection stuff later (will this JS even survive testing?)
}

(function () {
	function LoadJS(path, loadedCallback) {
		var script = document.createElement('script');
		script.onload = function () {
			// called when dep is ready
			loadedCallback();
		};
		script.src = path;
		document.head.appendChild(script)
	}

    // Initalize client
	window.DebugLog("Ldr", "Loading JS files required for the client");

    LoadJS("js/instc.js", () => {
        window.DebugLog("Ldr", "instc.js loaded");
		
        LoadJS("js/socket.js", () => {
            window.DebugLog("Ldr", "socket.js loaded, initalizing client");
            InitClient();
        });
		
    });

})();

function canvasMove(e) {
    function getMousePos(canvas, evt) {
        var rect = canvas.getBoundingClientRect();
        return {
            x: Math.round(evt.clientX - rect.left),
            y: Math.round(evt.clientY - rect.top)
        };
    }
    var pos = getMousePos(document.getElementById("screen"), e);
    window.CollabSocket.sendMessage(["mouse", pos.x.toString(), pos.y.toString(), '0']);
}
