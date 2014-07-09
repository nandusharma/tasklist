(function (angular, undefined) {
    'use strict';

    var module = angular.module('tasklist.projects', [
		'ngRoute',
		'tasklist.common',
    ]);

    module.config(['$routeProvider', 'MenuProvider', function ($routeProvider, MenuProvider) {
        $routeProvider
		.when('/projects', {
		    controller: 'ProjectController',
		    templateUrl: 'WebApp/tasklist/projects/project-list.html'
		})
		.otherwise({
		    redirectTo: '/projects'
		});
    }]);

    module.controller("ProjectController", ['$scope', '$common', '$controller', ProjectController]);

    function ProjectController($scope, $common, $controller) {
        /*var Api = $common.Api;
        var User = $common.User;
        $scope.today = new Date();
        $scope.username = User.UserName;
        $common.$rootScope.$broadcast($common.$events.VIEW_CHANGE, { name: 'Dashboard', icon: 'icon-dashboard' });
        $scope.projects
        Api.Booking.getProjectsByUser({
            $expand: 'Hotel,Airport,Airline,Handler,BookingDetails/Service',
            $orderby: 'CreationDate desc',
            $top: 5
        }, function (data) {
            $scope.setData(data);
        });*/

    }
}(angular));