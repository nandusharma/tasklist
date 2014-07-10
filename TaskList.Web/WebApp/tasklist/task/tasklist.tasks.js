(function (angular, undefined) {
    'use strict';

    var module = angular.module('tasklist.tasks', [
        'ngRoute',
        'tasklist.common',
    ]);

    module.config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('/tasks', {
                controller: 'TasksController',
                templateUrl: 'WebApp/tasklist/task/tasklist.html'
            })
            .otherwise({
                redirectTo: '/tasks'
            });
    }]);

    /*module.directive('onEsc', onEscapeKeyPress);
    module.directive('onEnter', onEnterKeyPress);
    module.directive('inlineEdit', inlineEdit);
    
    function onEscapeKeyPress() {
        return {
            link: function ($scope, elm, attr) {
                elm.bind('keydown', function (e) {
                    alert('12');
                    if (e.keyCode === 27) {
                        $scope.$apply(attr.onEsc);
                    }
                });
            }
        };
    }

    function onEnterKeyPress() {
        return {
            link: function ($scope, elm, attr) {
                elm.bind('keypress', function (e) {
                    alert('11');
                    if (e.keyCode === 13) {
                        $scope.$apply(attr.onEnter);
                    }
                });
            }
        };
    }

    function inlineEdit($timeout) {
        return {
            scope: {
                model: '=inlineEdit',
                handleSave: '&onSave',
                handleCancel: '&onCancel'
            },
            link: function ($scope, elm, attr) {
                var previousValue;

                $scope.edit = function () {
                    alert('1');
                    $scope.editMode = true;
                    previousValue = $scope.model;

                    $timeout(function () { elm.find('input')[0].focus; }, 0, false);
                };

                $scope.save = function () {
                    $scope.editModel = false;
                    $scope.handleSave({ value: $scope.model });
                };

                $scope.cancel = function () {
                    $scope.editMode = false;
                    $scope.model = previousValue;
                    $scope.handleCancel({ value: $scope.model });
                };
            }
        };
    }*/

    module.controller("TasksController", ['$scope', '$common', '$user', '$routeParams', 'SignalR', tasksController]);

    //module.directive('clickToEdit', inlineEdit);

    function tasksController($scope, $common, $user, $routeParams, SignalR) {
        var Api = $common.Api;
        var Error = $common.Error;
        var $events = $common.$events;
        var $config = $common.$config;
        $scope.baseUrl = $config.API_ENDPOINT;
        $scope.defaultPath = $config.RESOURCE_HOST;

        $scope.selectedTaskCount = 0;
        $scope.isDefaultProject = false;
        $scope.isNewTaskAdded = false;

        $scope.projectID = 0;
        if ($routeParams.project != undefined) {
            $scope.projectID = parseInt($routeParams.project);
        }
        Api.Task.getTasks({ userId: $user.Id, projectID: $scope.projectID }, function (data) {
            $scope.projectName = data.ProjectName;
            $scope.tasks = data.Tasks;
            $scope.isDefaultProject = data.IsDefaultProject;
        }, function () {
        });

        $scope.addNewTask = function () {
            $scope.isNewTaskAdded = true;
            $scope.tasks.push({ "Name": "Enter new task description...", "ProjectID": $scope.projectID, "IsNew": true })
        }

        $scope.selectTask = function () {
            var selectedTask;
            for (var i = 0; i < $scope.tasks.length; i++) {
                selectedTask = $scope.tasks[i];
                if (selectedTask.ID == this.t.ID) {
                    selectedTask.IsSelected = !selectedTask.IsSelected;
                }
            }
        };

        $scope.saveTask = function () {
            if ($scope.isNewTaskAdded === true) {
                Api.Task.addTask({ userID: $user.Id, projectID: $scope.projectID, name: this.t.Name, priority: 4, categoryID: 1 });
                $scope.isNewTaskAdded = false;
            } else {
                Api.Task.updateTask({ userID: $user.Id, taskID: this.t.ID, projectID: $scope.projectID, name: this.t.Name, priority: 4, categoryID: 1 });
            }           
            //alert('saving task' + this.t.Name);
        };

        $scope.cancelTask = function () {
            if ($scope.isNewTaskAdded === true) {
                $scope.tasks.pop();
            }
            $scope.isNewTaskAdded = false;
            //alert('cancelling task');
        };

        $scope.deleteTask = function () {
            debugger;
            if ($scope.isNewTaskAdded === true) {
                $scope.cancelTask();
            } else {
                Api.Task.deleteTask({ userID: $user.Id, taskID: this.t.ID, projectID: $scope.projectID });
                
                var taskToDelete;
                for (var i = $scope.tasks.length-1; i >= 0; i--) {
                    taskToDelete = $scope.tasks[i];
                    if (taskToDelete.ID == this.t.ID) {
                        $scope.tasks.splice(i, 1);
                    }
                }
            }
        }
    }

    //function inlineEdit() {
    //    var editorTemplate = '<div>' +
    //        '<div ng-hide="view.editorEnabled" ng-click="enableEditor()">' +
    //            '{{value}} ' +
    //        '</div>' +
    //        '<div ng-show="view.editorEnabled">' +
    //            '<input id="taskEditor" type="text" ng-model="view.editableValue">' +
    //            '<a ng-click="save()"><span class="glyphicon glyphicon-ok"></a>' +
    //            '<a ng-click="disableEditor()"><span class="glyphicon glyphicon-remove"></a>' +
    //        '</div>' +
    //    '</div>';
    //    return {
    //        restrict: "A",
    //        replace: true,
    //        template: editorTemplate,
    //        scope: {
    //            value: "=clickToEdit",
    //            saveCallback: "&saveFunction",
    //            cancelCallback: "&cancelFunction"
    //        },
    //        link: function (scope, elem, attrs) {
    //            attrs.$observe('newtask', function (value) {
    //                if (value == "true") {
    //                    scope.enableEditor();
    //                }
    //            });
    //        },
    //        controller: function ($scope, $timeout) {
    //            $scope.view = {
    //                previousValue: $scope.value,
    //                editableValue: $scope.value,
    //                editorEnabled: false
    //            };

    //            $scope.enableEditor = function () {
    //                $scope.view.editorEnabled = true;
    //                $scope.view.editableValue = $scope.value;
    //            };

    //            $scope.closeEditor = function () {
    //                $scope.view.editorEnabled = false;
    //            };

    //            $scope.disableEditor = function () {
    //                $scope.value = $scope.view.previousValue;
    //                $scope.closeEditor();
    //                $timeout(function () {
    //                    $scope.cancelCallback();
    //                });
    //            };

    //            $scope.save = function () {
    //                $scope.value = $scope.view.editableValue;
    //                $scope.closeEditor();
    //                $timeout(function () {
    //                    $scope.saveCallback();
    //                });
    //            };
    //        }
    //    }
    //}
    
}(angular));