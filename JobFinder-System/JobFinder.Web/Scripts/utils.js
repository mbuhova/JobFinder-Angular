window.util = window.util || {
    toJS: function (json) {
        return JSON.parse(JSON.stringify(json));
    }
};
