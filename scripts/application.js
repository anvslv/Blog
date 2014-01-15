var App = function() {};

App.applyMagnificPopup = function () {
    $('.image-link').magnificPopup({
        type: 'image',
        closeBtnInside: false,
        closeOnContentClick: false,

        callbacks: {
            open: function() {
                var self = this;
                self.wrap.on('click.pinhandler', 'img', function() {
                    self.wrap.toggleClass('mfp-force-scrollbars');
                });
            },
            beforeClose: function() {
                this.wrap.off('click.pinhandler');
                this.wrap.removeClass('mfp-force-scrollbars');
            }
        },
        image: {
            verticalFit: false
        }
    });
};

App.applyHighlightJs = function() {
    $('pre code').each(function(i, e) { hljs.highlightBlock(e); });
};

App.changeLocalization = function() {
    $('.language').click(function (event) {
        var lang = $(event.target).data('lang');
        if (lang) {
            document.cookie = 'language=' + lang + '; path=/; expires=Mon, 01-Jan-2020 00:00:00 GMT';
        }
    });
};

App.showLoginForm = function() {
    $('.popup-with-form').magnificPopup({
        type: 'inline',
        preloader: false,
        focus: '#username',

        // When elemened is focused, some mobile browsers in some cases zoom in
        // It looks not nice, so we disable it:
        callbacks: {
            beforeOpen: function() {
                if ($(window).width() < 700) {
                    this.st.focus = false;
                } else {
                    this.st.focus = '#name';
                }
            }
        }
    });

    jQuery.fn.shake = function() {
        this.each(function(i) {
            $(this).css({ "position": "relative" });
            for (var x = 1; x <= 2; x++) {
                $(this).animate({ left: -25 }, 10).animate({ left: 0 }, 50).animate({ left: 25 }, 10).animate({ left: 0 }, 50);
            }
        });
        return this;
    };

    $("#login").submit(function (e) {

        var url = $("#login").attr('action');
        var username = $("#username").val();
        var password = $("#password").val();
        var remember = $("#remember").checked == true;

        $.ajax({
            url: url,
            data: { username: username, password: password, remember: remember },
            type: 'POST',
            dataType: "html",
            statusCode: {
                200: function() {
                    location.reload();
                    return true;
                },
                401: function() {
                    //$("#loginIncorrect").removeClass("hidden");
                    $('.white-popup-block').shake();
                    return false;
                }
            }
        });
        return false;
    });
};

$(function () { 
    App.applyMagnificPopup();
    App.applyHighlightJs();
    App.changeLocalization();
    App.showLoginForm();
});
 