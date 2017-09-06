
function TelerikGridView_OnError(e) {
    debugger
    DisplayTextMessages(e.XMLHttpRequest.responseText);
    e.preventDefault();
}

function TelerikonUpload_OnSuccess(e) {
    DisplayTextMessages(e.XMLHttpRequest.responseText);
    $('.t-upload-files').remove();
}

function DisplayTextMessages(text) {
    var json = jQuery.parseJSON(text);
    DisplayJsonMessages(json);
}

function DisplayJsonMessages(json) {
    if (json != null) {
        if (json.ErrorMessages != null) {
            for (var i = 0; i < json.ErrorMessages.length; i++) {
                $message.error(json.ErrorMessages[i]);
            }
        }

        if (json.WarningMessages != null) {
            for (var i = 0; i < json.WarningMessages.length; i++) {
                $message.warning(json.WarningMessages[i]);
            }
        }

        if (json.SuccessMessages != null) {
            for (var i = 0; i < json.SuccessMessages.length; i++) {
                $message.success(json.SuccessMessages[i]);
            }
        }
    }
}
function TelerikGridView_OnComplete(e) {
    if (e.response.ErrorMessages != null) {
        for (var i = 0; i < e.response.ErrorMessages.length; i++) {
            $message.error(e.response.ErrorMessages[i]);
        }
    }

    if (e.response.WarningMessages != null) {
        for (var i = 0; i < e.response.WarningMessages.length; i++) {
            $message.warning(e.response.WarningMessages[i]);
        }
    }

    if (e.response.SuccessMessages != null) {
        for (var i = 0; i < e.response.SuccessMessages.length; i++) {
            $message.success(e.response.SuccessMessages[i]);
        }
    }
}

(function () {
    window.$message = {
        error: function (msg) {
            showMessage(msg, "error");
        },
        warning: function (msg) {
            showMessage(msg, "warning");
        },
        success: function (msg) {
            showMessage(msg, "success");
        }
    };
}
)();

var lastMessageTimeStamp = new Date("1900-1-1");
function showMessage(msg, level) {
    if (msg != undefined && msg.length > 0) {
        var currentDate = new Date();
        msg = currentDate.toLocaleString() + " - " + msg;

        if (currentDate.getTime() - lastMessageTimeStamp.getTime() < 1000) {
            lastMessageTimeStamp = new Date(lastMessageTimeStamp.getTime() + 100);
            setTimeout(ScrollMessage, lastMessageTimeStamp.getTime() - currentDate.getTime(), msg, level);
        }
        else {
            lastMessageTimeStamp = currentDate;
            ScrollMessage(msg, level);
        }
    }
}

function ScrollMessage(msg, level) {
    $('<li class="' + level + '"/>').fadeIn('slow').html(msg).prependTo($("#divMessage"));
    $("#messagebtn").show();
    $("#divMessage").show();
}

(function () {
    var st = window.setTimeout;
    window.setTimeout = function (fn, mDelay) {
        var t = new Date().getTime();
        if (typeof fn == 'function') {
            var args = Array.prototype.slice.call(arguments, 2);
            var f = function () {
                args.push(new Date().getTime() - t - mDelay);
                fn.apply(null, args)
            };
            return st(f, mDelay);
        }
        return st(fn, mDelay);
    }
})();

function ClearMessage() {
    $("#divMessage").empty();
    $("#messagebtn").hide();
    $("#divMessage").hide();
}

var flag = false;
function Resize() {
    flag = !flag;
    if (flag) {
        $("#divMessage").attr("style", "max-height:500px;");
    }
    else {
        $("#divMessage").attr("style", "max-height:58px;");
    }

    $("#divMessage").show();
}
