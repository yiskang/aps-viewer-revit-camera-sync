
(function () {
    class AdnRevitCameraSyncExtension extends Autodesk.Viewing.Extension {
        constructor(viewer, options) {
            super(viewer, options);
        }

        async initializeSocketIo() {
            if (this.socketIoTool)
                this.deinitializeSocket();

            const baseUrl = window.location.href.split('?')[0];
            await Autodesk.Viewing.Private.theResourceLoader.loadScript(
                `${baseUrl}socket.io/socket.io.js`,
                'io'
            );

            const socket = io.connect();

            socket.on('update view', (data) => {
                console.log(data);
                const viewData = JSON.parse(data);

                const view = {
                    aspect: viewData.aspect,
                    isPerspective: viewData.isPerspective,
                    fov: viewData.fov,
                    position: new THREE.Vector3().fromArray(viewData.position),
                    target: new THREE.Vector3().fromArray(viewData.target),
                    up: new THREE.Vector3().fromArray(viewData.up),
                    orthoScale: viewData.orthoScale
                };

                const offsetMatrix = this.viewer.model.getModelToViewerTransform();
                view.position = view.position.applyMatrix4(offsetMatrix);
                view.target = view.target.applyMatrix4(offsetMatrix);
                this.viewer.impl.setViewFromCamera(view);
            });

            this.socket = socket;
        }

        async deinitializeSocket() {
            if (!this.socket) return;

            delete this.socket;
            this.socket = null;
        }

        async ensureConnected2SocketIo() {
            await this.initializeSocketIo();
        }

        load() {
            this.ensureConnected2SocketIo();
            return true;
        }

        unload() {
            this.deinitializeSocket();
            return true;
        }
    }

    Autodesk.Viewing.theExtensionManager.registerExtension('Autodesk.ADN.RevitCameraSync', AdnRevitCameraSyncExtension);
})();