module.exports = function throttlePromises(numConcurrent, arrFn) {
    var promiseIndex = 0;
    var activePromises = [];
    var numPromises = arrFn.length;
    var results = new Array(numPromises);

    function resolveNext() {
        if (promiseIndex < numPromises) {
            var _index = promiseIndex;
            return arrFn[promiseIndex++]().then(function (result) {
                results[_index] = result;
            }).then(resolveNext);
        }
    }

    while (promiseIndex < numConcurrent) {
        activePromises.push(resolveNext());
    }

    return Promise.all(activePromises).then(function () {
        return results;
    });
};