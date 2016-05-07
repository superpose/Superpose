var app = angular.module("tutorialWebApp", [
    "ngRoute"
]);

app.config([
    "$routeProvider", function($routeProvider) {
        $routeProvider
            // Home
            .when("/", { templateUrl: "partials/home.html" /*,controller: ""*/
            })
            // else 404
            .otherwise("/404", { templateUrl: "partials/404.html", controller: "PageCtrl" });
    }
]);


angular.module("tutorialWebApp").controller("ActorsCtrl", function($scope, $rootScope, $http, $q, $timeout, $interval) {
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
            $scope.jobExecuting=response;
        });
    };

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
        });
    };
    // Start the connection.
    $.connection.hub.start().done(function() {
        chat.server.getJobStatistics();
    });

    $scope.queueSampleJob = function() {
        chat.server.queueSampleJob();
    };

});