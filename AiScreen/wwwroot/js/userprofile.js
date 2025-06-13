$(document).ready(function () {
    const email = $("#txtuseremail").val(); // Replace with dynamic value if needed
    loadUserProfileImage(email);
});

function loadUserProfileImage(email) {
    const storageKey = "profile_" + email;
    const cachedImage = localStorage.getItem(storageKey);
    console.log("email : " + email)
    if (cachedImage) {

        $('.img-userprofile').attr('src', cachedImage);
        $("#txtimageprofile").val(cachedImage)
    } else {
        $.ajax({
            url: `/Account/GetProfileImage?email=${encodeURIComponent(email)}`,
            method: 'GET',
            success: function (response) {
                if (response.success && response.data) {
                    localStorage.setItem(storageKey, response.data);
                    $('.img-userprofile').attr('src', response.data);
                    $("#txtimageprofile").val(cachedImage)
                } else {
                    console.error("Failed to load profile image:", response.msg);
                }
            },
            error: function (xhr) {
                console.error("Error fetching profile image:", xhr);
            }
        });
    }
}