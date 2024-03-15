const express = require('express');

let router = express.Router();

router.post('/api/viewStates:sync', async (req, res, next) => {
    console.log(req.body);
    const cameraDef = req.body;

    function camelize(str) {
        return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
            if (+match === 0) return ""; // or if (/\s+/.test(match)) for white spaces
            return index === 0 ? match.toLowerCase() : match.toUpperCase();
        });
    }

    let cameraDefLowerCased = {};
    for (let key in cameraDef) {
        let mappedKey = camelize(key);
        if (key === 'FOV')
            mappedKey = key.toLowerCase();

        cameraDefLowerCased[mappedKey] = cameraDef[key];
    }

    global.socketIO.emit('restoreCameraState', JSON.stringify(cameraDefLowerCased));

    res.status(200).end();
});

module.exports = router;