'use strict';

module.exports = (function () {

	const util = require('util');
	const EventEmitter = require('events').EventEmitter;

	const Cell = Object.freeze({
		EMPTY: 'E',
		WALL: 'W',
		DROID: 'D',
		FINISH: 'F'
	});

	const Direction = Object.freeze({
		NORTH: 0,
		EAST: 1,
		SOUTH: 2,
		WEST: 3,
		prop: ['north', 'east', 'south', 'west']
	});

	const Action = Object.freeze({
		MOVE: 'move',
		TURNLEFT: 'turn left',
		TURNRIGHT: 'turn right',
		MOVEFAIL: 'move fail'
	});

	const GameStatus = Object.freeze({
		NEWGAME: 'new game',
		ALIVE: 'alive',
		FINISHED: 'finished'
	});

	function Game(options) {
		if (!options) options = {};
		this.size = options.size || 7;
		this.droid = options.droid || { x: 1, y: 1, direction: Direction.SOUTH };
		this.finish = options.finish || { x: this.size, y: this.size };
		this.field = [];
		this.lastAction = undefined;
		this.status = GameStatus.NEWGAME;
	}

	function Change(x, y, newVal) {
		this.x = x;
		this.y = y;
		this.newVal = newVal;
	}

	Game.prototype.initField = function () {
		let size = this.size;
		let field = this.field;
		let droid = this.droid;
		let finish = this.finish;

		field.push(getRaw(size + 2, { 'object': Cell.WALL }));
		for (let i = 1; i < size + 1; i++) {
			field.push(getRaw(size + 2, { 'object': Cell.EMPTY }, { 'object': Cell.WALL }, { 'object': Cell.WALL }));
		}
		field.push(getRaw(size + 2, { 'object': Cell.WALL }));

		field[droid.x][droid.y] = { 'object': Cell.DROID, 'value': droid };
		field[finish.x][finish.y] = { 'object': Cell.FINISH };

		return this;

		function getRaw(len, el, first, last) {
			let r = [];
			for (let i = 0; i < len; i++) {
				r.push(el);
			}
			if (first) r[0] = first;
			if (last) r[len - 1] = last;
			return r;
		}
	}

	Game.prototype.move = function () {
		let targetCell = {};
		let droid = this.droid;
		let oldX = droid.x;
		let oldY = droid.y;

		switch (droid.direction) {
			case Direction.NORTH:
				targetCell.x = droid.x;
				targetCell.y = droid.y - 1;
				break;
			case Direction.SOUTH:
				targetCell.x = droid.x;
				targetCell.y = droid.y + 1;
				break;
			case Direction.WEST:
				targetCell.x = droid.x - 1;
				targetCell.y = droid.y;
				break;
			case Direction.EAST:
				targetCell.x = droid.x + 1;
				targetCell.y = droid.y;
				break;
		}

		if (this.field[targetCell.x][targetCell.y].object === Cell.WALL) {
			this.lastAction = Action.MOVEFAIL;
			this.status = GameStatus.ALIVE;
			return {
				'status': this.status,
				'action': this.lastAction,
				'changes': []
			};
		}

		this.status = (this.field[targetCell.x][targetCell.y].object === Cell.FINISH)
			? GameStatus.FINISHED
			: GameStatus.ALIVE;

		this.lastAction = Action.MOVE;
		this.field[oldX][oldY] = { 'object': Cell.EMPTY };
		this.field[targetCell.x][targetCell.y] = { 'object': Cell.DROID, 'value': droid };
		droid.x = targetCell.x;
		droid.y = targetCell.y;

		return {
			'status': this.status,
			'action': this.lastAction,
			'changes': [
				new Change(oldX, oldY, this.field[oldX][oldY]),
				new Change(targetCell.x, targetCell.y, this.field[targetCell.x][targetCell.y])
			]
		};
	}

	Game.prototype.turnLeft = function () {
		let droid = this.droid;
		droid.direction = turn(droid.direction, Action.TURNLEFT);
		this.status = GameStatus.ALIVE;
		this.lastAction = Action.TURNLEFT;
		this.field[droid.x][droid.y] = { 'object': Cell.DROID, 'value': droid };
		return {
			'status': this.status,
			'action': this.lastAction,
			'changes': [
				new Change(droid.x, droid.y, this.field[droid.x][droid.y])
			]
		};
	}

	Game.prototype.turnRight = function () {
		let droid = this.droid;
		droid.direction = turn(droid.direction, Action.TURNRIGHT);
		this.status = GameStatus.ALIVE;
		this.lastAction = Action.TURNRIGHT;
		this.field[droid.x][droid.y] = { 'object': Cell.DROID, 'value': droid };
		return {
			'status': this.status,
			'action': this.lastAction,
			'changes': [
				new Change(droid.x, droid.y, this.field[droid.x][droid.y])
			]
		};
	}

	function turn(dir, action) {
		let res = (action === Action.TURNLEFT) ? dir - 1 : dir + 1;
		if (res < 0) res = 3;
		if (res > 3) res = 0;
		return res;
	}

	function Votes() {
		this[Action.MOVE] = 0;
		this[Action.TURNLEFT] = 0;
		this[Action.TURNRIGHT] = 0;
	}

	function GameService() {
		this.game = new Game().initField();
		this.votes = new Votes();
	}

	GameService.prototype.getCurrentState = function () {
		return {
			'status': this.game.status,
			'field': this.game.field
		}
	}

	GameService.prototype.start = function (options) {
		if(!options) options = {};
		const interval = options.interval || 4000;

		setInterval(() => {
			if (this.game.status === GameStatus.FINISHED) {
				this.game = new Game().initField();
			}

			if (this.game.status === GameStatus.NEWGAME) {
				this.emit('new game', {
					'status': GameStatus.NEWGAME,
					'field': this.game.field
				});
				this.game.status = GameStatus.ALIVE;
				this.votes = new Votes();
				return;
			}

			let votes = Object.assign({}, this.votes);
			let action = getCommand(votes);
			let result;

			if (action) {
				this.emit('executing', action);
			}

			switch (action) {
				case Action.MOVE:
					result = this.game.move();
					break;
				case Action.TURNLEFT:
					result = this.game.turnLeft();
					break;
				case Action.TURNRIGHT:
					result = this.game.turnRight();
					break;
			}

			if (result) {
				this.emit('action', result);
			}
			this.votes = new Votes();
			this.emit('next round', interval);

		}, interval);

		return this;

		function getCommand(votes) {
			let res = [];
			let max = Number.MIN_VALUE;
			for (let key in votes) {
				if (votes[key] > max)
					max = votes[key];
			}
			for (let key in votes) {
				if (votes[key] === max)
					res.push(key);
			}
			return res[Math.floor(Math.random() * (res.length - 1))];
		}
	}

	GameService.prototype.addVote = function (action) {
		this.votes[action] += 1;
	}

	util.inherits(GameService, EventEmitter);

	return {
		GameService: GameService,
		Cell: Cell,
		Direction: Direction,
		Action: Action,
		GameStatus: GameStatus
	};
}());