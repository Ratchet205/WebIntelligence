const express = require('express');
const https = require('https');
const http = require('http');
const socketIO = require('socket.io');
const fs = require('fs');

const app = express();
const server = http.createServer(app);

app.use(express.static('public'));

const port = 3000;

server.listen(port, () => {
    console.log(`Server listening on http://localhost`);
});