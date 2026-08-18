mergeInto(LibraryManager.library, {
IsMobileBrowser: function() {
        return / Android | iPhone | iPad | iPod | Mobile / i.test(navigator.userAgent) ? 1 : 0;
    }
});