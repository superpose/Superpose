var app = angular.module("superposeWebApp", [
    "ngRoute", "jsonFormatter"
]);

app.config([
    "$routeProvider", function ($routeProvider) {
        $routeProvider
            // Home
            .when("/", {
                templateUrl: "partials/home.html" /*,controller: ""*/
            })
            // else 404
            .otherwise("/404", { templateUrl: "partials/404.html", controller: "PageCtrl" });
    }
]);
angular.module("superposeWebApp").factory("endpoints", function () {
    return {
        hub: "/signalr",
        webApi: "/api/values/ActorSystemStates"
    }
});
angular.module("superposeWebApp").service("service", function ($q, $http) {
    this.POST = function (url, item) {
        var deferred = $q.defer();
        var load = JSON.stringify(item);
        $http.post(url, load, {
            headers: {
                'Content-Type': 'application/json'
            }
        }).
            success(deferred.resolve).
            error(deferred.reject);
        $rootScope.allCurrentHttpPromises.push(deferred.promise);
        return deferred.promise;

    };
    this.GET = function (url) {

        var deferred = $q.defer();
        $http({
            method: 'GET',
            url: url
        }).
            success(deferred.resolve).
            error(deferred.reject);
        $rootScope.allCurrentHttpPromises.push(deferred.promise);
        return deferred.promise;
    };
});
angular.module("superposeWebApp").factory("hub", function (endpoints, $timeout) {
    $.connection.hub.url = endpoints.hub;
    var chat = $.connection.superposeLibHub;
    return {
        ready: function (f) {
            $.connection.hub.start().done(function () {
                var arg = arguments;
                $timeout(function () {
                    f && f.apply(null, arg);
                });
            });
        },
        chat: chat,
        server: chat.server,
        client: function (name, f) {
            chat.client[name] = function (response) {
                var arg = arguments;
                $timeout(function () {
                    f && f.apply(null, arg);
                });
            };
        }
    };

});

angular.module("superposeWebApp").controller("ActorsCtrl", function ($scope, $rootScope, $http, $q, $timeout, hub) {

    $scope.statistics = {};
    $scope.paging = {};
    $scope.paging.stateType = "Queued";
    $scope.paging.take = 10;
    $scope.paging.skip = 0;
    $scope.paging.queue = "DefaultJobQueue";
    $scope.jobQueue = {};
    $scope.list = {};

    hub.client("processing", function (response) {
        $scope.jobExecuting = response;
        $timeout(function () {
            $scope.jobExecuting = "";
        }, 1000);
    });
    hub.client("currentQueue", function (response) {
        if (response) {
            $scope.jobQueue = response;
        }
    });
    hub.client("currentProcessingState", function (response) {
        $scope.stop = response;
    });
    hub.client("jobsList", function (response) {
        response = response || [];
        $scope.list.jobList = [];
        for (var i = 0; i < response.length; i++) {
            $timeout((function (i) {
                $scope.list.jobList[i] = response[i];
            })(i),1000);
        }
    });
    hub.client("jobStatisticsCompleted", function (response) {
        $scope.statistics = response || {
            TotalDeletedJobs: 0,
            TotalFailedJobs: 0,
            TotalNumberOfJobs: 0,
            TotalProcessingJobs: 0,
            TotalQueuedJobs: 0,
            TotalSuccessfullJobs: 0,
            TotalUnknownJobs: 0
        };
        //$scope.getJobs();
    });
    
    $scope.queueSampleJob = function () {
        hub.server.queueSampleJob();
    };

    $scope.getCurrentQueue = function () {
        hub.server.getCurrentQueue();
    };
    $scope.setQueueMaxNumberOfJobsPerLoad = function () {
        hub.server.setQueueMaxNumberOfJobsPerLoad($scope.jobQueue.MaxNumberOfJobsPerLoad);
    };
    $scope.setQueueStorgePollSecondsInterval = function () {
        hub.server.setQueueStorgePollSecondsInterval($scope.jobQueue.StorgePollSecondsInterval);
    };
    $scope.setQueueWorkerPoolCount = function () {
        hub.server.setQueueWorkerPoolCount($scope.jobQueue.WorkerPoolCount);
    };
    $scope.stopProcessing = function (d) {
        hub.server.stopProcessing(d);
    };
 

    $scope.getJobs = function (stateType, take, skip, queue) {
        $scope.paging.stateType = stateType || $scope.paging.stateType;
        $scope.paging.take = take || $scope.paging.take;
        $scope.paging.skip = skip || $scope.paging.skip;
        $scope.paging.queue = queue || $scope.paging.queue;

        $scope.paging.stateType === "All" ?
        hub.server.loadJobsByQueue($scope.paging.take, $scope.paging.skip, $scope.paging.queue) :
        hub.server.loadJobsByJobStateTypeAndQueue($scope.paging.stateType, $scope.paging.take, $scope.paging.skip, $scope.paging.queue);

    };
    hub.ready(function () {
        $scope.getJobs();
        hub.server.getJobStatistics();
        hub.server.getCurrentQueue();
        hub.server.getCurrentProcessingState();
    });
    $scope.updateCurrentQueue = function () {
        $scope.setQueueWorkerPoolCount();
        $scope.setQueueStorgePollSecondsInterval();
        $scope.setQueueMaxNumberOfJobsPerLoad();
    };
});