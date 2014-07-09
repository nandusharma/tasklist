(function () {
    'use strict';

    var module = angular.module('tasklist.common', [
        'ngResource',
        'ui.bootstrap',
        /*'ui.select2'*/
    ]);

    module.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('RequestInterceptor');
    }]);

    module.constant('$events', {
        VIEW_CHANGE: 'VIEW_CHANGE',
        IMAGE_UPLOADED: 'ImageUploaded',
        SIGNALR_AVAILABILITY_CHANGED: 'SIGNALR_AVAILABILITY_CHANGED'
    });

    module.constant('$signalR', $);

    module.controller('TopNavController', ['$scope', '$events', '$location', '$user', topNavController]);
    module.controller('SidebarController', ['$scope', '$config', '$user', 'Api', sidebarController]);

    module.directive('clickToEdit', inlineEdit);
    module.directive('editFromMenu', inlineEditFromMenu);

    module.provider('Api', apiProvider);
    module.provider('Error', errorProvider);
    module.provider('SignalR', signalRProvider);

    module.service('Modal', ['$modal', modalService]);
    module.service('$common', ['$rootScope', '$events', '$config', '$location', '$timeout', '$q', '$http', '$window', 'Api', 'Error', 'Modal', '$user', commonService]);
    module.service('RequestInterceptor', ['$rootScope', '$q', '$config', requestInterceptor]);

    function _baseTableController($scope, $filter, $timeout, $config, $window) {
        $scope.data = [];
        $scope.groups = [];
        $scope.searchText = null;
        $scope.showInactive = true;
        $scope.dataReady = false;
        $scope.viewFormat = 'TABLE';

        $scope.currentOrder = null;
        $scope.currentFilter = null;
        $scope.currentGrouping = null;
        $scope.currentExpandedRow = null;

        $scope.rawData = [];

        var viewStorageKey = null;

        $scope.setData = function (data) {
            $scope.rawData = data;
            if (data) {
                applyResult();
                $scope.dataReady = true;
            } else {
                $scope.data = [];
                $scope.dataReady = false;
            }
        }

        $scope.setViewStorageKey = function (key) {
            viewStorageKey = key;
            var current = $window.localStorage.getItem(key);
            if (current && current === 'TABLE' || current === 'CARDS')
                $scope.viewFormat = current;
        }

        $scope.orderBy = function (field) {
            if ($scope.currentOrder === field) {
                field = '-' + field;
            }
            $scope.currentOrder = field;
            applyResult();
        }

        $scope.filter = function (filter) {
            $scope.currentFilter = filter;
            applyResult();
        }

        $scope.groupBy = function (field) {
            $scope.currentGrouping = field;
            applyResult();
        }

        $scope.switchViewFormat = function (format) {
            $scope.viewFormat = format;
            if (viewStorageKey)
                $window.localStorage.setItem(viewStorageKey, format);
        }

        $scope.toggleRowExpand = function (item, closeOthers) {
            if (item.$expanded) {
                item.$expanded = false;
            } else {
                if (closeOthers) {
                    angular.forEach($scope.data, function (i) {
                        i.$expanded = false;
                    });
                }
                item.$expanded = true;
            }

        };

        $scope.closeRowExpands = function () {
            angular.forEach($scope.data, function (value) {
                value.$expanded = false;
            });
        }

        var debounceDelay = null;
        $scope.$watch('searchText', function (newValue) {
            if (debounceDelay != null) {
                $timeout.cancel(debounceDelay);
            }

            debounceDelay = $timeout(function () {
                $scope.filter(newValue);
            }, $config.DEBOUNCE_INPUT_DELAY);
        });

        $scope.$watch('showInactive', function (newValue) {
            applyResult();
        });

        $scope.applyResult = applyResult;

        function applyResult() {
            $scope.closeRowExpands();

            var result = $scope.rawData;
            if (!$scope.showInactive) {
                result = result.filter(function (item) { return item.Active == true; });
            }
            if ($scope.currentFilter) {
                result = $filter('filter')(result, $scope.currentFilter);
            }
            if ($scope.currentOrder) {
                var reverse = false;
                var order = 'Id';
                if ($scope.currentOrder[0] == '-') {
                    reverse = true;
                    order = $scope.currentOrder.substring(1);
                } else {
                    order = $scope.currentOrder;
                }
                result = result.sort(function (item1, item2) {
                    var a, b;
                    if (order.indexOf('.') == -1) {
                        a = item1[order] || '';
                        b = item2[order] || '';
                    } else {
                        var firstPart = order.split('.')[0];
                        var secondPart = order.split('.')[1];
                        a = (item1[firstPart]) ? item1[firstPart][secondPart] : '';
                        b = (item2[firstPart]) ? item2[firstPart][secondPart] : '';
                    }
                    if (a == null && b == null)
                        return 0;
                    else if (a == null)
                        return -1;
                    else if (b == null)
                        return 1;
                    else if (typeof a == 'string' && typeof b == 'string') {
                        var stringA = a.toLowerCase(), stringB = b.toLowerCase()
                        if (stringA < stringB)
                            return (reverse) ? 1 : -1;
                        if (stringA > stringB)
                            return (reverse) ? -1 : 1;
                        return 0;
                    } else if (typeof a == 'number' && typeof b == 'number') {
                        if (!reverse) {
                            return a - b;
                        } else {
                            return b - a;
                        }
                    } else if (typeof a == 'boolean' && typeof b == 'boolean') {
                        if (reverse) {
                            return ((a) ? 100 : 50) - ((b) ? 100 : 50);
                        } else {
                            return ((b) ? 100 : 50) - ((a) ? 100 : 50);
                        }
                    } else {
                        return 0;
                    }
                });
            }
            if ($scope.currentGrouping) {
                $scope.groups = [];
                var initials = false;
                var datetime = false;
                var current = $scope.currentGrouping;
                if (current[0] == '-') {
                    initials = true;
                    current = current.substring(1);
                }
                if (current[0] == '*') {
                    datetime = true;
                    current = current.substring(1);
                }
                angular.forEach(result, function (item, key) {
                    var value;
                    if (current.indexOf('.') == -1) {
                        value = (item[current] || "None").toString();
                    } else {
                        var split = current.split('.');
                        var backup = {};
                        backup[split[1]] = "None"
                        value = (item[split[0]] || backup)[split[1]].toString();
                    }
                    if (initials)
                        value = value[0].toUpperCase();
                    if (datetime)
                        value = new moment(value).startOf('day').format('DD/MM/YYYY');
                    if (value == 'true')
                        value = 'True';
                    if (value == 'false')
                        value = 'False';
                    var group = $scope.groups.filter(function (g) { return g.name == value; });
                    if (group.length == 0) {
                        $scope.groups.push({
                            name: value,
                            items: [item]
                        });
                    } else {
                        group[0].items.push(item);
                    }
                });
            } else {
                $scope.groups = [{ name: 'All', items: result }];
            }
            $scope.data = result;
        };
    }

    function topNavController($scope, $events, $location, $user) {
        $scope.logout = $user.logout;
        $scope.pageTitle = "tasklist - manage your tasks with ease";
        $scope.pageIcon = "";
        $scope.username = $user.UserName;

        $scope.$on($events.VIEW_CHANGE, function (event, args) {
            $scope.pageTitle = args.name;
            $scope.pageIcon = args.icon;
        });

        /*$scope.shouldShowBack = function () {
            var loc = $location.path();
            return (loc.split('/').length > 2);
        };

        $scope.goBack = function () {
            var loc = $location.path();
            $location.path('/' + loc.split('/')[1]);
        };*/

        $scope.logout = function () {
            window.location = "/index.html";
            window.sessionStorage.clear();
        };
    }

    function sidebarController($scope, $config, $user, Api) {
        $scope.user = $user;
        $scope.selectionType = 'R';
        $scope.selectedIndex = 1;
        $scope.isNewProjectAdded = false;

        Api.Project.getProjects({ userID: $scope.user.Id}, function (data) {
            if (data != undefined) {
                $scope.projects = data;
            }
        });

        Api.Category.getCategories({}, function (data) {
            if (data != undefined) {
                $scope.categories = data;
            }
        });

        $scope.setSelected = function (type, index) {
            $scope.selectionType = type;
            $scope.selectedIndex = index;
            return true;
        };

        $scope.isSelected = function (type, index) {
            return ($scope.selectionType === type && $scope.selectedIndex === index)
        };

        $scope.addNewProject = function () {
            $scope.isNewProjectAdded = true;
            $scope.projects.push({ "Name": "My New Project", "IsDefault": "false",  "IsNew": true, "IsEdited": false, "Order": $scope.projects.length })
        };

        $scope.setProjectEditable = function () {
            //debugger;
            this.p.IsEdited = true;
        };

        $scope.saveProject = function () {
            debugger;
            if ($scope.isNewProjectAdded === true) {
                Api.Project.addProject({ userID: $user.Id, name: this.p.Name, order: this.p.Order });
                this.p.IsNew = false;
                $scope.isNewProjectAdded = false;
            } else {
                Api.Project.updateProject({ userID: $user.Id, ID: this.p.ID, name: this.p.Name });
                this.p.IsEdited = false;
            }
            //alert('saving task' + this.t.Name);
        };

        $scope.cancelProjectEdit = function () {
            this.p.IsEdited = false;
            if ($scope.isNewProjectAdded === true) {
                $scope.projects.pop();
            }
            $scope.isNewProjectAdded = false;
        };

        $scope.deleteProject = function () {
            debugger;
            if ($scope.isNewProjectAdded === true) {
                $scope.cancelProjectEdit();
            } else {
                Api.Project.deleteProject({ userID: $user.Id, projectID: this.p.ID });

                var projectToDelete;
                for (var i = $scope.projects.length - 1; i >= 0; i--) {
                    projectToDelete = $scope.projects[i];
                    if (projectToDelete.ID == this.p.ID) {
                        $scope.projects.splice(i, 1);
                    }
                }
            }
        };
    }

    function apiProvider() {
        var _endpoint = null;

        var apiProvider = {
            setEndpoint: setEndpoint,
            $get: ['$config', '$resource', '$http', getApiService]
        };

        return apiProvider;

        function setEndpoint(endpoint) {
            _endpoint = endpoint;
        }

        function getApiService($config, $resource, $http) {
            _endpoint = _endpoint || $config.API_ENDPOINT;

            var _standardUpdateProcedure = {
                method: 'PUT',
                params: {
                    id: '@Id'
                }
            };

            var project = $resource(_endpoint + '/project/:id', {}, {
                update: _standardUpdateProcedure,
                getProjects: {
                    method: 'GET',
                    url: _endpoint + '/project/getprojects/:id',
                    isArray: true
                },
                addProject: {
                    method: 'POST',
                    url: _endpoint + '/project/addproject',
                },
                updateProject: {
                    method: 'POST',
                    url: _endpoint + '/project/updateproject',
                },
                deleteProject: {
                    method: 'POST',
                    url: _endpoint + '/project/deleteproject',
                }
            });

            var task = $resource(_endpoint + '/task/:id', {}, {
                update: _standardUpdateProcedure,
                getTasks: {
                    method: 'GET',
                    url: _endpoint + '/task/gettasks/:id',
                },
                addTask: {
                    method: 'POST',
                    url: _endpoint + '/task/addtask',
                },
                updateTask: {
                    method: 'POST',
                    url: _endpoint + '/task/updatetask',
                },
                deleteTask: {
                    method: 'POST',
                    url: _endpoint + '/task/deletetask',
                }
            });

            var category = $resource(_endpoint + '/category/:id', {}, {
                update: _standardUpdateProcedure,
                getCategories: {
                    method: 'GET',
                    url: _endpoint + '/category/all',
                    isArray: true
                }
            });           

            var user = $resource(_endpoint + '/account', {}, {
                details: {
                    method: 'GET',
                    url: _endpoint + '/account/userinfo',
                },
                save: {
                    method: 'POST',
                    url: _endpoint + '/account/create'
                },
                update: _standardUpdateProcedure,
                changePassword: {
                    method: 'POST',
                    url: _endpoint + '/account/changepassword'
                },
                setPassword: {
                    method: 'POST',
                    url: _endpoint + '/account/setpassword'
                },
                lockUser: {
                    method: 'POST',
                    url: _endpoint + '/account/lockUser'
                },
                unlockUser: {
                    method: 'POST',
                    url: _endpoint + '/account/unlockUser'
                },
                setEmail: {
                    method: 'POST',
                    url: _endpoint + '/account/setEmail'
                },
                unique: {
                    method: 'GET',
                    url: _endpoint + '/account/unique',
                    isArray: false
                }
            });

            var apiService = {
                Project: project,
                Task: task,
                Category: category,
                User: user
            };

            return apiService;
        }
    }

    function errorProvider() {
        var levels = {
            NONE: 'NONE',
            ERRORS: 'ERRORS',
            WARNINGS: 'WARNINGS',
            INFOMATIVE: 'INFOMATIVE',
        };
        var consoleLogging = false;
        var consoleLoggingLevel = levels.NONE;
        var serverLogging = false;
        var serverLoggingLevel = levels.NONE;
        var serverLogPath = null;

        //#region Provider

        var errorProvider = {
            configure: configure,
            $get: ['$http', getErrorService]
        };

        return errorProvider;

        function configure(configuration) {
            consoleLogging = (configuration.CONSOLE_LOGGING || false),
            consoleLoggingLevel = (configuration.CONSOLE_LOG_LEVEL || levels.NONE),
            serverLogging = (configuration.SERVER_LOGGING || false),
            serverLoggingLevel = (configuration.SERVER_LOG_LEVEL || levels.NONE),
	        serverLogPath = (configuration.SERVER_LOG_PATH || null)
        }

        //#endregion

        //#region Service

        function getErrorService($http) {
            var menuService = {
                levels: levels,
                error: error,
                warning: warning,
                info: info
            };

            return menuService;

            function error() {
                console.log(arguments);
            }

            function warning() {
                console.log(arguments);
            }

            function info() {
                console.log(arguments);
            }
        }

        //#endregion
    }

    function signalRProvider() {
        var hubName = '';
        var onStart = function () { console.log('SignalR Started'); };
        var onStop = function () { };
        var onError = function () { };
        var onSlow = function () { };
        var onLost = function () { };
        var onReconnect = function () { };

        var listeners = [];

        var signalRProvider = {
            setHubName: setHubName,
            registerListener: registerListenerBefore,
            $get: ['$rootScope', '$events', '$signalR', '$config', getSignalRService]
        }

        function setHubName(value) {
            hubName = value;
        };

        function registerListenerBefore(settings) {
            listeners.push(settings);
        }

        return signalRProvider;

        function getSignalRService($rootScope, $events, $signalR, $config) {
            var signalRService = {
                hub: null,
                hubName: hubName,
                registerListener: registerListenerAfter,
                running: false,
                initalise: initalise,
                dispose: dispose
            };

            function initalise(callback) {
                $signalR.connection.hub.url = $config.RESOURCE_HOST + '/signalr';
                signalRService.hub = $signalR.connection[signalRService.hubName];
                var client = signalRService.hub.client;
                angular.forEach(listeners, function (l) {
                    if (l.function) {
                        client[l.event] = l.function;
                    } else if (l.broadcast) {
                        client[l.event] = function () {
                            $rootScope.$broadcast(l.broadcast, arguments);
                        }
                    } else {
                        client[l.event] = function () {
                            $rootScope.$broadcast(l.event, arguments);
                        }
                    }
                });
                $signalR.connection.hub.start(function () {
                    signalRService.running = true;
                    $rootScope.$broadcast($events.SIGNALR_STARTED);
                    onStart();
                    callback();
                });
            };

            function dispose(callback) {
                signalRService.running = false;
                $signalR.connection.hub.stop(function () {
                    $rootScope.$broadcast($events.SIGNALR_STOPPED);
                    onStop();
                    callback();
                });
            }

            function registerListenerAfter(settings) {
                if (signalRService.running)
                    throw new Error('SignalR Service: Cannot add listener after initialization.');
                listeners.push(settings);
            }

            return signalRService;
        }
    }

    function modalService($modal) {
        var modalService = {
            displayMessage: displayMessage,
            deleteConfirmModal: deleteConfirmModal,
            openModalForResult: openModalForResult
        }

        return modalService;

        function displayMessage(title, message) {
            var modal = $modal.open({
                templateUrl: 'WebApp/common/modals/simple-message-modal.html',
                controller: 'SimpleMessageModalController',
                resolve: {
                    modalConfig: function () {
                        return {
                            title: title,
                            message: message
                        };
                    }
                }
            });
            return modal.result;
        }

        function deleteConfirmModal(message) {
            var modal = $modal.open({
                templateUrl: 'WebApp/common/modals/delete-confirm-modal.html',
                controller: 'DeleteConfirmModalController',
                keyboard: false,
                backdrop: 'static',
                resolve: {
                    modalConfig: function () {
                        return {
                            message: message
                        };
                    }
                }
            });
            return modal.result;
        }

        function openModalForResult(config, passthrough) {
            config.resolve = {
                modalConfig: function () {
                    return passthrough;
                }
            }
            var modal = $modal.open(config);
            return modal.result;
        }
    }

    function commonService($rootScope, $events, $config, $location, $timeout, $q, $http, $window, Api, Error, Modal, User) {
        return {
            $rootScope: $rootScope,
            $events: $events,
            $config: $config,
            $location: $location,
            $timeout: $timeout,
            $q: $q,
            $http: $http,
            $window: $window,
            Api: Api,
            Error: Error,
            Modal: Modal,
            User: User
        };
    }

    function requestInterceptor($rootScope, $q, $config) {
        return {
            request: function (config) {
                config.headers = config.headers || {};
                config.headers.Authorization = "Bearer " + $config.ACCESS_TOKEN;
                return config || $q.when(config);
            },
            responseError: function (response) {
                if (response.status === 401) {

                } else if (response.status === 400) {

                } else if (response.status === 500) {

                }
                return response || $q.reject(response);
            }
        }
    }
    
    function inlineEdit() {
        var editorTemplate = '<div>' +
            '<div ng-hide="view.editorEnabled" ng-click="enableEditor()">' +
                '{{value}} ' +
            '</div>' +
            '<div ng-show="view.editorEnabled">' +
                '<input id="taskEditor" type="text" ng-model="view.editableValue">' +
                '<a ng-click="save()"><span class="glyphicon glyphicon-ok"></a>' +
                '<a ng-click="disableEditor()"><span class="glyphicon glyphicon-remove"></a>' +
            '</div>' +
        '</div>';
        return {
            restrict: "A",
            replace: true,
            template: editorTemplate,
            scope: {
                value: "=clickToEdit",
                saveCallback: "&saveFunction",
                cancelCallback: "&cancelFunction"
            },
            link: function (scope, elem, attrs) {
                attrs.$observe('newitem', function (value) {
                    if (value == "true") {
                        scope.enableEditor();
                    }
                });
                attrs.$observe('isbeingedited', function (value) {
                    if (value == "true") {
                        scope.enableEditor();
                    }
                });
            },
            controller: function ($scope, $timeout) {
                $scope.view = {
                    previousValue: $scope.value,
                    editableValue: $scope.value,
                    editorEnabled: false
                };

                $scope.enableEditor = function () {
                    $scope.view.editorEnabled = true;
                    $scope.view.editableValue = $scope.value;
                };

                $scope.closeEditor = function () {
                    $scope.view.editorEnabled = false;
                };

                $scope.disableEditor = function () {
                    $scope.value = $scope.view.previousValue;
                    $scope.closeEditor();
                    $timeout(function () {
                        $scope.cancelCallback();
                    });
                };

                $scope.save = function () {
                    $scope.value = $scope.view.editableValue;
                    $scope.closeEditor();
                    $timeout(function () {
                        $scope.saveCallback();
                    });
                };
            }
        }
    }

    function inlineEditFromMenu() {
        var editorTemplate = '<div class="inlineeditpanel">' +
            '<div ng-hide="view.editorEnabled">' +
                '{{value}} ' +
            '</div>' +
            '<div ng-show="view.editorEnabled">' +
                '<input id="taskEditor" type="text" ng-model="view.editableValue">' +
                '<a ng-click="save()"><span class="glyphicon glyphicon-ok"></a>' +
                '<a ng-click="disableEditor()"><span class="glyphicon glyphicon-remove"></a>' +
            '</div>' +
        '</div>';
        return {
            restrict: "A",
            replace: true,
            template: editorTemplate,
            scope: {
                value: "=editFromMenu",
                saveCallback: "&saveFunction",
                cancelCallback: "&cancelFunction"
            },
            link: function (scope, elem, attrs) {
                attrs.$observe('newitem', function (value) {
                    if (value == "true") {
                        scope.enableEditor();
                    }
                });
                attrs.$observe('isbeingedited', function (value) {
                    if (value == "true") {
                        scope.enableEditor();
                    }
                });
            },
            controller: function ($scope, $timeout) {
                $scope.view = {
                    previousValue: $scope.value,
                    editableValue: $scope.value,
                    editorEnabled: false
                };

                $scope.enableEditor = function () {
                    $scope.view.editorEnabled = true;
                    $scope.view.editableValue = $scope.value;
                };

                $scope.closeEditor = function () {
                    $scope.view.editorEnabled = false;
                };

                $scope.disableEditor = function () {
                    $scope.value = $scope.view.previousValue;
                    $scope.closeEditor();
                    $timeout(function () {
                        $scope.cancelCallback();
                    });
                };

                $scope.save = function () {
                    $scope.value = $scope.view.editableValue;
                    $scope.closeEditor();
                    $timeout(function () {
                        $scope.saveCallback();
                    });
                };
            }
        }
    }

})();