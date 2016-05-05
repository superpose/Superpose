var app = angular.module('tutorialWebApp', [
      'ngRoute'
]);

app.config([
    '$routeProvider', function ($routeProvider) {
        $routeProvider
            // Home
            .when("/", { templateUrl: "partials/home.html", controller: "ActorsCtrl" })
            // Pages
            .when("/about", { templateUrl: "partials/about.html", controller: "PageCtrl" })
            .when("/faq", { templateUrl: "partials/faq.html", controller: "PageCtrl" })
            .when("/pricing", { templateUrl: "partials/pricing.html", controller: "PageCtrl" })
            .when("/services", { templateUrl: "partials/services.html", controller: "PageCtrl" })
            .when("/contact", { templateUrl: "partials/contact.html", controller: "PageCtrl" })
            // Blog
            .when("/blog", { templateUrl: "partials/blog.html", controller: "BlogCtrl" })
            .when("/blog/post", { templateUrl: "partials/blog_item.html", controller: "BlogCtrl" })
            // else 404
            .otherwise("/404", { templateUrl: "partials/404.html", controller: "PageCtrl" });
    }
]);


angular.module('tutorialWebApp').factory("Grapgher", function() {
    
 

    function Run(o) {
        try {
          console.log(o);
        } catch (e) {
            console.warn(e);
        }
    }


    function Refresh() {
        console.log("Refreshed");
    };


    return {
        Run: Run,
        Regresh: Refresh
    };
});

angular.module('tutorialWebApp').controller('ActorsCtrl', function ($scope, $rootScope, $http, $q, $timeout, $interval, Grapgher) {
    var endpoint = '/api/values/ActorSystemStates';
    var getThings = function (selection) {
        var deferred = $q.defer();
        $http.get(endpoint, JSON.stringify(selection), { headers: { 'Content-Type': 'application/json' } }).success(deferred.resolve).error(deferred.reject);

        return deferred.promise;
    };
    $scope.stringify = function (data) {
        return JSON.stringify(data);
    };
    $scope.actors = [];
   
    var actorVals = {};
    $scope.actorState = { state: {} };

    $.connection.hub.url = "http://localhost:8008/signalr";
    var chat = $.connection.myHub;
    chat.client.addMessage = function ( response) {
        $timeout(function() {
            Grapgher.Run(response);
        });
    };
    // Start the connection.
    $.connection.hub.start().done(function () {
        // getActorSystemStates();
    });

});

