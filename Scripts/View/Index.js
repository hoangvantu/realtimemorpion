$(function () {
    var loader = "<img class='loader'  src='/Content/images/ajaxloader.gif' />";
    var gameEnd = false;

    var currentPlayer = 1;
    var waitForSomeoneToPlay = false;

    var hideOrShow = function () {
        if (waitForSomeoneToPlay) {
            $("#myTurn").addClass("hide");
            $("#yourTurn").removeClass("hide");
            $("#gameEnd").addClass("hide");
        }
        else {
            $("#yourTurn").addClass("hide");
            $("#myTurn").removeClass("hide");
            $("#gameEnd").addClass("hide");
        }
        if (gameEnd) {
            $("#yourTurn").addClass("hide");
            $("#myTurn").addClass("hide");
            $("#gameEnd").removeClass("hide");
        }
        else {
            $("#gameEnd").addClass("hide");
        }

    }

    var getImage = function (player) {

        if (player === 1 || parseInt(player, 10) === 1) {
            return "<img src='/content/images/croix.png'/>";
        }
        else return "<img src='/content/images/cercle.png'/>";
    }

    var incrementScore = function (id) {
        var score = parseInt($(id).html(), 10);
        score++;
        $(id).html(score);
    }



    // Reference the auto-generated proxy for the hub.
    var ticktacktoe = $.connection.tickTackToeHub;

    ticktacktoe.client.cc_gameEnd = function (board, data) {
        if (data === -2) {
            $("#gameEnd").html(" Game tied ! you have to reset :(");
        }
        else {
            if (data === 0) {
                incrementScore("#scorJ");
            }
            else {
                incrementScore("#scorO");
            }
            $("#gameEnd").html(" Game won ! you can now safely reset :)");
        }

        gameEnd = true;
        hideOrShow();
        $(".square").each(function () {
            var coord = $(this).attr("data-coord");
            var res = coord.split(",");
            var player = board[res[0]][res[1]];
            if (player != undefined && player >= 0) {
                var label = getImage(player);
                $(this).html(label);
            }
        });
    }

    ticktacktoe.client.cc_refresh = function (data, nextPlayer, force) {
        console.log("currentPlayer = " + currentPlayer);
        console.log("nextplayer = " + nextPlayer);
        if (currentPlayer == nextPlayer) {
            waitForSomeoneToPlay = true;
        }
        else {
            waitForSomeoneToPlay = false;
            currentPlayer = nextPlayer;
        }
        hideOrShow();
        var debugRefreshedSquareCount = 0;
        $(".square").each(function () {
            var coord = $(this).attr("data-coord");
            var res = coord.split(",");
            var player = data[res[0]][res[1]];
            
            if (player != undefined && player >= 0) {
                var htmlInSquare =  $(this).html();
                if (force === true || htmlInSquare === "&nbsp;" || htmlInSquare.indexOf('loader') != -1)
                {
                    var label = getImage(player);
                    $(this).html(label);
                    debugRefreshedSquareCount++;
                    console.log("refreshed -> " + debugRefreshedSquareCount);
                }
            }
            
        });

    }

    ticktacktoe.client.cc_join = function (data, nextPlayer, score1, score2) {
        currentPlayer = (nextPlayer + 1) % 2;
        ticktacktoe.client.cc_refresh(data, nextPlayer);
        for (var i = 0; i < score1; i++) {
            incrementScore("#scorJ");
        }
        for (var i = 0; i < score2; i++) {
            incrementScore("#scorO");
        }
   

    }


    ticktacktoe.client.cc_updateUsersOnlineCount = function (count) {
        // Add the message to the page.
        var label = count + " user(s) connected";
        $('#usersCount').text(label);
    };

    ticktacktoe.client.cc_reset = function () {
        $(".square").each(function () {
            $(this).html("&nbsp;");
        });
        waitForSomeoneToPlay = false;
        gameEnd = false;
        currentPlayer =0;
        
        hideOrShow();
        console.log("reset -> currentPlayer = " + currentPlayer);

    };


    // Start the connection.
    $.connection.hub.start().done(function () {
        $(".square").click(function () {
            var $this = $(this);
            if (waitForSomeoneToPlay === false && gameEnd === false) {
                var currentValue = $this.html();

                if (currentValue == '&nbsp;') {
                    waitForSomeoneToPlay === true;
                    var value = $this.attr("data-coord");
                    $this.html(loader);
                    ticktacktoe.server.sC_Play(value, currentPlayer);
                    currentPlayer = (currentPlayer + 1) % 2;
                    hideOrShow();
                }
            }
        });

        $("#reset").click(function () {
            if (gameEnd || confirm("are you sure?")) {
                ticktacktoe.server.sC_Reset();
            }

        });
    });
});