/* global SIMD */
'use strict';

const _ = require('./src/matrix')(Float32Array,
    { type: SIMD.Float32x4, count: 4 }); // only x4 types for now

const N = 512, M = 10;
const matrix = _.getRandomMatrix(N);
console.time('no-simd');
_.inverse(matrix, M);
console.timeEnd('no-simd');

console.time('simd');
_.inverseWithSIMD(matrix, M);
console.timeEnd('simd');