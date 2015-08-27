
$("#btn").click(function () {
    if ($("#txtUrl").val() != '') {
        $.post("/", { url: $("#txtUrl").val() })
            .done(function (data) {
                $("#minurl").html("Copy the following: <a href='" + window.location.href.replace(/default.aspx/gi, "") + data.url + "'>" + window.location.href.replace(/default.aspx/gi, "") + data.url + "</a>");
            })
          .fail(function () {
              $("#minurl").html("Sorry, it looks like there was an error. Please try again later.");
          });
    } else {
        $("#minurl").html("Oops, it looks like you forgot to enter a URL !");
    }
});


$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results == null) {
        return null;
    }
    else {
        return results[1] || 0;
    }
}

$(document).ready(function () {
    var err = $.urlParam("error");

    if(err == 404){
        $("#minurl").html("Sorry, it looks like this link does not exist in our database.");
    }

    if (err == "unkown") {
        $("#minurl").html("Sorry, it looks like theres been an unkown error.");
    }
});
