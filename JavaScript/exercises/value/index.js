module.exports = function value(e) {
    return typeof e === 'function' ? value(e()) : e;
};
