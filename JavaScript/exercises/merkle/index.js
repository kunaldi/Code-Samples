function merkle(values, hashFn) {
    var nodeMap = [];

    function getNodeMap() {
        return nodeMap;
    }

    function registerNode(k, v) {
        nodeMap.push({key: k, value: v});
    }

    function newNode(data) {
        return { data: data };
    }

    function findNode(key) {
        var index;
        var node = getNodeMap().find(function (node, idx) {
            if (node.key == key) {
                index = idx;
            }
            return index != undefined;
        });

        return node ? { index: index, node: node.value } : undefined;
    }

    function generateBreadCrumbs(node) {
        var arr = [];
        var cursor = node.pair;

        while (cursor) {
            arr.push(cursor.data);
            cursor = cursor.parent.pair;
        }

        return arr;
    }

    // takes two values and hasher, returns hashed parent obj with paired child refs
    function hashPair(lv, rv, h) {
        var parent, n1, n2;

        if (typeof lv === 'object' && typeof rv === 'object') {
            n1 = lv; n2 = rv;
            parent = newNode(h(lv.data + rv.data));
        } else {
            parent = newNode(h(lv + rv));
            n1 = newNode(lv);
            n2 = newNode(rv);
            registerNode(lv, n1);
            registerNode(rv, n2);
        }

        n1.parent = n2.parent = parent;
        n1.pair = n2;
        n2.pair = n1;
        parent.lchild = n1;
        parent.rchild = n2;
        return parent;
    }

    function merk(v, h) {
        var hashRow = [];
        var numItems = v.length;

        if (numItems % 2 != 0) {
            v.push(v[numItems - 1]);
        }

        for (var i = 0; i < numItems; i += 2) {
            var hashedObj = hashPair(v[i], v[i + 1], h); // lvalue, rvalue, hasher
            hashRow.push(hashedObj);
        }

        if (hashRow.length == 1) {
            return {
                root: hashRow[0].data,
                getVerification: getVerification
            };
        } else {
            return merk(hashRow, h);
        }

        function getVerification(key) {
            var obj = findNode(key);

            if (obj) {
                return {
                    index: obj.index,
                    breadcrumbs: generateBreadCrumbs(obj.node)
                };
            } else {
                return false;
            }
        }
    }

    return merk.call(this, values, hashFn);
}

merkle.verify = function(value, root, obj, hasher) {
    var _bitmask = 1;

    return root == obj.breadcrumbs.reduce(function (m, data) {
        m = (obj.index & _bitmask) ? data + m : m + data;
        _bitmask <<= 1;

        return hasher(m);
    }, value);
};

module.exports = merkle;