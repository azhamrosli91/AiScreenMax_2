function createCookies(key, value,expiredMinutes)
{
    var currentTime = new Date();

    var expirationTime = new Date(currentTime.getTime() + expiredMinutes + 60000);
    $.cookie(key, value, { expires: expirationTime });
}
function deleteCookies(key) {
    $.cookie(key, null, { expires: -1, path: '/' });

}
function validateForm()
{
    var status = true;
    // Iterate over each element with class .require
    //checking all require field
    $(".require").each(function() {
        // Check if the value is empty or null

        if (!$(this).val()) {
            // If value is empty or null, add red border
            $(this).css("border-color", "red");
            var errorLabelId = $(this).attr("error-msg-id");
            $("#" + errorLabelId).text("Field is required");
            $("#" + errorLabelId).show(); // Show the error label
            $("#" + errorLabelId).removeClass("d-none");
            $("#" + errorLabelId).html($(this).attr("error-msg"));
            status = false;
        }
    });
    if (status == false) {
        return status;
    }

    //checking all required email
    $("input[type='email'].require").each(function () {
        var email = $(this).val();
        // Regular expression for email validation
        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email.match(emailPattern)) {
            // If email is not valid, set border color to red
            $(this).css("border-color", "red");
            var errorLabelId2 = $(this).attr("error-msg-id");
            $("#" + errorLabelId2).show(); // Show the error label
            $("#" + errorLabelId2).removeClass("d-none");
            $("#" + errorLabelId2).text("Invalid email address");
            status = false;

        } else {
            // If email is valid, reset border color
            $(this).css("border-color", "");
            var errorLabelId3 = $(this).attr("error-msg-id");
            $("#" + errorLabelId3).hide(); // Hide the error label
            $("#" + errorLabelId3).addClass("d-none");
        }
    });

    // Checking all pattern required fields
    $("input[pattern].require").each(function () {
        var value = $(this).val();
        var pattern = new RegExp($(this).attr("pattern"));
        if (!pattern.test(value)) {
            // If pattern is not valid, set border color to red
            $(this).css("border-color", "red");
            var errorLabelId4 = $(this).attr("error-msg-id");
            var errorLabel = $(this).data("error-pattern");
            $("#" + errorLabelId4).show(); // Show the error label
            $("#" + errorLabelId4).removeClass("d-none");
            $("#" + errorLabelId4).text(errorLabel);
            status = false;
        } else {
            // If pattern is valid, reset border color
            $(this).css("border-color", "");
            var errorLabelId5 = $(this).attr("error-msg-id");
            $("#" + errorLabelId5).hide(); // Hide the error label
            $("#" + errorLabelId5).addClass("d-none");
        }
    });

   
    return status;
}
function loadings(def) {
    def = def === undefined ? true : def;
    if (def) {
        $("div.spanner").addClass("show");
        $("div.overlay").addClass("show");
    } else {
        $("div.spanner").removeClass("show");
        $("div.overlay").removeClass("show");
    }
}
function confirmSubmit(action, title, message) {
    if (!title || title.trim() === '') {
        title = 'Confirmation';
    }
    if (!message || message.trim() === '') {
        message = 'Are you sure to submit this record?';
    }
    if ($('#submit-confirmation-modal').length) {
        $('#submit-confirmation-modal').find('.modal-title').text(title);
        $('#submit-confirmation-modal').find('.modal-text').text(message);
        $('#submit-confirmation-modal').find('.modal-submit-btn').off('click').on('click', function () {
            if (typeof action === 'function') {
                action();
            }
        });
        $('#submit-confirmation-modal').modal('show');
    }
}
//GENERAL ALERT
function toastError(title, message, callback) {
    if (!message || message.trim() === '') {
        message = title;
        title = 'Warning!';
    }
    if ($('#errorToast').length) {
        $('#errorToast').find('.title').text(title);
        $('#errorToast').find('.message').text(message);
        const toastLiveExample = document.getElementById('errorToast');
        const toast = new bootstrap.Toast(toastLiveExample, { delay: 1000 });
        toast.show();
        // If a callback function is provided, execute it after the toast is shown
        if (typeof callback === 'function') {
            toastLiveExample.addEventListener('hidden.bs.toast', function () {
                callback();
            }, { once: true });
        }
    }
}
function toastSuccess(title, message, callback) {
    if (!message || message.trim() === '') {
        message = title;
        title = 'Successful!';
    }
    if ($('#successToast').length) {
        $('#successToast').find('.title').text(title);
        $('#successToast').find('.message').text(message);
        const toastLiveExample = document.getElementById('successToast');
        const toast = new bootstrap.Toast(toastLiveExample, { delay: 1000 });
        toast.show();
        // If a callback function is provided, execute it after the toast is shown
        if (typeof callback === 'function') {
            toastLiveExample.addEventListener('hidden.bs.toast', function () {
                callback();
            }, { once: true });
        }
    }
}

