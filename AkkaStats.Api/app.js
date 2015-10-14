/*jslint browser: true*/
/*global toastr, $, console*/
(function () {
    'use strict';

    var disableScope = function ($compileProvider) {
        $compileProvider.debugInfoEnabled(false);
    };

    disableScope.$inject = ['$compileProvider'];

    var homeController = function (httpRepository, toaster, $timeout, realTimeService, $scope, $uibModal, $log) {
        var vm = this;

        vm.messages = [];

        $scope.$on('realTimeService-event-fired', function (evt, data) {
            console.log(data);
            vm.messages.unshift(data.value);
            $scope.$apply();
        });

        var loadHitters = function () {
            vm.hitters = [];
            httpRepository.getData('api/stats/hitter').then(function (data) {
                console.log(data);
                vm.hitters = data;
            });
        }

        var loadPitchers = function () {
            vm.pitchers = [];
            httpRepository.getData('api/stats/pitcher').then(function (data) {
                console.log(data);
                vm.pitchers = data;
            });
        }

        loadHitters();
        loadPitchers();

        vm.removeLog = function () {
            vm.messages = [];
        };

        vm.addHitter = function () {

            httpRepository.postData('api/stats/hitter', vm.hitter).then(function (data) {
                $timeout(function () { loadHitters(); }, 1000);
                vm.hitter.name = "";
                vm.hitter.hrs = "";
                toaster.pop('success', "Added hitter", vm.hitter.name);
            });
        };

        vm.deleteHitter = function (id) {
            httpRepository.deleteData('api/stats/hitter/' + id).then(function (data) {

                $timeout(function () { loadHitters(); }, 1000);
                toaster.pop('error', "Deleted hitter", "");
            });
        };

        vm.addPitcher = function () {

            httpRepository.postData('api/stats/pitcher', vm.pitcher).then(function (data) {


                $timeout(function () { loadPitchers(); }, 1000);

                vm.pitcher.name = "";
                vm.pitcher.wins = "";
                toaster.pop('success', "Added pitcher", vm.pitcher.name);

            });
        };

        vm.deletePitcher = function (id) {
            httpRepository.deleteData('api/stats/pitcher/' + id).then(function (data) {
                $timeout(function () { loadPitchers(); }, 1000);
                toaster.pop('error', "Deleted pitcher", "");
            });
        };

        vm.openHitter = function (id) {

            httpRepository.getData('api/stats/hitter/' + id).then(function (data) {

                var modalInstance = $uibModal.open({
                    templateUrl: 'myModalContent.html',
                    controller: 'modalController',
                    resolve: {
                        player: function () {
                            return data;
                        }
                    }
                });

                modalInstance.result.then(function () {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });

            });
            console.log(id);
        };

        vm.openPitcher = function (id) {

            httpRepository.getData('api/stats/pitcher/' + id).then(function (data) {

                var modalInstance = $uibModal.open({
                    templateUrl: 'myModalContent.html',
                    controller: 'modalController',
                    resolve: {
                        player: function () {
                            return data;
                        }
                    }
                });

                modalInstance.result.then(function () {
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });

            });

            console.log(id);
        };

    };

    homeController.$inject = ['httpRepository', 'toaster', '$timeout', 'realTimeService', '$scope', '$uibModal', '$log'];


    var modalController = function ($scope, $modalInstance, player) {



            console.log('------');
            console.log(player);
            console.log('------');
  


        $scope.playerInfo = player;


        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    };

    modalController.$inject = ['$scope', '$modalInstance', 'player'];

    angular.module('akkaApp', ['ngAnimate', 'toaster', 'realTimeService', 'ui.bootstrap'])
        .config(disableScope)
        .controller('homeController', homeController)
        .controller('modalController', modalController);

    angular.module('realTimeService', []).factory('realTimeService', ['$rootScope', function ($rootScope) {

        var hub = $.connection.statsHub;
        hub.client.broadcastMessage = function (date, data) {
            $rootScope.$broadcast('realTimeService-event-fired', {
                'date': date,
                'value': data
            });
        };

        $.connection.hub.start().done().fail(function (data) {
            alert(data);
        });

        return {

        };

    }]);

}());
