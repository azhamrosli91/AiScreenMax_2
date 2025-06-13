$(document).ready(function () {
    validate();
    isfirstTime(); 

    $('form').validate().settings.errorClass += ' is-invalid';
    $('form').validate().settings.validClass += ' is-valid';

    $(".has-validation input").attrchange({
        trackValues: true, // set to true so that the event object is updated with old & new values
        callback: function (evnt) {
            // console.log("triggered", evnt);

            if (evnt.newValue.includes("is-invalid")) {
                //console.log("aria-invalid", $(this).attr("is-invalid"));
                $(this).siblings('.input-group-text').css('border', '1px solid red');
            }
            if (evnt.newValue.includes("is-valid")) {
                $(this).siblings('.input-group-text').css('border', '1px solid #48BB8F');
            }
        }
    });

    $("#show_hide_password a").on('click', function (event) {
        event.preventDefault();
        if ($('#show_hide_password input').attr("type") == "text") {
            $('#show_hide_password input').attr('type', 'password');
            $('.showPass').addClass("fa-eye-slash");
            $('.showPass').removeClass("fa-eye");
        } else if ($('#show_hide_password input').attr("type") == "password") {
            $('#show_hide_password input').attr('type', 'text');
            $('.showPass').removeClass("fa-eye-slash");
            $('.showPass').addClass("fa-eye");
        }
    });

    $("#btnSubmit").click(function (e) {
        $('form').validate();
        if ($('form').valid()) {
            // For prototype show loading purpose
            e.preventDefault();
            // For prototype re-trigger purpose
   
            $("#loginForm").submit();
        }
    });

    $("#btnForgot").click(function () {
       
        var url = '/Account/ForgotPassword';
        window.location.href = url;

    });
});
$(window).on('beforeunload', function () {
    $("div.spanner").addClass("show");
    $("div.overlay").addClass("show");
});
$(window).on('unload', function () {
    $("div.spanner").removeClass("show");
    $("div.overlay").removeClass("show");
});
function isfirstTime() {
    var isFirstTime = $("#viewbag_isFistTime").data("firsttime");
    var dataModel = $("#viewbag_isFistTime").data("userdetail");

    if (isFirstTime == "True") {
        var _url = `/Home/FirstTimeLogin?datamodel=${dataModel}`;
        $.ajax({
            type: "GET",
            url: _url,
            success: function (result) {
           
                $("#modalContent").html(result);
                $("#myModal").modal("show");
            },
        });
    }
}
function validate() {
    var validate = $('#viewbag_validate').attr('data-validate');

    if (validate == "True") {
        const toastLiveExample = document.getElementById('liveToast');
        const toast = new bootstrap.Toast(toastLiveExample);

        $.ajax({
            type: "get",
            url: "/Home/GetSidebar",
            datatype: "json",
            traditional: true,
            success: function (data) {
                console.log(data)
                localStorage.setItem("Sidebar", data);
                toast.show();

                setTimeout(function () {
                    var url = '/Home/Dashboard';
                    window.location.href = url;
                }, 2000);
            },
        });
    }
    else if (validate == "False") {
        const toastLiveExample = document.getElementById('liveToast2');
        const toast = new bootstrap.Toast(toastLiveExample);

        toast.show();
    }
}

function redirect(color) {
    if (color == "green") {
        var url = '/Home/List'; 
        window.location.href = url;
    }
    if (color == "red") {
        var url = '/Home/Index';
        window.location.href = url;
    }
}

function keyboardOn(id) {
    $("#" + id).keyboard({
        autoAccept: true,
        accepted: function () {

        }
    }).addTyping();

    $("#" + id).focus();
}