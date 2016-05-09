var app = angular.module("superposeWebApp", [
    "ngRoute"
]);

app.config([
    "$routeProvider", function($routeProvider) {
        $routeProvider
            // Home
            .when("/", {
                templateUrl: "partials/home.html" /*,controller: ""*/
            })
            // else 404
            .otherwise("/404", { templateUrl: "partials/404.html", controller: "PageCtrl" });
    }
]);


angular.module("superposeWebApp").controller("ActorsCtrl", function($scope, $rootScope, $http, $q, $timeout, $interval) {
    var endpoint = "/api/values/ActorSystemStates";
    var getThings = function(selection) {
        var deferred = $q.defer();
        $http.get(endpoint, JSON.stringify(selection), { headers: { 'Content-Type': "application/json" } }).success(deferred.resolve).error(deferred.reject);

        return deferred.promise;
    };

    $scope.statictics = {};

    $scope.stringify = function(data) {
        return JSON.stringify(data);
    };
    $scope.actors = [];


    $scope.actorState = { state: {} };

    $.connection.hub.url = "http://localhost:8008/signalr";
    var chat = $.connection.myHub;
    chat.client.processing = function(response) {
        $timeout(function() {
            $scope.jobExecuting = response;
            $timeout(function () {
                $scope.jobExecuting = "";
            }, 1000);
        });
    };
    $scope.paging = {};
    $scope.paging.stateType = 'Queued';
    $scope.paging.take = 10;
    $scope.paging.skip = 0;
    $scope.paging.queue = 'DefaultJobQueue';
    chat.client.jobStatisticsCompleted = function(response) {
        $timeout(function() {
            $scope.statictics = response || {
                TotalDeletedJobs: 0,
                TotalFailedJobs: 0,
                TotalNumberOfJobs: 0,
                TotalProcessingJobs: 0,
                TotalQueuedJobs: 0,
                TotalSuccessfullJobs: 0,
                TotalUnknownJobs: 0
            };

            $scope.getJobsByJobStateType($scope.paging.stateType, $scope.paging.take, $scope.paging.skip, $scope.paging.queue);
        });
    };
    // Start the connection.
    $.connection.hub.start().done(function () {
            $scope.getJobsByJobStateType($scope.paging.stateType, $scope.paging.take, $scope.paging.skip, $scope.paging.queue);

        chat.server.getJobStatistics();
        chat.server.getCurrentQueue();
        chat.server.getCurrentProcessingState();
   
    });
    $scope.jobQueue = {};
    $scope.queueSampleJob = function() {
        chat.server.queueSampleJob();
        window.location.reload(false);
    };

    chat.client.currentQueue = function(response) {
        $timeout(function () {
            if (response) {
                  $scope.jobQueue = response;
            }
           
          
        });
    };
    $scope.getCurrentQueue = function() {
        chat.server.getCurrentQueue();
    };

    $scope.setQueueMaxNumberOfJobsPerLoad = function() {
        chat.server.setQueueMaxNumberOfJobsPerLoad($scope.jobQueue.MaxNumberOfJobsPerLoad);
    };
    $scope.setQueueStorgePollSecondsInterval = function() {
        chat.server.setQueueStorgePollSecondsInterval($scope.jobQueue.StorgePollSecondsInterval);
    };
    $scope.setQueueWorkerPoolCount = function() {
        chat.server.setQueueWorkerPoolCount($scope.jobQueue.WorkerPoolCount);
    };
    $scope.updateCurrentQueue = function() {
        $scope.setQueueWorkerPoolCount();
        $scope.setQueueStorgePollSecondsInterval();
        $scope.setQueueMaxNumberOfJobsPerLoad();
    };

    $scope.list = { jobList :[]};
    chat.client.jobsList = function (response) {
        $timeout(function () {
                $scope.list.jobList = response||[];
        });
    };
    $scope.stopProcessing = function(d) {
        chat.server.stopProcessing(d);
    };
    chat.client.currentProcessingState = function (response) {
        $timeout(function () {
            $scope.stop = response;
        });
    };
    $scope.getJobsByJobStateType = function (stateType, take, skip, queue) {
        $scope.paging.stateType = stateType;
        $scope.paging.take = take;
        $scope.paging.skip = skip;
        $scope.paging.queue = queue;
        chat.server.getJobsByJobStateType($scope.paging.stateType, $scope.paging.take, $scope.paging.skip, $scope.paging.queue);
    };


});