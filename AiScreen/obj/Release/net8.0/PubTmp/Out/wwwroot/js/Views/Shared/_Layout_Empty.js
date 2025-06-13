document.addEventListener("DOMContentLoaded", function () {
    var dropdownToggle = document.querySelector(".overlay-dropdown .dropdown-toggle");
    var dropdown = document.querySelector(".overlay-dropdown");

    if (dropdown && dropdownToggle) {
        // Add a click event listener to the document
        document.addEventListener("click", function (event) {
            // Check if the clicked element is not inside the dropdown
            if (!dropdown.contains(event.target) && event.target !== dropdownToggle) {
                // Close the dropdown by removing the "show" class
                dropdown.classList.remove("show");
            }
        });

        // Add a click event listener to the dropdown toggle button
        dropdownToggle.addEventListener("click", function (event) {
            // Prevent the click event from propagating to the document
            event.stopPropagation();

            // Toggle the "show" class on the dropdown
            dropdown.classList.toggle("show");
        });
    }
});


$(document).ready(function () {


    // $('#btnEnglish').on('click', function () {
    //     $.ajax({
    //         type: "POST",
    //         url: "/Home/SiteLanguage",
    //         data: { language: "EN" },
    //         success: function (data) {
    //             location.reload();
    //         },
    //         error: function (xhr, status, error) {
    //             console.log(error);
    //         }
    //     });
    // });

    // $('#btnJapanese').on('click', function () {
    //     $.ajax({
    //         type: "POST",
    //         url: "/Home/SiteLanguage",
    //         data: { language: "JA" },
    //         success: function (data) {
    //             location.reload();
    //         },
    //         error: function (xhr, status, error) {
    //             console.log(error);
    //         }
    //     });
    // });

    // $('#btnHelp').on('click', function () {
    //     var url = '@Url.Action("Help", "Home")';
    //     window.location.href = url;
    // });

    // $(".file-upload").fileinput({
    //     showPreview: false,
    //     showUpload: false,
    //     showRemove: false,
    //     showUploadStats: false,
    //     browseClass: 'btn btn-sm btn-outline-secondary',
    //     browseLabel: 'Choose File',
    //     buttonLabelClass: 'file-input-browse-label'
    // });

    // var url = window.location.pathname;
    // if (url.includes('/Home/Help')) {
    //     $("#spanHelp").css({
    //         'font-weight': 'bold',
    //         'color': '#004098'
    //     });
    // }
});

$(document).ready(function (e) {
    var existingDiv = $('.detail-frame');
    existingDiv.removeClass('ps-md-2 mx-2').addClass('px-0');
    resizeContainer();
    $(window).on('resize', resizeContainer);

    $('input[type="email"]').on('change', function () {
        validateEmail(this);
    });
});

var loadingBool = true;

$(document).on('click', '#btnDashboard_44', function () {
    loadingBool = false;
});

$(window).on('beforeunload', function () {
    loadings(loadingBool);
});

$(window).on('unload', function () {
    loadings(false);
});


function resizeContainer() {
    var windowHeight = $(window).height();
    var headerHeight = $('header.sticky-top').outerHeight();
    var titleHeight = $('.tr-listing-header').outerHeight();
    var footerHeight = $('.tr-footer').outerHeight();
    var footerHeight1 = $('.tr-btm-btn-margin').outerHeight();
    var bottomHeight = $(".tr-btm-btn-area").length ? $(".tr-btm-btn-area").outerHeight() : 0;
    var newHeight = windowHeight - (headerHeight + titleHeight + bottomHeight + footerHeight);
    $('.page-container').css('height', newHeight + 'px');
}
function getProfile() {
    //var profilePic = localStorage.getItem('ProfilePicture')
    //if (profilePic == "" || profilePic == null || profilePic == "undefined" || profilePic == undefined) {
 
    //    $.ajax({
    //        type: "get",
    //        url: "/Account/GetProfileImage?email=" + $("#txtUserEmail").val(),
    //        datatype: "json",
    //        success: function (data) {

    //            if (data.success == true) {
    //                localStorage.setItem("ProfilePicture", data.data);
    //                $("#userprofile").attr("src", "data:image/png;base64," + data.data);
    //            }
    //        },
    //    });

    //} else {
    //    $("#userprofile").attr("src", "data:image/png;base64," + profilePic);
    //}
    
}

function pageViewType() {
    var type = $("#txtPageType").val();

    if (type == 'View') {

        $("input.isEdit").attr("readonly", true);
        $("input.isEdit").addClass("disabled");
        $("textarea.isEdit").attr("readonly", true);
        $("textarea.isEdit").addClass("disabled");

        $("select.isEdit").attr("disabled", true);
        $("select.isEdit").addClass("disabled");

        $("input[type=file]").attr("disabled", true);
        $("input[type=checkbox]").attr("disabled", true);

        $(".isEditable_Display").hide();
        $(".btnDelete").show();

        $(".attachmentbtn").css('display', 'none');

    } else if (type == 'Add' || type == 'New') {

        $("input.isEdit").attr("readonly", false);
        $("input.isEdit").removeClass("disabled");
        $("textarea.isEdit").attr("readonly", false);
        $("textarea.isEdit").removeClass("disabled");

        $("select.isEdit").attr("disabled", false);
        $("select.isEdit").removeClass("disabled");
        $("input[type=checkbox]").attr("disabled", false);
        $(".isEditable_Display").show();
        $(".btnDelete").hide();

        var currentDate = new Date();

        // Format the date as yyyy-MM-dd
        var formattedDate = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2) + '-' + ('0' + currentDate.getDate()).slice(-2);

        // Set the formatted date to the DATE_FROM and DATE_TO fields
        $('#DATE_FROM').val(formattedDate);
        $('#DATE_TO').val(formattedDate);

        $('#LEAVE_TYPE').change();

        $(".attachmentbtn").show();

    } else if (type == 'Edit') {

        $("input.isEdit").attr("readonly", false);
        $("input.isEdit").removeClass("disabled");
        $("textarea.isEdit").attr("readonly", false);
        $("textarea.isEdit").removeClass("disabled");

        $("select.isEdit").attr("disabled", false);
        $("select.isEdit").removeClass("disabled");

        $("input[type=file].isEdit").attr("disabled", false);
        $("input[type=checkbox].isEdit").removeAttr("disabled");

        $(".isEditable_Display").show();
        $(".btnDelete").show();
        $(".attachmentbtn").show();
    }

}
function validateEmail(obj) { 

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    const email = $(obj).val();

    if (emailRegex.test(email)) {
        $(obj).css('border-color', ''); // Reset border color
    } else {
        $(obj).css('border-color', 'red'); // Highlight border in red

        if ($(obj).hasClass('required') == false && email.length == 0) {
            $(obj).css('border-color', ''); // Reset border color
        } 
    }
}