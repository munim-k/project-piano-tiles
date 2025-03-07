mergeInto(LibraryManager.library, {
    ACS_GenerateNonce: function () {
        console.log("🔹 Generating Nonce...");
        let nonce = allocate(intArrayFromString(window.crypto.randomUUID()), 'i8', ALLOC_NORMAL);
        console.log("Returning generated nonce: ", nonce);
        return nonce;
    },

    ACS_GenerateSignature: function (itemsJsonPtr, timestamp, noncePtr, apiSecretPtr) {
        let itemsJson = UTF8ToString(itemsJsonPtr);
        let nonce = UTF8ToString(noncePtr);
        let apiSecret = UTF8ToString(apiSecretPtr);
        let finalStr = itemsJson + timestamp + nonce;

        console.log("✅ Nonce:", nonce);
        console.log("✅ Items JSON:", itemsJson);
        console.log("✅ API Secret:", apiSecret);
        console.log("✅ Final String for Signature:", finalStr);

        let encoder = new TextEncoder();
        let key = encoder.encode(apiSecret);
        let msg = encoder.encode(finalStr);

        crypto.subtle.importKey("raw", key, { name: "HMAC", hash: "SHA-256" }, false, ["sign"])
            .then(hmacKey => crypto.subtle.sign("HMAC", hmacKey, msg))
            .then(signature => {
                let hexSig = Array.from(new Uint8Array(signature))
                    .map(b => b.toString(16).padStart(2, '0'))
                    .join('');

                console.log("✅ Generated Signature:", hexSig);

                // ✅ Send Signature to Unity
                const unityGame = typeof unityInstance !== "undefined" ? unityInstance : window.UnityInstance;
                if (unityGame) {
                    unityGame.SendMessage("ACSManager", "ReceiveSignature", hexSig);
                } else {
                    console.error("❌ Unity instance not found!");
                }
            })
            .catch(err => console.error("❌ Error generating signature:", err));
    },

    ACS_SendRequest: function (noncePtr, signaturePtr, timestampPtr, bodyJsonPtr) {
        console.log("🔹 Sending API Request...");

        let nonce = UTF8ToString(noncePtr);
        let signature = UTF8ToString(signaturePtr);
        let timestamp = UTF8ToString(timestampPtr);
        let bodyJson = UTF8ToString(bodyJsonPtr);

        console.log("✅ Nonce:", nonce);
        console.log("✅ Signature:", signature);
        console.log("✅ Timestamp:", timestamp);
        console.log("✅ Body JSON:", bodyJson);

        let headers = new Headers({
            "Content-Type": "application/json",
            "x-timestamp": timestamp,
            "x-nonce": nonce,
            "x-signature": signature
        });

        fetch("https://test4.xzsean.eu.org/acs/addDiscretionaryPointsBatch", {
            method: "POST",
            headers: headers,
            body: bodyJson
        })
        .then(response => response.text().then(responseText => ({
            responseText,
            status: response.status
        })))
        .then(({ responseText, status }) => {
            console.log(`✅ API Response [${status}]:`, responseText);

            if (typeof UnityInstance !== "undefined" && UnityInstance !== null) {
                UnityInstance.SendMessage("ACSManager", "OnACSResponse", JSON.stringify({
                    status,
                    response: responseText
                }));
            } else {
                console.error("❌ UnityInstance not available.");
            }
        })
        .catch(error => {
            console.error("❌ API Request Error:", error);

            if (typeof UnityInstance !== "undefined" && UnityInstance !== null) {
                UnityInstance.SendMessage("ACSManager", "OnACSResponse", JSON.stringify({
                    error: error.message
                }));
            }
        });
    }
});
