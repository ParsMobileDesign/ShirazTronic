function showLoading() {
    $('body').loadingModal({
        position: 'auto',
        text: 'Loading... Please wait',
        color: '#fff',
        opacity: '0.7',
        backgroundColor: '#9393e2',
        animation: 'chasingDots'
    });
}
function hideLoading() {
    $('body').loadingModal('hide');
    $('body').loadingModal('destroy');
}
function resetPl() {
    showLoading();
    $('#grid').load('/Customer/Product/resetAp', function () {
        hideLoading();
    });
}
$(document).ready(function () {
    // Mobile Navigation
    if ($('.nav-menu').length) {
        var $mobile_nav = $('.nav-menu').clone().prop({
            class: 'mobile-nav d-lg-none'
        });
        $('body').append($mobile_nav);
        $('body').prepend('<button type="button" class="mobile-nav-toggle d-lg-none"><i class="icofont-navigation-menu"></i></button>');
        $('body').append('<div class="mobile-nav-overly"></div>');

        $(document).on('click', '.mobile-nav-toggle', function (e) {
            $('body').toggleClass('mobile-nav-active');
            $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
            $('.mobile-nav-overly').toggle();
        });

        $(document).on('click', '.mobile-nav .drop-down > a', function (e) {
            e.preventDefault();
            $(this).next().slideToggle(300);
            $(this).parent().toggleClass('active');
        });

        $(document).click(function (e) {
            var container = $(".mobile-nav, .mobile-nav-toggle");
            if (!container.is(e.target) && container.has(e.target).length === 0) {
                if ($('body').hasClass('mobile-nav-active')) {
                    $('body').removeClass('mobile-nav-active');
                    $('.mobile-nav-toggle i').toggleClass('icofont-navigation-menu icofont-close');
                    $('.mobile-nav-overly').fadeOut();
                }
            }
        });
    } else if ($(".mobile-nav, .mobile-nav-toggle").length) {
        $(".mobile-nav, .mobile-nav-toggle").hide();
    }
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.customer-header').addClass('header-scrolled');
        } else {
            $('.customer-header').removeClass('header-scrolled');
        }
    });

    if ($(window).scrollTop() > 100) {
        $('.customer-header').addClass('header-scrolled');
    }

    // Stick the header at top on scroll
    $(".customer-header").sticky({
        topSpacing: 0,
        zIndex: '50'
    });
    $(".sticky-filter").sticky({
        topSpacing: 80,
        zIndex: '50'
    });


    $('[specvalsrl]').click(function () {
        if ($('[specvalsrl]:checked').length == 0) {
            resetPl();
        }
        else {
            var filterObj = "";
            var filtersep = "";
            $('[specsrl]').each(function () {
                var obj = $(this);
                var id = obj.attr('specsrl');
                var childIds = "";
                var sep = "";
                obj.find('input[specvalsrl]').each(function () {

                    if ($(this).prop("checked")) {
                        childIds += sep + $(this).attr("specvalsrl")
                        sep = ",";
                    }
                });
                if (childIds.length > 0) {
                    filterObj += filtersep + childIds;
                    filtersep = "-";
                }
            });
            showLoading();
            $('#grid').load('/Customer/Product/filterAp?jsonQuery=' + filterObj, function () {
                hideLoading();
            });
        }
    });

    $('[id ^= "btnAddToCart_"]').click(function () {
         var pId = $(this).attr('id').split("_")[1];
        $('#shoppingCart').load('/Customer/Product/AddToCart?ProductId=' + pId, function (response, status, xhr) {
            if (status == "error") {
                window.location="/Identity/Account/Login"
            }
        });
    });


});



