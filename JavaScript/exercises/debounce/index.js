module.exports = function debounce(callback, delay) {
    var timeoutID;

    return function() {
        var context = this, args = arguments;

        clearTimeout(timeoutID);
        timeoutID = setTimeout(function () {
            callback.apply(context, args);
        }, delay);
    };
};