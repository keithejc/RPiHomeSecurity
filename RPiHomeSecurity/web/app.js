console.log("open ");

var thisWsUri;
if (document.location.origin == "file://") {
    thisWsUri = "ws://127.0.0.1:8080/ws";
} else {
    thisWsUri = (document.location.protocol === "http:" ? "ws:" : "wss:") + "//" +
    document.location.host + "/ws";
}

var connection = new autobahn.Connection({
    url: thisWsUri,
    realm: "rpihomesecurity"
});


var app = angular.module("RpiHomeSecurity", []);

app.controller("PublishingCtrl", function ($scope) {
    $scope.model = { message: "Hello World" };

    $scope.clickMe = function (outgoingMsg) {
        if (connection.session) {
            connection.session.call("com.rpihomesecurity.runactionlist", [outgoingMsg]).then(
                function(sub) {
                    console.log("sent runactionlist" + outgoingMsg);
                });
            
        } else {
            console.log("cannot publish: no session");
        }
    };

    $scope.getActionLists = function() {
        if (connection.session) {
            console.log("call com.rpihomesecurity.getactionlists");
            connection.session.call("com.rpihomesecurity.getactionlists").then(
                function(result) {
                    console.log("result com.rpihomesecurity.getactionlists" + result);
                    $scope.model.actionLists = result;
                });
        }
    }
});

app.controller("ReceivingCtrl", ['$scope', function ($scope) {
    $scope.model = { message: "Nothing..." };

    $scope.showMe = function (incomingMsg) {
        $scope.model.message = incomingMsg;
    };
}]);


// "onopen" handler will fire when WAMP session has been established ..
connection.onopen = function (session) {

    console.log("session established!");

    // our event handler we will subscribe on our topic
    //
    function onevent1(args, kwargs) {
        console.log("got event:", args, kwargs);
        status = args[0];

        var scope = angular.element(document.getElementById('Receiver')).scope();
        scope.$apply(function () {
            scope.showMe(args[0]);
        });
    }

    // subscribe to receive events on a topic ..
    //
    session.subscribe('com.rpihomesecurity.onstatus', onevent1).then(
       function (subscription) {
           console.log("ok, subscribed with ID " + subscription.id);
       },
       function (error) {
           console.log(error);
       }
    );
};


// "onclose" handler will fire when connection was lost ..
connection.onclose = function (reason, details) {
    console.log("connection lost", reason);
}


// initiate opening of WAMP connection ..
connection.open();