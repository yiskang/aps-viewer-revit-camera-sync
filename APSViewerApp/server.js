const express = require('express');
const http = require('http');
const socket = require('socket.io');

const { PORT } = require('./config.js');

let app = express();
app.use(express.json());
app.use(express.urlencoded({ extended: false }));

app.use(express.static('wwwroot'));
app.use(require('./routes/auth.js'));
app.use(require('./routes/models.js'));
app.use(require('./routes/viewStateSync.js'));

const server = http.createServer(app);
const io = socket(server, {
    pingInterval: 10000,
    pingTimeout: 5000
});

global.socketIO = io;

io.on('connection', socket => {
    console.log('Socket.io init success');

    socket.on('sever disconnect', close => {
        socket.disconnect(close);
    });
});

server.listen(PORT, function () { console.log(`Server listening on port ${PORT}...`); });
