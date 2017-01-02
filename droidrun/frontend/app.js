(function () {
  'use strict';

  var app = angular.module('droidrun', ['ngProgress']);

  app.controller('MainController', ['$scope', '$timeout', 'ngProgressFactory', function ($scope, $timeout, ngProgressFactory) {
    var buttons = ['movePicked', 'turnLeftPicked', 'turnRightPicked', 'buttonsDisabled'];
    
    $scope.userName = localStorage.getItem('userName') || 'Nemo';
    $scope.progressbar = ngProgressFactory.createInstance();
    $scope.progressbar.setColor('#33C3F0');
    
    $scope.socket = io({ query: "name=" + $scope.userName });
    
    $scope.vote = function (action, button) {
      if(!$scope.buttonsDisabled) {
        $scope.socket.emit('vote', action);
        $scope[button] = true;
        $scope.buttonsDisabled = true;
       }
    };
    
    $scope.saveName = function () {
      localStorage.setItem('userName', $scope.userName);
      $scope.socket.emit('change name', $scope.userName);
    };
    
    $scope.socket.on('next round', function (data) {
      $scope.progressbar.start();
      $timeout($scope.progressbar.complete.bind($scope.progressbar), data / 2);
      angular.forEach(buttons, function (but) {
        $scope[but] = false;
        $scope.$apply();
      });
    });
  }]);

  app.directive('drLog', function () {
    return {
      restrict: 'A',
      scope: true,
      link: function (scope, elem, attrs) {
        scope.log = [];

        scope.socket.on('logMessage', function (data) {
          logMessage(data);
          scope.$apply();
          elem[0].scrollTop = elem[0].scrollHeight;
        });

        function logMessage(mes) {
          scope.log.push(mes);
          if (scope.log.length > 100) {
            scope.log = scope.log.slice(50);
          }
        }
      }
    };
  });

} ());