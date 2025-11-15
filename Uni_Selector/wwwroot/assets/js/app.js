// Navbar and Layout Interactive Functions

$(document).ready(function () {
    // Initialize all components
    initializeSearchOverlay();
    initializeScrollToTop();
    initializeNavbarScroll();
    initializeMobileMenu();
    
    // Hide loader when page is fully loaded
    $(window).on('load', function () {
        $('.loader').fadeOut(500);
    });
});

// Search Overlay Functions
function initializeSearchOverlay() {
    const searchBtn = $('.search_btn');
    const searchOverlay = $('#search');
    const closeBtn = $('#search .close');

    // Open search overlay
    searchBtn.on('click', function (e) {
        e.preventDefault();
        searchOverlay.addClass('open').fadeIn(300);
        setTimeout(function () {
            $('#search input[type="search"]').focus();
        }, 300);
    });

    // Close search overlay
    closeBtn.on('click', function (e) {
        e.preventDefault();
        searchOverlay.removeClass('open').fadeOut(300);
    });

    // Close on ESC key
    $(document).on('keydown', function (e) {
        if (e.keyCode === 27 && searchOverlay.hasClass('open')) {
            searchOverlay.removeClass('open').fadeOut(300);
        }
    });

    // Close on overlay click
    searchOverlay.on('click', function (e) {
        if (e.target === this) {
            $(this).removeClass('open').fadeOut(300);
        }
    });
}

// Scroll to Top Button
function initializeScrollToTop() {
    const scrollBtn = $('.scrollToTop');

    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            scrollBtn.addClass('active');
        } else {
            scrollBtn.removeClass('active');
        }
    });

    scrollBtn.on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, 600);
        return false;
    });
}

// Navbar Scroll Effect
function initializeNavbarScroll() {
    const navbar = $('.navbar');
    const logo = $('.logo');

    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            navbar.removeClass('no-background').addClass('scrolled');
            $('.logo-display').hide();
            $('.logo-scrolled').show();
        } else {
            navbar.addClass('no-background').removeClass('scrolled');
            $('.logo-display').show();
            $('.logo-scrolled').hide();
        }
    });
}

// Mobile Menu Toggle
function initializeMobileMenu() {
    $('.navbar-toggle').on('click', function () {
        $(this).toggleClass('active');
    });

    // Close mobile menu when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.navbar').length) {
            $('.navbar-collapse').collapse('hide');
            $('.navbar-toggle').removeClass('active');
        }
    });

    // Close mobile menu when clicking a link
    $('.navbar-nav a').on('click', function () {
        if ($(window).width() < 992) {
            $('.navbar-collapse').collapse('hide');
            $('.navbar-toggle').removeClass('active');
        }
    });
}

// Active Menu Highlighting
function setActiveMenuItem() {
    const currentPath = window.location.pathname;
    
    $('.navbar-nav li a').each(function () {
        const href = $(this).attr('href');
        if (href && currentPath.indexOf(href) !== -1 && href.length > 1) {
            $(this).closest('li').addClass('active');
            $(this).closest('.dropdown').addClass('active');
        }
    });
}

// Call on page load
$(document).ready(function () {
    setActiveMenuItem();
});

// Form Validation Helper
function validateForm(formId) {
    const form = $(formId);
    let isValid = true;

    form.find('[required]').each(function () {
        if (!$(this).val()) {
            $(this).addClass('input-validation-error');
            isValid = false;
        } else {
            $(this).removeClass('input-validation-error');
        }
    });

    return isValid;
}

// Show Loading Spinner
function showLoadingSpinner(message) {
    const spinnerHtml = `
        <div class="spinner-overlay">
            <div>
                <div class="spinner"></div>
                ${message ? '<p class="text-center mt-3">' + message + '</p>' : ''}
            </div>
        </div>
    `;
    $('body').append(spinnerHtml);
}

