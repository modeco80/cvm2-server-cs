window.InstCodec = {
    decode: (string) => {
        var pos = -1;
        var sections = [];
        for(;;){
            var len = string.indexOf('.', pos + 1);
            
            if(len == -1){
                break;
            }

            pos = parseInt(string.slice(pos+1,len)) + len + 1;

            sections.push(string.slice(len + 1, pos)
                .replace(/&#x27;/g,"'")
                .replace(/&quot;/g,'"')
                .replace(/&#x2F;/g,'/')
                .replace(/&lt;/g,'<')
                .replace(/&gt;/g,'>')
                .replace(/&amp;/g,'&'));

            if(string.slice(pos,pos +1 ) == ';'){
                break;
            }
        }
        return sections;
    },

    encode: (arguments) => {
        // lifted from guac wstunnel code (i thought original code i lifted everything from in the first place
       // had some form of deadlock and this didnt but uhh oopstm )
        function getElement(value) {
            var string = new String(value);
            return string.length + "." + string; 
        }
        var message = getElement(arguments[0]);

        for (var i=1; i<arguments.length; i++)
            message += "," + getElement(arguments[i]);

        message += ";";
        return message;
    }
};