var createState = function () {
   return "StateValueLongersomejdjkwhljpoir3fknklncleriofohvjjkvknvklnvcdjkcdejcjkjkcedfjkjefbvcjkwvfkcn";
}

var createNounce = function () {
    return "NounceValuefjhrhjhefhheriohviorjoeojeiorjfvnk;led";
}
var signIn = function () {
    var redirectUri = "https://localhost:44337/Home/SignIn";
    var responseType = "id_token token";
    var scope = "openid ApiOne";
    var authUrl =
        "/connect/authorize/callback" +
        "?client_id=client_id_js" +
        "&redirect_uri=" + encodeURIComponent(redirectUri) +
        "&response_type=" + encodeURIComponent(responseType) +
        "&scope=" + encodeURIComponent(scope) +
        "&nonce=" + createNounce() +
        "&state=" + createState();

    var returnUrl = encodeURIComponent(authUrl);
    console.log(returnUrl);

    window.location.href = `https://localhost:44399/Auth/Login?ReturnUrl=${returnUrl}`;
}