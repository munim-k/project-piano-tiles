mergeInto(LibraryManager.library, {
    SetCookie: function (key, value, days) {
        var parsedDays = Pointer_stringify(days);
        var parsedKey = Pointer_stringify(key);
        var parsedValue = Pointer_stringify(value);

        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (parsedDays * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = parsedKey + "=" + encodeURIComponent(parsedValue) + expires + "; path=/";
    },

    GetCookie: function (key) {
        var parsedKey = Pointer_stringify(key);

        var nameEQ = parsedKey + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(nameEQ) == 0) {
                return decodeURIComponent(c.substring(nameEQ.length, c.length));
            }
        }
        return "";
    }
});
