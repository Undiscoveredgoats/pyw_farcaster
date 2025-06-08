mergeInto(LibraryManager.library, {
    GetQueryParam: function(paramId) {
        var urlParams = new URLSearchParams(location.search);
        var param = urlParams.get(Pointer_stringify(paramId));
        console.log("JavaScript read param: " + param);
        var bufferSize = lengthBytesUTF8(param) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(param, buffer, bufferSize);
        return buffer;
    },
    InjectWebhooks: function(){
        function checkUnityInstance() {
            if (window.unityInstance) {
                window.addEventListener('message', (event) => {
                    try {
                        console.log('Received postMessage:', event);
                        var { callbackType, callbackUuid, data, ruleActionType } = event.data.payload || {};
                
                        if (event.data.source !== "basement") {
                            console.log('Message source not recognized. Ignoring message.');
                            return;
                        }

                        if (!callbackType || !callbackUuid || !ruleActionType || !data) {
                            console.error('Invalid message data received:', event.data);
                            return; // Ignore invalid data
                        }
                        
                
                        if (data.error) {
                            console.error('Error in message data:', data.error);
                        }
                
                        window.unityInstance.SendMessage("B3WebhooksHandler", "onWebhookReceived", JSON.stringify(event.data.payload));
                    } catch (error) {
                        console.error('Error processing postMessage:', error);
                    }
                });
                console.log("B3 Webhook handler injected")
            } else {
                setTimeout(checkUnityInstance, 100); // Check every 100ms
            }
        }

        checkUnityInstance();
    },
    PostLauncherMessage: function (message){
        window.bridgeMessageToLauncher(JSON.parse(UTF8ToString(message)));
    }
});