(function () {
    'use strict';

    var tasklist = angular.module('tasklist.manage', [
        'ngAnimate',
        'ngRoute',
        'tasklist.common',
        'tasklist.tasks',
        /*'tasklist.projects',
        'tasklist.categories',
        'tasklist.users'*/
    ]);

    tasklist.config(['SignalRProvider', '$events', function (SignalRProvider, $events) {
        SignalRProvider.setHubName('liveHub');
        SignalRProvider.registerListener({ event: 'availabilityChanged', broadcast: $events.SIGNALR_AVAILABILITY_CHANGED });
    }]);
})();