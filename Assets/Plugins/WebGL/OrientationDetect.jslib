using UnityEditor.PackageManager.UI;

mergeInto(LibraryManager.library, {
IsPortrait: function() {
        return window.matchMedia("(orientation: portrait)").matches ? 1 : 0;
    },

  RegisterOrientationListener: function(gameObjectNamePtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var mediaQuery = window.matchMedia("(orientation: portrait)");

        var notify = function() {
            var isPortrait = mediaQuery.matches ? "1" : "0";
            SendMessage(gameObjectName, "HandleOrientationChanged", isPortrait);
        }
        ;

        mediaQuery.addEventListener("change", notify);
        window.addEventListener("resize", notify);
    }
});