var app = angular.module("app", []);
app.controller('fbFrinedsCtrl', function ($scope, $http) {
    $scope.controllerType = "friend";
    $scope.orginalUserDataList = connectionUserData;

    $scope.complete = function (userdata) {

        var output = [];
        angular.forEach($scope.orginalUserDataList, function (userdata) {

            if (userdata.name.toLowerCase().indexOf(userdata.name.toLowerCase()) >= 0) {
                output.push(userdata);
            }

        });
        $scope.filterUserDataList = output;

    }
    $scope.fillTextbox = function (userdata) {
        $scope.userdata = userdata;
        console.log("fillTextbox");
        $scope.filterUserDataList = null;
    }

    $scope.loadUser = function () {
        console.log("loadUser");
    }

    $scope.addConnection = function (id) {
        alert($scope.controllerType + " " + id);


        $http({
            method: 'GET',
            url: 'api/users/listGrid',
            params: {
                name: 'Johnson'
            }.success(function (data, status, headers, config) {
                console.log(data);
                console.log(status);
            }).error(function (data, status, headers, config) {
                // Error
            });
        })

        //$.ajax({
        //    url: 'api/users/listGrid',
        //    type: 'GET',
        //    data: {
        //        //user_name: $('#user_name').val()
        //    },
        //    dataType: "json",
        //    error: function (xhr) {
        //        alert('Ajax request 發生錯誤');
        //    },
        //    success: function (response) {
        //        //$('#msg_user_name').html(response);
        //        //$('#msg_user_name').fadeIn();

        //    }
        //});

    }
});