function getFormattedDate(date) {
    var year = date.getFullYear();
    var month = ('0' + (date.getMonth() + 1)).slice(-2); // Adding 1 because month is zero based
    var day = ('0' + date.getDate()).slice(-2);
    return day + '/' + month + '/' + year;
}
function validateField(field) {
    var isValid = true;
    var fieldValue = field.val();
    var fieldType = field.attr('type');

    if (field.hasClass('required') && !fieldValue) {
        isValid = false;
        field.addClass('is-invalid');
        field.next('.invalid-feedback').remove();
        field.after('<div class="invalid-feedback">Field is required.</div>');
     
    } else {
        if (fieldType === 'email' && fieldValue) {
            var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailPattern.test(fieldValue)) {
                isValid = false;
                field.addClass('is-invalid');
                field.next('.invalid-feedback').remove();
                field.after('<div class="invalid-feedback">Please fill up email address.</div>');
            } else {
                field.removeClass('is-invalid');
                field.next('.invalid-feedback').remove();
            }
        } else if (fieldType === 'number' && fieldValue) {
            if (isNaN(fieldValue)) {
                isValid = false;
                field.addClass('is-invalid');
                field.next('.invalid-feedback').remove();
                field.after('<div class="invalid-feedback">Number is invalid.</div>');
            } else {
                field.removeClass('is-invalid');
                field.next('.invalid-feedback').remove();
            }
        } else {
            field.removeClass('is-invalid');
            field.next('.invalid-feedback').remove();
        }
    }
    return isValid;
}
function getFileExtension(filename) {
    return filename.split('.').pop().toLowerCase();
}
function validateDateFromToField(field) {
    var isValid = true;
    var fieldValue = field.val();
    var fieldType = field.attr('type');

    // Additional validation for date comparison
    if (field.attr('id') === 'DATE_FROM' && fieldValue) {
        var toDateValue = $('#DATE_TO').val();
        if (toDateValue) {
            var fromDate = new Date(fieldValue);
            var toDate = new Date(toDateValue);
            if (fromDate > toDate) {
                isValid = false;
                field.addClass('is-invalid');
                field.next('.invalid-feedback').remove();
                field.after('<div class="invalid-feedback">Invalid date.</div>');
            } else {
                field.removeClass('is-invalid');
                field.next('.invalid-feedback').remove();
            }
        }
    }

    // Main validation logic for other field types
    // ...

    return isValid;
}
function updateRowNumbers(table) {
    $(table).find('tbody tr').each(function (index, tr) {
        $(tr).find('td').first().text(index + 1);
    });
}
// Helper function to generate GUID
function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

// Helper function to get file extension
function getFileExtension(filename) {
    return filename.substring(filename.lastIndexOf('.'));
}
// Helper function to format date
function formatDate(dateString) {
    var date = new Date(dateString);
    return date.toLocaleString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: true });
}

// Helper function to format amount with comma separator
function formatAmount(amount) {
    return parseFloat(amount).toLocaleString('en', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}
function getControllerNameFromUrl() {
    var pathSegments = window.location.pathname.split('/').filter(segment => segment); // Split and remove empty segments
    return pathSegments.length > 0 ? pathSegments[0] : null; // Assume the first segment is the controller name
}
function setReadOnly(isReadOnly) {
    const method = isReadOnly ? 'attr' : 'removeAttr';
    const classMethod = isReadOnly ? 'addClass' : 'removeClass';

    $("input.isEdit, textarea.isEdit").each(function () {
        $(this)[method]("readonly", true);
        $(this)[classMethod]("disabled");
    });

    $("select.isEdit").each(function () {
        $(this)[method]("disabled", true);
        $(this)[classMethod]("disabled");
    });

    $("input[type=file], input[type=checkbox], input[type=date]").attr("disabled", isReadOnly);
}

function toggleDisplay(type,showEditable = false,showSubmit = false, showDelete = false) {
    $(".isEditable_Display").toggle(showEditable);
    $(".btnDelete").toggle(showDelete);
    $(".attachmentbtn").toggle(showEditable);
    $(".btnSubmit").toggle(showSubmit);

    if ($(window).width() < 768 && type == "Edit") {
        $("#buttonContainer > div:last-child").addClass("col-12 col-md-12");
    }
}
