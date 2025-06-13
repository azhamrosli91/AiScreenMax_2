$(document).ready(function () {
    // Redirect to login when "Go to Login" button is clicked
    $("#btnGoToLogin").click(function () {
        window.location.href = '/Home/Index';
    });
    //// Initial state check
    //toggleSubmitButton();

    //// Listen for changes on the checkbox
    //$("#acknowledgeAgreement").change(function () {
    //    toggleSubmitButton();
    //});

    //// Function to enable/disable the submit button and toggle classes
    //function toggleSubmitButton() {
    //    var submitButton = $("button[type='submit']");
    //    if ($("#acknowledgeAgreement").is(":checked")) {
    //        submitButton.removeAttr("disabled"); // Enable the button
    //        submitButton.removeClass("btn-secondary").addClass("btn-primary"); // Add 'btn-primary' and remove 'btn-secondary'
    //    } else {
    //        submitButton.attr("disabled", "disabled"); // Disable the button
    //        submitButton.removeClass("btn-primary").addClass("btn-secondary"); // Add 'btn-secondary' and remove 'btn-primary'
    //    }
    //}
    // Handle ddlCompany change event
    $("#ddlCompany").change(function () {
        if ($(this).val() === "0") {
            $("#dvCompany").removeClass("d-none"); // Show the div
            $("#txtNameCompany").addClass("required"); // Add 'required' class
        } else {
            $("#dvCompany").addClass("d-none"); // Hide the div
            $("#txtNameCompany").removeClass("required"); // Remove 'required' class
        }
    });

    $("#frmRegisterUser").submit(function (event) {
        event.preventDefault(); // Prevent default form submission

        // Validate the form


        // Validate the form
        var formIsValid = true;
        $(this).find('input, select, textarea').each(function () {
            if (!validateField($(this))) {
                formIsValid = false;
            }
        });
        if ($("#txtPassword").val().length < 8)
        {
            formIsValid = true;
            $("#txtPassword").addClass('is-invalid');
            $("#txtPassword").next('.invalid-feedback').remove();
            $("#txtPassword").after('<div class="invalid-feedback">Kata laluan hendaklah melebihi atau sama dengan 8 angka.</div>');
            return; // Stop form submission if validation fails
        } else
        {
            formIsValid = true;
            $("#txtPassword").removeClass('is-invalid');
            $("#txtPassword").next('.invalid-feedback').remove();
        }

        if ($("#txtPassword").val() != $("#txtPasswordConfirm").val()) {
            formIsValid = false;
            $("#txtPassword").addClass('is-invalid');
            $("#txtPassword").next('.invalid-feedback').remove();
            $("#txtPassword").after('<div class="invalid-feedback">Kata laluan tidak sama dengan kata laluan pengesahan.</div>');
            $("#txtPasswordConfirm").addClass('is-invalid');
            $("#txtPasswordConfirm").next('.invalid-feedback').remove();
            return; // Stop form submission if validation fails
        } else {
            formIsValid = true;
            $("#txtPassword").removeClass('is-invalid');
            $("#txtPassword").next('.invalid-feedback').remove();
            $("#txtPasswordConfirm").removeClass('is-invalid');
            $("#txtPasswordConfirm").next('.invalid-feedback').remove();
        }

        if ($("#acknowledgeAgreement").is(":checked")) {
            formIsValid = true;
        } else {
            formIsValid = false;
            $("#acknowledgeAgreement").addClass('is-invalid');
            $("#acknowledgeAgreement").next('.invalid-feedback').remove();
            $("#acknowledgeAgreement").after('<div class="invalid-feedback">Sila setuju untuk teruskan daftar masuk.</div>');
            $("#acknowledgeAgreement").addClass('is-invalid');
            $("#acknowledgeAgreement").next('.invalid-feedback').remove();
        }

        if (!formIsValid) {
            return; // Stop form submission if validation fails
        }

        var registerModel = {
            ID_MM_COMPANY: $("#ddlCompany").val(),
            COMPANY_NAME: $("#txtNameCompany").val(),
            Email: $("#txtEmail").val(),
            Name: $("#txtName").val(),
            Password: btoa($("#txtPassword").val()), // Converts password to Base64
            ConfirmPassword: btoa($("#txtPasswordConfirm").val()) // Converts confirm password to Base64
        };

        $.ajax({
            url: '/Home/RegisterUser', // Replace with your controller action URL
            type: 'POST',
            data: {
                model: registerModel
            },
            beforeSend: function () {
                // Tampilkan elemen loading sebelum AJAX dipanggil
                loadings();
            },
            success: function (response) {
                //response = JSON.parse(response);
                loadings(false);
                if (response.success) {
                    //toastSuccess("Success", response.message, function () {
                    //   // window.location.href = '/Home/Login';
                    //});
                    $("#lblMessage").html(response.message);
                    $("#lblMessage").addClass("text-success");
                    $("#lblMessage").removeClass("text-danger");
                    $("#lblMessage").removeClass("d-none");
                    $("#frmRegisterUser").find("input, textarea, select").val("");
                    $("#successModal .modal-body").text(response.message);
                    $("#successModal").modal('show');

                } else {
                    $("#lblMessage").html(response.message);
                    $("#lblMessage").addClass("text-danger");
                    $("#lblMessage").removeClass("text-success");
                    $("#lblMessage").removeClass("d-none");
                }

            },
            error: function (xhr, status, error) {
                // Handle error response
                console.error(xhr.responseText);
                // Example: Show error message or handle specific errors
            }
        });

    });

});
