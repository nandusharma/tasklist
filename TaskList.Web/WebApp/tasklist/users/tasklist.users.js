(function (angular, undefined) {
    'use strict';

    var module = angular.module('tasklist.users', [
        'ngRoute',
        'tasklist.common',
    ]);

    /*module.config(['$routeProvider', 'MenuProvider', '$user', function ($routeProvider, MenuProvider, $user) {

        $routeProvider
            .when('/settings', {
                controller: 'HotelSettingsController',
                templateUrl: 'WebApp/hotels/settings/hotels-settings.html'
            })

        MenuProvider.addMenuItem({
            title: 'Settings',
            path: '/settings',
            icon: 'icon-cogs',
            group: 'Admin Menu'
        });
    }]);

    module.controller("HotelSettingsController", ['$scope', '$user', '$common', HotelSettingsController]);
    module.controller("HotelsContractController", ['$scope', '$controller', '$user', '$common', HotelsContractController]);
    module.controller("HotelsUsersController", ['$scope', '$controller', '$user', '$common', HotelsUsersController]);

    function HotelSettingsController($scope, $user, $common) {
        var $events = $common.$events;

        $common.$rootScope.$broadcast($events.VIEW_CHANGE, { name: 'Settings', icon: 'icon-cogs' });
        $scope.tabs = {
            current: 'USERS'
        }
    }

    function HotelsContractController($scope, $controller, $user, $common) {
        var Api = $common.Api;
        var Modal = $common.Modal;
        $controller('_baseTableController', { $scope: $scope });

        $scope.selectConfig = {
            allowClear: true,
            placeholder: 'Select..'
        };

        $scope.orderBy('Hotel.Name');

        function refetch() {
            Api.ContractedRate.query({
                $filter: 'HotelId eq ' + $user.HotelId,
                $expand: 'Airline,RoomType'
            }, function (data) {
                $scope.setData(data);
            }, function () {
            });
        }

        refetch();
    }

    function HotelsUsersController($scope, $controller, $user, $common) {
        var Api = $common.Api;
        var Modal = $common.Modal;
        var $timeout = $common.$timeout;
        $controller('_baseTableController', { $scope: $scope });

        refetch();

        $scope.currently = {};
        $scope.editUser = editUser;
        $scope.createUser = createUser;

        function refetch() {
            Api.User.query({
                $filter: 'HotelId eq ' + $user.HotelId
            }, function (data) {
                $scope.setData(data);
            }, function () {

            });
        }

        function editUser(user) {
            Modal.openModalForResult({
                templateUrl: 'WebApp/common/modals/user-edit-modal.html',
                controller: 'UserEditModalController',
                keyboard: false,
                backdrop: 'static'
            }, {
                Id: user.Id,
                UserTypes: [{ Value: 'Hotel', Title: 'Basic User' }, { Value: 'HotelAdmin', Title: 'Admin User' }],
                HotelId: $user.HotelId
            }).then(function () {
                refetch();
            }, function () {
                refetch();
            });
        }

        function createUser() {
            Modal.openModalForResult({
                templateUrl: 'WebApp/common/modals/user-edit-modal.html',
                controller: 'UserCreateModalController',
                keyboard: false,
                backdrop: 'static'
            }, {
                UserTypes: [{ Value: 'Hotel', Title: 'Basic User' }, { Value: 'HotelAdmin', Title: 'Admin User' }],
                AirlineId: $user.HotelId
            }).then(function () {
                refetch();
            }, function () {
                refetch();
            });
        }
    }*/

}(angular));