(function ($) {
    $.extend({
        ajaxLoad: function (containerId, url, data, success, type) {
            var options = {
                url: url,
                type: type || 'GET',
                dataType: 'html',
                data: data
            }
            if (window.FormData && data instanceof FormData) {
                options.processData = false;
                options.contentType = false;
            }
            $.ajax(options)
            .success(function (result, status, xhr) {
                if (xhr.getResponseHeader('X-Ajax-Redirect') != null)
                    location.href = xhr.getResponseHeader('X-Ajax-Redirect');
                else {
                    $('#' + containerId).html(result);
                    if (typeof (_gaq) != "undefined") _gaq.push(['_trackPageview', url]);
                    if (success != undefined) success(result, status, xhr);
                }
            })
            .error(function (xhr, status) {
                $('#' + containerId).html(xhr.responseText);
            })
        }
    });
})(jQuery);

(function ($, window) {
    $.fn.ajaxLoad = function (options) {
        var settings = $.extend({
            url: null,
            customAction: 'html',
            type: 'GET',
            data: {}
        }, options, {
            datatype: 'html'
        });

        var $this = $(this);

        var deferred = $.Deferred();

        if (!settings.url) throw new Error("url is required");

        if (window.FormData && settings.data instanceof FormData) {
            settings.processData = false;
            settings.contentType = false;
        }

        $.ajax(settings)
            .done(function (result, status, xhr) {
                if (xhr.getResponseHeader('X-Ajax-Redirect') != null)
                    location.href = xhr.getResponseHeader('X-Ajax-Redirect');
                else {
                    if (settings.customAction == 'html') $this.html(result);
                    else if (settings.customAction == 'append') $this.append(result);
                    else if (settings.customAction == 'replaceWith') $this.replaceWith(result);
                    if (typeof (_gaq) != "undefined") _gaq.push(['_trackPageview', settings.url]);
                    deferred.resolve(result);
                }
            })
            .fail(function (xhr, status) {
                $this.html(xhr.responseText);
                deferred.reject.apply(deferred, arguments);
            });
        return deferred.promise();
    };
})(jQuery, window);