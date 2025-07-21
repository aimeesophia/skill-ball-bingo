window.keyboardInterop = {
    initialize: function (dotNetRef) {
        window.addEventListener('keydown', function (e) {
            dotNetRef.invokeMethodAsync('OnKeyDown', e.key);
        });
    }
};
