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
    module.controller('SidebarController', ['$scope', '$config', '$user', 'Api', 'Modal', sidebarController]);
    module.controller('ShareProjectModalController', ['$scope', '$modalInstance', 'modalConfig', 'Api', '$http', shareProjectModalController]);

    module.directive('clickToEdit', inlineEdit);
    module.directive('editFromMenu', inlineEditFromMenu);

    module.provider('Api', apiProvider);
    module.provider('Error', errorProvider);
    module.provider('SignalR', signalRProvider);

    module.service('Modal', ['$modal', modalService]);
    module.service('$common', ['$rootScope', '$events', '$config', '$location', '$timeout', '$q', '$http', '$window', 'Api', 'Error', 'Modal', '$user', commonService]);
    module.service('RequestInterceptor', ['$rootScope', '$q', '$config', requestInterceptor]);
    
    function topNavController($scope, $events, $location, $user) {
        $scope.logout = $user.logout;
        $scope.pageTitle = "tasklist - manage your tasks with ease";
        $scope.pageIcon = "";
        $scope.username = $user.UserName;

        $scope.$on($events.VIEW_CHANGE, function (event, args) {
            $scope.pageTitle = args.name;
            $scope.pageIcon = args.icon;
        });

        $scope.logout = function () {
            window.location = "/index.html";
            window.sessionStorage.clear();
        };
    }

    function sidebarController($scope, $config, $user, Api, Modal, $http) {
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
            $scope.projects.push({ "Name": "My New Project", "IsDefault": "false", "IsShared": false, "IsNew": true, "IsEdited": false, "Order": $scope.projects.length })
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

        $scope.openShareProjectModal = openShareProjectModal;

        function openShareProjectModal(project) {            
            Modal.shareProject(project).then(function (result) {
                debugger; var test = "";
            });
        }
    }

    function shareProjectModalController($scope, $modalInstance, modalConfig, Api, $http) {
        $scope.project = modalConfig.project;
        $scope.selected = "";
        $scope.users = [];
        $scope.selectedUsers = []

        Api.User.getAllUsers({}, function (data) {
            $scope.users = [];
                angular.forEach(data, function (item) {
                    $scope.users.push(item.UserName);
                });
                return $scope.users;
            }, function (exception) {
            //Error.error(exception);
            });

        $scope.addSelectedUser = function ($item, $model, $label) {
            if ($label != undefined && $label.length > 0) {
                $scope.selectedUsers.push($label);
                $label = "";
            }
        }

        $scope.removeUser = function () {
            var user = this.user;
            for (var i = $scope.selectedUsers.length-1; i > 0; i--) {
                if ($scope.selectedUsers[i] === user) {
                    $scope.selectedUsers.splice(i,1);
                }
            }
        }

        //$scope.getUsers = function (val) {
            
        //    return Api.User.getAllUsers({ userID: val }, function (data) {
        //        var users = [];
        //        angular.forEach(data, function (item) {
        //            users.push(item.UserName);
        //        });
        //        return users;
        //    }, function (exception) {
        //        //Error.error(exception);
        //    });
        //    ///*return $http.get('http://localhost:51111/api/account/getallusers', {
        //    //    params: {
        //    //        userID: val,
        //    //    }
        //    //}).then(function (res) {
        //    //    var users = [];
        //    //    angular.forEach(res, function (item) {
        //    //        users.push(item.UserName);
        //    //    });
        //    //    return users;
        //    //});*/
        //};

        $scope.share = function () {
            $modalInstance.close($scope.selectedUsers);
            //return $scope.selectedUsers;
        };

        $scope.cancel = function () {
            $modalInstance.dismiss(false);
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
                getAllUsers: {
                    method: 'GET',
                    url: _endpoint + '/account/getallusers',
                    isArray: true
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
            /*displayMessage: displayMessage,
            deleteConfirmModal: deleteConfirmModal,
            openModalForResult: openModalForResult,*/
            shareProject : shareProject
        }

        return modalService;

        function shareProject(project) {
            var modal = $modal.open({
                templateUrl: 'WebApp/common/modals/share-project-modal.html',
                controller: 'ShareProjectModalController',
                keyboard: false,
                backdrop: 'static',
                resolve: {
                    modalConfig: function () {

                        return {
                            project: project
                        };
                    }
                }
            });
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