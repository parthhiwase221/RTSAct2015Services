// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Common JavaScript functions for RTS forms

// Validation patterns
const validationPatterns = {
    mobile: /^[6-9]\d{9}$/,
    pincode: /^[1-9][0-9]{5}$/,
    email: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/,
    name: /^[A-Za-z\u0900-\u097F][A-Za-z\u0900-\u097F\s'.-]{1,39}$/
};

// Common validation messages
const validationMessages = {
    required: 'This field is required / हे फील्ड आवश्यक आहे',
    mobile: 'Please enter valid 10-digit mobile number / कृपया वैध 10 अंकी मोबाईल क्रमांक टाका',
    pincode: 'Please enter valid 6-digit PIN code / कृपया वैध 6 अंकी पिन कोड टाका',
    email: 'Please enter valid email address / कृपया वैध ईमेल पत्ता टाका',
    name: 'Please enter valid name (2-40 characters) / कृपया वैध नाव टाका (2-40 अक्षरे)'
};

// Common form validation function
function validateForm(formSelector) {
    let isValid = true;

    $(formSelector + ' input[required], ' + formSelector + ' select[required]').each(function () {
        const $field = $(this);
        const value = $field.val().trim();
        const fieldName = $field.attr('name');

        // Check if required field is empty
        if (!value) {
            showFieldError($field, validationMessages.required);
            isValid = false;
            return;
        }

        // Specific validations
        if (fieldName === 'Mobile' && !validationPatterns.mobile.test(value)) {
            showFieldError($field, validationMessages.mobile);
            isValid = false;
            return;
        }

        if (fieldName === 'PinCode' && !validationPatterns.pincode.test(value)) {
            showFieldError($field, validationMessages.pincode);
            isValid = false;
            return;
        }

        if (fieldName === 'Email' && value && !validationPatterns.email.test(value)) {
            showFieldError($field, validationMessages.email);
            isValid = false;
            return;
        }

        if ((fieldName === 'FirstName' || fieldName === 'LastName') && !validationPatterns.name.test(value)) {
            showFieldError($field, validationMessages.name);
            isValid = false;
            return;
        }

        // If validation passes
        showFieldSuccess($field);
    });

    return isValid;
}

// Show field error
function showFieldError($field, message) {
    $field.addClass('is-invalid').removeClass('is-valid');

    // Remove existing error message
    $field.next('.invalid-feedback').remove();

    // Add error message
    $field.after('<div class="invalid-feedback">' + message + '</div>');
}

// Show field success
function showFieldSuccess($field) {
    $field.addClass('is-valid').removeClass('is-invalid');
    $field.next('.invalid-feedback').remove();
}

// File upload validation
function validateFile(fileInput, maxSizeMB = 10) {
    const file = fileInput.files[0];
    if (!file) return true;

    const allowedTypes = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
    const maxSize = maxSizeMB * 1024 * 1024;

    if (!allowedTypes.includes(file.type)) {
        alert('Please upload only PDF, JPG, PNG, DOC, DOCX files / कृपया फक्त PDF, JPG, PNG, DOC, DOCX फाइल अपलोड करा');
        fileInput.value = '';
        return false;
    }

    if (file.size > maxSize) {
        alert(`File size should not exceed ${maxSizeMB}MB / फाइल साइझ ${maxSizeMB}MB पेक्षा जास्त असू शकत नाही`);
        fileInput.value = '';
        return false;
    }

    return true;
}

// Format file size for display
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// Show loading state for button
function showButtonLoading($button, loadingText = 'Submitting...') {
    const originalText = $button.html();
    $button.data('original-text', originalText);
    $button.html('<i class="fas fa-spinner fa-spin me-2"></i>' + loadingText);
    $button.prop('disabled', true);
}

// Hide loading state for button
function hideButtonLoading($button) {
    const originalText = $button.data('original-text');
    $button.html(originalText);
    $button.prop('disabled', false);
}

// Common document ready functions
$(document).ready(function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-format mobile numbers
    $('input[name="Mobile"]').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '').slice(0, 10);
    });

    // Auto-format PIN codes
    $('input[name="PinCode"]').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '').slice(0, 6);
    });

    // File input change handlers
    $('input[type="file"]').on('change', function () {
        validateFile(this);
    });

    // Form field validation on blur
    $('input, select').on('blur', function () {
        const $field = $(this);
        const value = $field.val();

        if ($field.prop('required') && !value) {
            showFieldError($field, validationMessages.required);
        } else if (value) {
            const fieldName = $field.attr('name');
            let isValid = true;

            if (fieldName === 'Mobile' && !validationPatterns.mobile.test(value)) {
                showFieldError($field, validationMessages.mobile);
                isValid = false;
            } else if (fieldName === 'PinCode' && !validationPatterns.pincode.test(value)) {
                showFieldError($field, validationMessages.pincode);
                isValid = false;
            } else if (fieldName === 'Email' && !validationPatterns.email.test(value)) {
                showFieldError($field, validationMessages.email);
                isValid = false;
            } else if ((fieldName === 'FirstName' || fieldName === 'LastName') && !validationPatterns.name.test(value)) {
                showFieldError($field, validationMessages.name);
                isValid = false;
            }

            if (isValid) {
                showFieldSuccess($field);
            }
        } else {
            $field.removeClass('is-valid is-invalid');
            $field.next('.invalid-feedback').remove();
        }
    });
});
