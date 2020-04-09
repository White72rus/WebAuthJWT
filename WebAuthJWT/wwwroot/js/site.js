"use strict";

(function () {
    let cookies = document.cookie;
    console.dir(cookies);
    //alert("Run script");
})();

let tokenKey = "tokenKey";

async function getTokenAsync(token) {
    const tokenKey = token;
    const formData = new FormData();
    const login = document.querySelector("#login").value;
    const pass = document.querySelector("#pass").value;
    formData.append("grant_type", "password");
    formData.append("username", login);
    formData.append("password", pass);

    const response = await fetch("/token", {
        method: "POST",
        headers: { "Accept" : "application/json" },
        body: formData
    });

    const data = await response.json();

    if (response.ok === true) {
        //sessionStorage.setItem(tokenKey, data.access_token);
        //console.dir(data.access_token);
        document.location = "/";
    } else {
        console.log("Error: ", response.status, data.erorrText);
    }
}

async function getData(url, _token) {
    const tokenKey = _token;
    //const token = sessionStorage.getItem(tokenKey);

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "application/json",
        }
    });
    if (response.ok == true) {
        const data = await response.text();
        if (data != null) {
            document.write(data);
        }
    } else {
        if (response.status == "401") {
            console.log("Error: ", response.status, "Unautorise");
        } 
    }
}

function setCookies(name, value, _options = {}) {
    let options = {
        path: "/",
        samesite: "lax",
        domain: "localhost",
    };

    if (_options != null && len(_options)) {
        for (let key in _options) {
            options[key] = _options[key];
        }
    }
    let cookiesData = name + "=" + value;
    for (let opt in options) {
        let optValue = options[opt];
        cookiesData += "; " + opt + "=" + optValue;
    }
    document.cookie = cookiesData;
}

// Path: / AUTH
if (document.location.pathname.toLowerCase() == "/auth") {
    document.querySelector("#submit").addEventListener("click", function (e) {
        getTokenAsync(tokenKey);
    });
}

// Path: / (HOME)
if (document.location.pathname == "/") {
    document.querySelector("#to_privacy").addEventListener("click", function (e) {
        getData("/home/privacy", tokenKey);
    });
    document.querySelector("#set_cookie").addEventListener("click", function (e) {
        setCookies("MyCook", "666", {});
    });
}

/**
 * Return of object length.
 * @param {any} object
 */
function len(object) {
    return Object.keys(object).length;
}






