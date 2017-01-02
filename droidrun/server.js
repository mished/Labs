'use strict';

const express = require('express');
const app = express();
const server = require('http').Server(app);
const io = require('socket.io')(server);

const GS = require('./game.js');
const gameService = new GS.GameService().start();

let users = {};

app.set('wwwroot', (__dirname + '/www/'));
app.set('port', (process.env.PORT || 5000));

app.use(express.static(app.get('wwwroot')));

app.get('/', (req, res) =>
  res.sendFile(app.get(wwwroot) + 'index.html'));

io.use(function(socket, next) {
    let mes = socket.handshake.query.name + ' connected.';
    users[socket.id] = { name: socket.handshake.query.name };
    socket.broadcast.emit('logMessage', mes);
    socket.emit('logMessage', mes);
    return next();    
});

io.on('connection', (socket) => {
  socket.emit('connected', gameService.getCurrentState());
  
  socket.on('disconnect', () => {
    socket.broadcast.emit('logMessage', users[socket.id].name + ' disconnected.');
    delete users[socket.id];    
  });
  
  socket.on('change name', (data) => {
    io.emit('logMessage', users[socket.id].name + ' changed name to ' + data);
    users[socket.id].name = data;
  });
  
  socket.on('vote', (action) => {
    if(!users[socket.id].voted) {
      io.emit('logMessage', users[socket.id].name + ' votes for ' + '"' + action + '"');
      gameService.addVote(action);
      users[socket.id].voted = true;
    }
  });
});

gameService.on('executing', (data) => {
  io.emit('logMessage', 'Executing command: ' + '"' + data + '"');
});

gameService.on('action', (data) => {
  io.emit('action', data);
  io.emit('logMessage', data.action);
});

gameService.on('new game', (data) => {
  io.emit('new game', data);
  io.emit('logMessage', 'Starting new game.')
});

gameService.on('next round', (data) => {
  io.emit('next round', data);
  for(let key in users) {
    users[key].voted = false;
  }  
});

server.listen(app.get('port'), () =>
  console.log('Node app is running on port', app.get('port')));