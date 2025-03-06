mergeInto(LibraryManager.library, {
  ACS_GenerateNonce: function () {
    let nonce = crypto.getRandomValues(new Uint8Array(16));
    let nonceHex = Array.from(nonce).map(b => b.toString(16).padStart(2, '0')).join('');
    return allocate(intArrayFromString(nonceHex), 'i8', ALLOC_NORMAL);
  },

  ACS_GenerateSignature: function (itemsJsonPtr, timestamp, noncePtr, apiSecretPtr) {
    let itemsJson = UTF8ToString(itemsJsonPtr);
    let nonce = UTF8ToString(noncePtr);
    let apiSecret = UTF8ToString(apiSecretPtr);

    let items = JSON.parse(itemsJson);
    let signStr = items.map(item => 
      Object.keys(item).sort().map(key => `${key}=${item[key]}`).join('&')
    ).join('&');

    let finalStr = `${signStr}&timestamp=${timestamp}&nonce=${nonce}`;

    let encoder = new TextEncoder();
    let key = encoder.encode(apiSecret);
    let msg = encoder.encode(finalStr);

    return crypto.subtle.importKey("raw", key, {name: "HMAC", hash: "SHA-256"}, false, ["sign"])
      .then(hmacKey => crypto.subtle.sign("HMAC", hmacKey, msg))
      .then(signature => {
        let hexSig = Array.from(new Uint8Array(signature)).map(b => b.toString(16).padStart(2, '0')).join('');
        return allocate(intArrayFromString(hexSig), 'i8', ALLOC_NORMAL);
      });
  },

  ACS_SendRequest: function (noncePtr, signaturePtr, bodyJsonPtr) {
    let url = "https://test4.xzsean.eu.org/acs/addDiscretionaryPointsBatch";
    let timestamp = Date.now();

    let nonce = UTF8ToString(noncePtr);
    let signature = UTF8ToString(signaturePtr);
    let bodyJson = UTF8ToString(bodyJsonPtr);

    let headers = {
      "Accept": "*/*",
      "User-Agent": "Unity WebGL",
      "x-timestamp": timestamp.toString(),
      "x-signature": signature,
      "x-nonce": nonce,
      "Content-Type": "application/json"
    };

    return fetch(url, {
      method: "POST",
      headers: headers,
      body: bodyJson
    })
    .then(response => response.text().then(text => ({status: response.status, body: text})))
    .then(result => allocate(intArrayFromString(JSON.stringify(result)), 'i8', ALLOC_NORMAL))
    .catch(error => allocate(intArrayFromString(JSON.stringify({status: 500, body: error.message})), 'i8', ALLOC_NORMAL));
  }
});
