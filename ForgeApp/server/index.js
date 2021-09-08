/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

const jsonServer = require('json-server');
const http = require('http');
const socket = require('socket.io');
const path = require('path');

const { DIRNAME } = require('./expose');
const routes = require('./routes.json');
const config = require('./config');

const PORT = config.port;
if (config.credentials.client_id == null || config.credentials.client_secret == null) {
    console.error('Missing FORGE_CLIENT_ID or FORGE_CLIENT_SECRET env. variables.');
    return;
}

const dbFile = path.join(DIRNAME, 'db.json');
const app = jsonServer.create();
const foreignKeySuffix = '_id';
const router = jsonServer.router(dbFile, { foreignKeySuffix });

const defaultsOpts = {
    static: path.join(DIRNAME, 'www'),
    bodyParser: true
};
const middleware = jsonServer.defaults(defaultsOpts);
const rewriter = jsonServer.rewriter(routes);

app.use(middleware);
app.use('/api/forge/views', require('./routes/views'));
app.use('/api/forge/oauth', require('./routes/oauth'));
app.use('/api/forge/oss', require('./routes/oss'));
app.use('/api/forge/modelderivative', require('./routes/modelderivative'));
app.use((err, req, res, next) => {
    if (!err) {
        next();
    } else {
        console.error(err);
        res.status(err.statusCode).json(err);
    }
});

app.use(rewriter);
app.use(router);

const server = http.createServer(app);
const io = socket(server, {
    pingInterval: 10000,
    pingTimeout: 5000
});

global.socketIO = io;

io.on('connection', socket => {
    console.log('Socket.io init success');

    socket.on('camera sync', (data) => {
        console.log(data);

        const cameraDef = JSON.parse(data);

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

        socket.emit('update view', JSON.stringify(cameraDefLowerCased));
    });

    socket.on('sever disconnect', close => {
        socket.disconnect(close);
    });
});

server.listen(PORT, undefined, () => {
    console.log('JSON API app running on port %d', PORT);
});