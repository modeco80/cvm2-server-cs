// Client driving stuff

var ws = null;
var canvas = null;
var ctx = null;
// Once a "rect" instruction is recieved, this variable will be set to true.
// The next WS transmission that is a byte stream
// will be treated as display data, and this variable will be reset.
var expectingDisplayBytes = false;
var expectedDisplayRect = null;

window.CollabSocket = {

    connect: (host, opencallback) => {
        window.DebugLog("Socket", "Connecting to " + host);
       
		ws = new WebSocket("ws://" + host, "cvm2");
		ws.binaryType = 'arraybuffer';
		
		ws.onopen = () => {
		    opencallback();
		}
        
		ws.onmessage = (message) => {
			
			window.DebugLog("Socket", "Message recieved");
			if(expectingDisplayBytes == false) {
			    if (typeof (message.data) === "string") {
			        // decode Guacamole message
			        DecodeInstruction(message);
			    }

			} else {
				
				if(typeof(message.data) === "string") {
					// Short circuit to Guacamole path again
					DecodeInstruction(message); return; 
				}
				
				window.DebugLog("Socket", "Recieved binary message whilist waiting for display");
				if (expectedDisplayRect == null) return;
				
			    // attempt to draw the image
				canvas = document.getElementById("screen");
				var ctx = canvas.getContext("2d");
				var buf = new Uint8Array(message.data);
				var blob = new Blob([buf], {type: 'image/png'});
				var url = URL.createObjectURL(blob);
				
				var img = new Image();
				img.onload = () => {
					window.DebugLog("Socket", "Drawing image at " + expectedDisplayRect.x + ',' + expectedDisplayRect.y);
					ctx.drawImage(img, expectedDisplayRect.x, expectedDisplayRect.y);
					expectingDisplayBytes = false;
					//expectedDisplayRect = null;
					
					// GC hinting (just to make sure)
					url = null;
					blob = null;
					buf = null;
					img = null;
					ctx = null;
					canvas = null;
				}
				img.src = url;

			}
		}
		
	},
	
	sendMessage: (ToEncode) => {
		ws.send(window.InstCodec.encode(ToEncode));
	}
};

function DecodeInstruction(message){
	    var decoded = window.InstCodec.decode(message.data);
		switch (decoded[0]) {
			default: break;
			case "rect": {
				// Set everything up to expect a
				// binary PNG message.
				expectingDisplayBytes = true;
				expectedDisplayRect = { 
					x: parseInt(decoded[1]), 
					y: parseInt(decoded[2]),
					w: parseInt(decoded[3]),
					h: parseInt(decoded[4])
				};
			} break;
		}
}