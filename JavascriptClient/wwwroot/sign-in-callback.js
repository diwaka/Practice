var Token = function (address) {
    var returnValue = address.split("#")[1];
    var values = returnValue.split("&");

    for (var i = 0; i < values.length; i++) {
        var keyValuePair = values[i].split("=");
        localStorage.setItem(keyValuePair[0], keyValuePair[1]);
    }

    window.location.href = "/Home/Index";
}

Token(window.location.href);