# Simple Viewer (Node.js) with view state sync support

![platforms](https://img.shields.io/badge/platform-windows%20%7C%20osx%20%7C%20linux-lightgray.svg)
[![node.js](https://img.shields.io/badge/Node.js-16.16-blue.svg)](https://nodejs.org)
[![npm](https://img.shields.io/badge/npm-8.11-blue.svg)](https://www.npmjs.com/)
[![license](https://img.shields.io/:license-mit-green.svg)](https://opensource.org/licenses/MIT)

This sample is a revision of the [Simple Viewer](https://tutorials.autodesk.io/tutorials/simple-viewer/) tutorial
from https://tutorials.autodesk.io with view state sync support for syncing camera state between Revit and viewer.

![thumbnail](thumbnail.png)

## Development

### Prerequisites

- [APS credentials](https://forge.autodesk.com/en/docs/oauth/v2/tutorials/create-app)
- [Node.js](https://nodejs.org) (Long Term Support version is recommended)
- Command-line terminal such as [PowerShell](https://learn.microsoft.com/en-us/powershell/scripting/overview)
or [bash](https://en.wikipedia.org/wiki/Bash_(Unix_shell)) (should already be available on your system)

> We recommend using [Visual Studio Code](https://code.visualstudio.com) which, among other benefits,
> provides an [integrated terminal](https://code.visualstudio.com/docs/terminal/basics) as well.

### Setup & Run

- Clone this repository: `git clone https://github.com/autodesk-platform-services/aps-simple-viewer-nodejs`
- Go to the project folder: `cd aps-simple-viewer-nodejs`
- Install Node.js dependencies: `npm install`
- Open the project folder in a code editor of your choice
- Create a _.env_ file in the project folder, and populate it with the snippet below,
replacing `<client-id>` and `<client-secret>` with your APS Client ID and Client Secret:

```bash
APS_CLIENT_ID="<client-id>"
APS_CLIENT_SECRET="<client-secret>"
```

- Run the application, either from your code editor, or by running `npm start` in terminal
- Open http://localhost:8080

> When using [Visual Studio Code](https://code.visualstudio.com), you can run & debug
> the application by pressing `F5`.


### Camera Sync functions

- Get current viewer camera state for restoring back to Revit Desktop
    ```js
    // Type and execute below line in the web browser dev console to get view state string
    JSON.stringify(NOP_VIEWER.getExtension('Autodesk.DAS.RevitCameraSync').getViewState())
    ```

- Restore view camera from Revit Desktop
    1. Run this app first
    2. Open http://localhost:8080 and view the same model opened in Revit
    3. Execute `SyncCamToWebViewerCommand` addin from Revit Desktop. Here is core logic on how it work
        - Revit will send view state to this app.
        - While receiving view state, this app backend will pass the data via WebSocket to viewer.

## Troubleshooting

Please contact us via https://forge.autodesk.com/en/support/get-help.

# License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.

## Written by

Eason Kang [in/eason-kang-b4398492/](https://www.linkedin.com/in/eason-kang-b4398492), [Developer Advocate](http://aps.autodesk.com)
