/*global angular*/
(function () {
    angular
        .module('simplAdmin.catalog')
        .factory('connectionService', ['$http', 'Upload', connectionService]);

    function connectionService($http, Upload) {
        var service = {
            getUsers: getUsers,
        };
        return service;

        function getUsers(params) {
            return $http.post('api/user/grid', params);
        }
    }
})();
