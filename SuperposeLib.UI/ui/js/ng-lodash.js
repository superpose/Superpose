﻿(function(ng, _) {
    "use strict";
    var underscoreModule = ng.module("angular-underscore", []), utilsModule = ng.module("angular-underscore/utils", []), filtersModule = ng.module("angular-underscore/filters", []);

    function propGetterFactory(prop) { return function(obj) { return obj[prop] } }

    _._ = _;
    _.each(["min", "max", "sortedIndex"], function(fnName) {
        _[fnName] = _.wrap(_[fnName], function(fn) {
            var args = _.toArray(arguments).slice(1);
            if (_.isString(args[2])) {
                args[2] = propGetterFactory(args[2])
            } else if (_.isString(args[1])) {
                args[1] = propGetterFactory(args[1])
            }
            return fn.apply(_, args)
        })
    });
    ng.injector(["ng"]).invoke(["$filter", function($filter) {
            _.filter = _.select = _.wrap($filter("filter"), function(filter, obj, exp) {
                if (!_.isArray(obj)) {
                    obj = _.toArray(obj)
                }
                return filter(obj, exp)
            });
            _.reject = function(obj, exp) {
                if (_.isString(exp)) {
                    return _.filter(obj, "!" + exp)
                }
                var diff = _.bind(_.difference, _, obj);
                return diff(_.filter(obj, exp))
            }
        }
    ]);
    _.each(_.methods(_), function(methodName) {
        function register($rootScope) { $rootScope[methodName] = _.bind(_[methodName], _) }

        _.each([underscoreModule, utilsModule, ng.module("angular-underscore/utils/" + methodName, [])], function(module) { module.run(["$rootScope", register]) })
    });
    var adapList = [["map", "collect"], ["reduce", "inject", "foldl"], ["reduceRight", "foldr"], ["find", "detect"], ["filter", "select"], "where", "findWhere", "reject", "invoke", "pluck", "max", "min", "sortBy", "groupBy", "countBy", "shuffle", "toArray", "size", ["first", "head", "take"], "initial", "last", ["rest", "tail", "drop"], "compact", "flatten", "without", "union", "intersection", "difference", ["uniq", "unique"], "zip", "object", "indexOf", "lastIndexOf", "sortedIndex", "keys", "values", "pairs", "invert", ["functions", "methods"], "pick", "omit", "tap", "identity", "uniqueId", "escape", "result", "template"];
    _.each(adapList, function(filterNames) {
        if (!_.isArray(filterNames)) {
            filterNames = [filterNames]
        }
        var filter = _.bind(_[filterNames[0]], _), filterFactory = function() { return filter };
        _.each(filterNames, function(filterName) { _.each([underscoreModule, filtersModule, ng.module("angular-underscore/filters/" + filterName, [])], function(module) { module.filter(filterName, filterFactory) }) })
    })
})(angular, _);