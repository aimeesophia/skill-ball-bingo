window.keyboardInterop = {
    initialize: function (dotNetRef) {
        const handler = function (e) {
            dotNetRef.invokeMethodAsync('OnKeyDown', e.key);
        };
        window.addEventListener('keydown', handler);
        this.handler = handler;
        this.ref = dotNetRef;
    },
    dispose: function () {
        if (this.handler) {
            window.removeEventListener('keydown', this.handler);
            this.ref.dispose();
            this.handler = null;
            this.ref = null;
        }
    }
};
