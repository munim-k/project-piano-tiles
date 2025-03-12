mergeInto(LibraryManager.library, {
    ACS_GenerateNonce: function () {
        if (typeof ACSPlugin === "undefined") {
            console.error("❌ ACSPlugin is not loaded. Make sure it's included in the WebGL template.");
            return allocate(intArrayFromString("Error"), ALLOC_NORMAL);
        }

        let nonce = ACSPlugin.generateNonce();
        return allocate(intArrayFromString(nonce), ALLOC_NORMAL);
    },

    ACS_GenerateSignature: function (itemsJsonPtr, timestampPtr, noncePtr, apiSecretPtr) {
        let itemsJson = UTF8ToString(itemsJsonPtr);
        let timestamp = parseInt(UTF8ToString(timestampPtr));
        let nonce = UTF8ToString(noncePtr);
        let apiSecret = UTF8ToString(apiSecretPtr);

        let items = JSON.parse(itemsJson);

        ACSPlugin.generateSignature({ items, timestamp, nonce, apiSecret })
            .then(signature => {
                window.UnityInstance.SendMessage("ACSManager", "ReceiveSignature", signature);
            })
            .catch(err => console.error("❌ Error generating signature:", err));
    },

    ACS_SendRequest: function (noncePtr, signaturePtr, timestampPtr, bodyJsonPtr) {

    let nonce = UTF8ToString(noncePtr);
    let signature = UTF8ToString(signaturePtr);
    let timestamp = UTF8ToString(timestampPtr);
    let bodyJson = UTF8ToString(bodyJsonPtr);

    let headers = {
        "Content-Type": "application/json",
        "x-timestamp": timestamp,
        "x-nonce": nonce,
        "x-signature": signature
    };

    fetch("https://test4.xzsean.eu.org/acs/addDiscretionaryPointsBatch", {
        method: "POST",
        headers: headers,
        body: bodyJson
    })
    .then(response => {
        console.log(`🔹 HTTP Status Code: ${response.status}`);

        if (response.status === 201) {
            console.log("Success! API returned 201 Created.");
        } else if (response.status === 401) {
            console.error("Unauthorized! API returned 401.");
        } else {
            console.warn(`Unexpected Status Code: ${response.status}`);
        }

        return response.text(); // Read raw response first
    })
    .then(text => {
        console.log("🔹 Raw API Response:", text);
        return text ? JSON.parse(text) : {}; // Parse only if not empty
    })
    .then(data => {

        if (typeof window.UnityInstance !== "undefined" && window.UnityInstance !== null) {
            window.UnityInstance.SendMessage("ACSManager", "OnACSResponse", JSON.stringify({
                status: response.status,
                response: data
            }));
        } else {
            console.error("❌ UnityInstance not available.");
        }
    })
    .catch(error => {
        console.error("❌ API Request Error:", error);

        if (typeof window.UnityInstance !== "undefined" && window.UnityInstance !== null) {
            window.UnityInstance.SendMessage("ACSManager", "OnACSResponse", JSON.stringify({
                error: error.message
            }));
        }
    });
}

});
