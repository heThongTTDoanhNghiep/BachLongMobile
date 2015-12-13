$(document).ready(function () {
    $('ul.nav.navbar-nav').find('a[href="' + location.pathname + '"]')
        .closest('li').addClass('active');

    var min_height = $('.product-list .item:first').height();

    //$('.product-list .item').each(function () {
    //    if ($(this).height() > min_height) {
    //        min_height = $(this).height();
    //    }
    //    console.log($(this).height());
    //});

    //$('.product-list .item').css({ 'height': min_height });
});