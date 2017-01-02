(function () {
	'use strict';

	angular.module('droidrun').directive('drField', [function () {
		return {
			restrict: 'A',
			scope: true,
			template: '<canvas id="dr-field">You\'re missing cool canvas :(</canvas>',
			link: function (scope, elem, attrs) {
				var curField;
				var canvas = document.getElementById('dr-field');
				var ctx = canvas.getContext('2d');
				var fieldSize, cellsCount, cellSize;
				var directionRad = {
					'0': 0,
					'1': Math.PI / 2,
					'2': Math.PI,
					'3': -Math.PI / 2
				};

				scope.socket.on('connected', function (data) {
					console.log(data);
					curField = data;
					drawField(data);
				});

				scope.socket.on('action', function (data) {
					console.log(data);
					angular.forEach(data.changes, function (change) {
						ctx.fillStyle = 'white';
						curField.field[change.x][change.y] = change.newVal;
						drawRectInCell(change.x, change.y, 1);
						processCell(change.x, change.y, change.newVal);
					});
				});

				scope.socket.on('new game', function (data) {
					console.log(data);
					curField = data;
					drawField(data);
				});
				
				window.onresize = function() {
					if(curField) {
						drawField(curField);
					}
				}

				function drawField(data) {
					cellsCount = data.field.length;
					fieldSize = elem[0].offsetWidth;
					cellSize = fieldSize / cellsCount;
					canvas.height = fieldSize;
					canvas.width = fieldSize;

					drawGrid();
					drawElements(data.field);
				}

				function drawGrid() {
					var offset = cellSize;
					ctx.strokeStyle = 'rgba(158, 160, 162, 0.99)';
					ctx.lineWidth = 1;
					ctx.strokeRect(0, 0, fieldSize, fieldSize);

					ctx.lineWidth = 0.5;
					while (offset <= fieldSize) {
						ctx.beginPath();
						ctx.moveTo(0, offset);
						ctx.lineTo(fieldSize, offset);
						ctx.stroke();

						ctx.beginPath();
						ctx.moveTo(offset, 0);
						ctx.lineTo(offset, fieldSize);
						ctx.stroke();

						offset += cellSize;
					}
				}

				function drawElements(field) {
					var i, j;
					for (i = 0; i < cellsCount; i++)
						for (j = 0; j < cellsCount; j++)
							processCell(i, j, field[i][j]);
				}
				
				function processCell(i, j, cell) {
					switch (cell.object) {
								case 'W':
									drawWall(i, j);
									break;
								case 'E':
									break;
								case 'D':
									drawDroid(i, j, cell.value);
									break;
								case 'F':
									drawFinish(i, j);
									break;
							}
				}
				
				function drawWall(i, j) {
					ctx.fillStyle = 'rgba(15, 52, 111, 0.33)';
					drawRectInCell(i, j);
				}

				function drawFinish(i, j) {
					ctx.fillStyle = 'rgba(20, 219, 103, 0.75)';
					drawRectInCell(i, j);
				}
				
				function drawDroid(i, j, droid) {					
					ctx.fillStyle = 'rgba(243, 32, 85, 0.9)';
					drawRotated(i, j, directionRad[droid.direction], drawTrianInCell);
				}
				
				function drawRotated(i, j, rad, drawFn) {
					var x, y;
					x = i * cellSize + cellSize / 2;
					y = j * cellSize + cellSize / 2;
					ctx.save();
					ctx.translate(x, y);
					ctx.rotate(rad);
					ctx.translate(-x, -y);
					drawFn(i, j);
					ctx.restore();
				}
				
				function drawTrianInCell(i, j, margin) {
					var x, y;
					margin = margin || 3;
					x = i * cellSize;
					y = j * cellSize;
					ctx.beginPath();
					ctx.moveTo(x + cellSize / 2, y + margin);
					ctx.lineTo(x + cellSize - margin, y + cellSize - margin);
					ctx.lineTo(x + margin, y + cellSize - margin);
					ctx.closePath()
					ctx.fill();
				}

				function drawRectInCell(i, j, margin) {
					var x, y, w, h;
					margin = margin || 3;
					x = i * cellSize + margin;
					y = j * cellSize + margin;
					w = h = cellSize - margin * 2;
					ctx.fillRect(x, y, w, h);
				}

			}
		};
	}]);
}()); 