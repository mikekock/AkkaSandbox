/*jslint browser: true*/
/*global toastr, $, console*/
(function () {
    'use strict';

    var disableScope = function ($compileProvider) {
        $compileProvider.debugInfoEnabled(false);
    };

    disableScope.$inject = ['$compileProvider'];

    var homeController = function (httpRepository, toaster) {
        var vm = this;

       

        var loadHitters = function () {
            httpRepository.getData('api/stats/hitter').then(function (data) {
                vm.hitters = data;
            });
        }

        var loadPitchers = function () {
            httpRepository.getData('api/stats/pitcher').then(function (data) {
                vm.pitchers = data;
            });
        }

        loadHitters();
        loadPitchers();

        vm.addHitter = function () {

            httpRepository.postData('api/stats/hitter', vm.hitter).then(function (data) {
                loadHitters();
                toaster.pop('success', "Added hitter", vm.hitter.name);
            });
        };

        vm.deleteHitter = function (id) {
            httpRepository.deleteData('api/stats/hitter/' + id).then(function (data) {
                loadHitters();
                toaster.pop('error', "Deleted hitter", "");
            });
        };

        vm.addPitcher = function () {

            httpRepository.postData('api/stats/pitcher', vm.pitcher).then(function (data) {
                loadPitchers();
                vm.pitcher.name = "";
                vm.pitcher.wins = "";
                toaster.pop('success', "Added pitcher", vm.pitcher.name);

            });
        };

        vm.deletePitcher = function (id) {
            httpRepository.deleteData('api/stats/pitcher/' + id).then(function (data) {
                loadPitchers();
                toaster.pop('error', "Deleted pitcher", "");
            });
        };

    };

    homeController.$inject = ['httpRepository', 'toaster'];

    angular.module('akkaApp', ['ngAnimate', 'toaster'])
        .config(disableScope)
        .controller('homeController', homeController);


}());
