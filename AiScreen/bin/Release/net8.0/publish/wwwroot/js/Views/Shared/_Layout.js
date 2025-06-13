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
    var jsonsidebar = localStorage.getItem('Sidebar');
    var isValidJson = true; // Assume it's valid initially

    try {
        var sidebar = JSON.parse(jsonsidebar);
    } catch (e) {
        isValidJson = false;
        console.error("Invalid JSON:", e);
    }

    if (isValidJson && sidebar != "" && sidebar != null && sidebar != "undifined") {
       // console.log("Valid JSON:", sidebar);
    } else {
        $.ajax({
            type: "get",
            url: "/Home/GetSidebar",
            datatype: "json",
            traditional: true,
            success: function (data) {
                console.log(data)
                localStorage.setItem("Sidebar", data);
                //toast.show();

                setTimeout(function () {
                    var url = '/Home/Dashboard';
                    window.location.href = url;
                }, 2000);
            },
        });
    }

    //getProfile();


    $('#sidebarcontent').load('/Home/Sidebar', function () {

        setActiveSidebar();
        setActiveSidebarMini();

    });

    //$('#sidebarcontent2').load('/Home/Sidebar2', function () {

    //    setActiveSidebarMini();
    //});


    $('.sidebarToggle').on('click', function () {
        if ($(this).attr('parent-sidebar') == "1") {
            $('#sidebar').addClass('hidden');
            $('#sidebar2').removeClass('hidden');
        }
        else {
            $('#sidebar').removeClass('hidden');
            $('#sidebar2').addClass('hidden');
        }
    });

    $(document).on('click', "#sidebarcontent li a.nav-header", function () {
        $("#sidebarcontent li ul.list-unstyled").collapse('hide');
        $("#sidebarcontent").find(".active").removeClass("active");
        $(this).find(".fas").addClass("active");
        $(this).addClass("active");
    });

    $('#btnLogout').on('click', function () {
        var url = '/Home/Index';
        window.location.href = url;
    });

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

function setActiveSidebar() {
    var page_url = window.location.pathname;
    $("#sidebarcontent li.nav-item a.nav-link").each(function () {
        var nav_url = $(this).attr("href");
        if (nav_url.includes(page_url) || page_url.includes(nav_url)) {
            var submenu_id = $(this).closest("ul").attr('id');
            $('#' + submenu_id).collapse('show');
            $('#sidebarcontent a.nav-header[href="#' + submenu_id + '"]').find(".fas").addClass("active");
            $('#sidebarcontent a.nav-header[href="#' + submenu_id + '"]').addClass("active");
            $(this).addClass('active');
            return false;
        }
    });
}

function setActiveSidebarMini() {
    var page_url = window.location.pathname;
    $("#sidebarcontent2 li.nav-item a.nav-link").each(function () {
        var nav_url = $(this).attr("href");
        if (nav_url.includes(page_url) || page_url.includes(nav_url)) {
            var submenu_id = $(this).closest("ul").attr('id');
            $('#' + submenu_id).collapse('show');
            $('#sidebarcontent2 a.nav-header[href="#' + submenu_id + '"]').find(".fas").addClass("active");
            $('#sidebarcontent2 a.nav-header[href="#' + submenu_id + '"]').addClass("active");
            $(this).addClass('active');
            return false;
        }
    });
}

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

async function pageViewType() {
    const type = $("#txtPageType").val();
    console.log(type);

    switch (type) {
        case 'View':
            setReadOnly(true);
            toggleDisplay(type,true,false, false);
            break;

        case 'Add':
        case 'New':
            setReadOnly(false);
            toggleDisplay(type, false,true,false);
            break;

        case 'Edit':
            setReadOnly(false);
            toggleDisplay(type, false, true,true);
            break;
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