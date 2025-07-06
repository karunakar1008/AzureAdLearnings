window.tokenHelper = {
    //getIdToken: function (key) {
    //    return sessionStorage.getItem(key);
    //},
    getAllSessionStorageKeys: function () {
        return Object.keys(sessionStorage);
    },
     getIdToken: function () {
        const keys = Object.keys(sessionStorage);
        const idTokenKey = keys.find(k => k.includes('idtoken'));
        return idTokenKey ? sessionStorage.getItem(idTokenKey) : null;
    }
};