// Hide Loading Spinner
function hideLoadingSpinner() {
    $('.spinner-overlay').fadeOut(300, function () {
        $(this).remove();
    });
}

// Show Alert Message
function showAlert(message, type = 'info', duration = 5000) {
    const alertTypes = {
        success: 'alert-success',
        error: 'alert-danger',
        warning: 'alert-warning',
        info: 'alert-info'
    };

    const alertClass = alertTypes[type] || alertTypes.info;
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert" style="position: fixed; top: 80px; right: 20px; z-index: 9998; min-width: 300px;">
            ${message}
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
    `;

    $('body').append(alertHtml);

    if (duration > 0) {
        setTimeout(function () {
            $('.alert').fadeOut(300, function () {
                $(this).remove();
            });
        }, duration);
    }
}

// AJAX Form Submission Helper
function submitFormAjax(formElement, successCallback, errorCallback) {
    const form = $(formElement);
    const formData = new FormData(form[0]);
    const url = form.attr('action');
    const method = form.attr('method') || 'POST';

    $.ajax({
        url: url,
        type: method,
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (successCallback) successCallback(response);
        },
        error: function (xhr, status, error) {
            if (errorCallback) errorCallback(xhr, status, error);
        }
    });
}

// Debounce Function for Search
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Format Date Helper
function formatDate(dateString, format = 'relative') {
    const date = new Date(dateString);
    const now = new Date();

    if (format === 'relative') {
        const diff = Math.floor((now - date) / 1000); // seconds

        if (diff < 60) return 'Just now';
        if (diff < 3600) return Math.floor(diff / 60) + ' minutes ago';
        if (diff < 86400) return Math.floor(diff / 3600) + ' hours ago';
        if (diff < 604800) return Math.floor(diff / 86400) + ' days ago';
        return date.toLocaleDateString();
    } else if (format === 'short') {
        return date.toLocaleDateString();
    } else {
        return date.toLocaleString();
    }
}

// Confirmation Dialog
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Auto-hide alerts after delay
$(document).on('DOMContentLoaded', function () {
    setTimeout(function () {
        $('.alert').not('.alert-permanent').fadeOut(300, function () {
            $(this).remove();
        });
    }, 5000);
});

// Handle AJAX errors globally
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    if (jqxhr.status === 401) {
        window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
    } else if (jqxhr.status === 403) {
        showAlert('You do not have permission to perform this action.', 'error');
    } else if (jqxhr.status === 500) {
        showAlert('An error occurred on the server. Please try again later.', 'error');
    }
});

// Prevent double form submission
$('form').on('submit', function () {
    const submitBtn = $(this).find('button[type="submit"]');
    if (submitBtn.prop('disabled')) {
        return false;
    }
    submitBtn.prop('disabled', true);
    setTimeout(function () {
        submitBtn.prop('disabled', false);
    }, 3000);
});

// Auto-resize textarea
$('textarea.auto-resize').on('input', function () {
    this.style.height = 'auto';
    this.style.height = (this.scrollHeight) + 'px';
});

// Number formatting
function formatNumber(num, decimals = 2) {
    return parseFloat(num).toFixed(decimals).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

// Currency formatting for JOD
function formatCurrency(amount, currency = 'JOD') {
    return new Intl.NumberFormat('en-JO', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

// Copy to Clipboard
function copyToClipboard(text) {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    showAlert('Copied to clipboard!', 'success', 2000);
}

// Export functions for use in other scripts
window.UniSelectorApp = {
    showLoadingSpinner: showLoadingSpinner,
    hideLoadingSpinner: hideLoadingSpinner,
    showAlert: showAlert,
    submitFormAjax: submitFormAjax,
    debounce: debounce,
    formatDate: formatDate,
    confirmAction: confirmAction,
    formatNumber: formatNumber,
    formatCurrency: formatCurrency,
    copyToClipboard: copyToClipboard,
    validateForm: validateForm
};
