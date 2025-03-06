mergeInto(LibraryManager.library, {
    SetCookie: function (key, value, days) {
        var parsedKey = UTF8ToString(key);
        var parsedValue = UTF8ToString(value);
        var parsedDays = days; // No need to stringify an integer

        var expires = "";
        if (parsedDays) {
            var date = new Date();
            date.setTime(date.getTime() + (parsedDays * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = parsedKey + "=" + encodeURIComponent(parsedValue) + expires + "; path=/";
    },

    GetCookie: function (key) {
        var parsedKey = UTF8ToString(key);

        var nameEQ = parsedKey + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(nameEQ) == 0) {
                return allocateUTF8(decodeURIComponent(c.substring(nameEQ.length, c.length)));
            }
        }
        return allocateUTF8(""); // Return an empty string if key is not found
    }
});
