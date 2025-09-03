// Track Application JavaScript - Professional Version
document.addEventListener('DOMContentLoaded', function () {
    initializeTrackForm();
    initializeValidation();
    initializeAnimations();
});

function initializeTrackForm() {
    const complaintInput = document.querySelector('input[name="ComplaintNumber"]');
    const mobileInput = document.querySelector('input[name="MobileNumber"]');

    if (complaintInput) {
        // Format complaint number input
        complaintInput.addEventListener('input', function (e) {
            let value = e.target.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
            e.target.value = value;

            // Add visual feedback
            if (isValidComplaintFormat(value)) {
                e.target.classList.add('is-valid');
                e.target.classList.remove('is-invalid');
            } else if (value.length > 0) {
                e.target.classList.add('is-invalid');
                e.target.classList.remove('is-valid');
            } else {
                e.target.classList.remove('is-valid', 'is-invalid');
            }
        });

        // Auto-focus on complaint number input
        if (!complaintInput.value) {
            setTimeout(() => complaintInput.focus(), 300);
        }
    }

    if (mobileInput) {
        // Format mobile number input
        mobileInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');
            if (value.length > 10) {
                value = value.substring(0, 10);
            }
            e.target.value = value;

            // Add visual feedback
            if (value.length === 10 || value.length === 0) {
                e.target.classList.remove('is-invalid');
            } else {
                e.target.classList.add('is-invalid');
            }
        });
    }
}

function initializeValidation() {
    const form = document.querySelector('.track-form');
    if (form) {
        form.addEventListener('submit', function (e) {
            const complaintInput = document.querySelector('input[name="ComplaintNumber"]');
            const mobileInput = document.querySelector('input[name="MobileNumber"]');

            let isValid = true;

            // Validate complaint number
            if (!complaintInput.value.trim()) {
                showValidationError(complaintInput, 'कृपया तक्रार क्रमांक प्रविष्ट करा');
                isValid = false;
            } else if (!isValidComplaintFormat(complaintInput.value)) {
                showValidationError(complaintInput, 'कृपया योग्य तक्रार क्रमांक प्रारूप वापरा');
                isValid = false;
            } else {
                clearValidationError(complaintInput);
            }

            // Validate mobile number if provided
            if (mobileInput.value && mobileInput.value.length !== 10) {
                showValidationError(mobileInput, 'मोबाइल नंबर 10 अंकांचा असावा');
                isValid = false;
            } else {
                clearValidationError(mobileInput);
            }

            if (!isValid) {
                e.preventDefault();
                showToast('कृपया सर्व फील्ड योग्यरित्या भरा', 'error');
            } else {
                // Show loading state
                const submitBtn = form.querySelector('button[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Searching...';
                }
            }
        });
    }
}

function initializeAnimations() {
    // Animate elements on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observe elements for animation
    document.querySelectorAll('.info-card, .application-card, .details-section').forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(30px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);
    });
}

function clearForm() {
    const complaintInput = document.querySelector('input[name="ComplaintNumber"]');
    const mobileInput = document.querySelector('input[name="MobileNumber"]');

    if (complaintInput) {
        complaintInput.value = '';
        complaintInput.classList.remove('is-valid', 'is-invalid');
        clearValidationError(complaintInput);
        complaintInput.focus();
    }

    if (mobileInput) {
        mobileInput.value = '';
        mobileInput.classList.remove('is-valid', 'is-invalid');
        clearValidationError(mobileInput);
    }

    // Clear validation summary
    const validationSummary = document.querySelector('[data-valmsg-summary]');
    if (validationSummary) {
        validationSummary.style.display = 'none';
    }

    showToast('फॉर्म क्लियर केला गेला', 'success');
}

function isValidComplaintFormat(complaintNumber) {
    const patterns = [
        /^GUT\d{5}$/,  // GUT00001
        /^OFC\d{5}$/,  // OFC00001
        /^RPF\d{5}$/,  // RPF00001
        /^TFL\d{5}$/,  // TFL00001
        /^TTR\d{5}$/,  // TTR00001
        /^DEP\d{5}$/   // DEP00001
    ];

    return patterns.some(pattern => pattern.test(complaintNumber));
}

function showValidationError(input, message) {
    input.classList.add('is-invalid');

    let errorElement = input.parentNode.querySelector('.invalid-feedback');
    if (!errorElement) {
        errorElement = document.createElement('div');
        errorElement.className = 'invalid-feedback';
        input.parentNode.appendChild(errorElement);
    }
    errorElement.textContent = message;
}

function clearValidationError(input) {
    input.classList.remove('is-invalid');

    const errorElement = input.parentNode.querySelector('.invalid-feedback');
    if (errorElement) {
        errorElement.remove();
    }
}

function showToast(message, type = 'info') {
    // Create toast container if it doesn't exist
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '1060';
        document.body.appendChild(toastContainer);
    }

    const toastId = 'toast-' + Date.now();
    const bgClass = type === 'error' ? 'bg-danger' : type === 'success' ? 'bg-success' : 'bg-info';

    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white ${bgClass}" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas ${type === 'error' ? 'fa-exclamation-circle' : type === 'success' ? 'fa-check-circle' : 'fa-info-circle'} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: type === 'error' ? 7000 : 4000
    });

    toast.show();

    // Remove toast element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });
}

// Copy complaint number to clipboard
function copyComplaintNumber(complaintNumber) {
    navigator.clipboard.writeText(complaintNumber).then(function () {
        showToast('तक्रार क्रमांक कॉपी झाला!', 'success');
    }).catch(function (err) {
        console.error('Could not copy text: ', err);
        showToast('कॉपी करता आली नाही', 'error');
    });
}

// Add click to copy functionality to complaint numbers
document.addEventListener('click', function (e) {
    const numberElement = e.target.closest('.complaint-number .number');
    if (numberElement) {
        copyComplaintNumber(numberElement.textContent.trim());
    }
});
