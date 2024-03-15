
(function () {
    class DasRevitCameraSyncExtension extends Autodesk.Viewing.Extension {
        constructor(viewer, options) {
            super(viewer, options);
        }

        restoreViewState(viewData) {
            const view = {
                aspect: viewData.aspect,
                isPerspective: viewData.isPerspective,
                fov: viewData.fov,
                position: new THREE.Vector3().fromArray(viewData.position),
                target: new THREE.Vector3().fromArray(viewData.target),
                up: new THREE.Vector3().fromArray(viewData.up),
                orthoScale: viewData.orthoScale
            };

            let model = this.viewer.getAllModels()[0];
            const offsetMatrix = model.getModelToViewerTransform();
            view.position = view.position.applyMatrix4(offsetMatrix);
            view.target = view.target.applyMatrix4(offsetMatrix);
            console.log(view);
            this.viewer.impl.setViewFromCamera(view);
        }

        getViewState() {
            let viewState = this.viewer.getState({ viewport: true });
            const { viewport } = viewState;

            let model = this.viewer.getAllModels()[0];
            const invOffsetMatrix = model.getInverseModelToViewerTransform();

            var eyeOffset = new THREE.Vector3().fromArray(viewport.eye).applyMatrix4(invOffsetMatrix);
            var targetOffset = new THREE.Vector3().fromArray(viewport.target).applyMatrix4(invOffsetMatrix);

            // Project back to Revit coordinate space
            viewport.eye = eyeOffset.toArray();
            viewport.target = targetOffset.toArray();

            const view = {
                aspect: viewport.aspectRatio,
                isPerspective: !viewport.isOrthographic,
                fov: viewport.fieldOfView,
                position: eyeOffset.toArray().concat(),
                target: targetOffset.toArray().concat(),
                up: viewport.up.concat(),
                orthoScale: viewport.orthographicHeight ?? 1
            };

            return view;
        }

        async initializeSocketIo() {
            if (this.socketIoTool)
                this.deinitializeSocket();

            const baseUrl = window.location.origin;
            await Autodesk.Viewing.Private.theResourceLoader.loadScript(
                `${baseUrl}/socket.io/socket.io.js`,
                'io'
            );

            const socket = io.connect();

            socket.on('restoreCameraState', (data) => {
                //console.log(data);
                const viewData = JSON.parse(data);
                this.restoreViewState(viewData);
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

    Autodesk.Viewing.theExtensionManager.registerExtension('Autodesk.DAS.RevitCameraSync', DasRevitCameraSyncExtension);
})();