//see more: https://github.com/IdentityModel/oidc-client-js/wiki
var config = {
    // userStore :  optional -  for storing callback value in local storage by default it is store in session storage
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }), // tell from where to read
    authority: "https://localhost:44399/",
    client_id: "client_id_oidc_js",
    redirect_uri: "https://localhost:44337/Home/SignIn",
    post_logout_redirect_uri: "https://localhost:44337/Home/Index",
    response_type: "id_token token",
    scope: "openid custom_claims ApiOne"
}

var userManager = new Oidc.UserManager(config);

var signIn = function () {

    userManager.signinRedirect();
}

var signOut = function () {
    userManager.signoutRedirect();
}
userManager.getUser().then(user => {
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
});

var callApi = function () {
    axios.get("https://localhost:44324/secret").then(results => {
        console.log(results);
    })
}
var refreshing = false;
axios.interceptors.response.use(
    function (response) {
        return response;
    },
    function (error) {
        console.log(error.response);
        var axiosConfig = error.response.config;
        //if error status is 401 try to refresh token
        if (error.response.status === 401) {

            if (!refreshing) {
                refreshing = true;
                //do the refresh
                console.log("Starting Refreshing Token");
                userManager.signinSilent().then(user => {
                    console.log("New Users",user);
                    //update the http request and clients
                    axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
                    axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;
                    //retry Http request
                    axios(axiosConfig)
                    refreshing = false;

                });
            }

        }
        return Promise.reject(error);
    }
);