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

$(function () { 
    App.applyMagnificPopup();
    App.applyHighlightJs();
    App.changeLocalization();
});
 