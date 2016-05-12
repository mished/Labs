(function e(t,n,r){function s(o,u){if(!n[o]){if(!t[o]){var a=typeof require=="function"&&require;if(!u&&a)return a(o,!0);if(i)return i(o,!0);var f=new Error("Cannot find module '"+o+"'");throw f.code="MODULE_NOT_FOUND",f}var l=n[o]={exports:{}};t[o][0].call(l.exports,function(e){var n=t[o][1][e];return s(n?n:e)},l,l.exports,e,t,n,r)}return n[o].exports}var i=typeof require=="function"&&require;for(var o=0;o<r.length;o++)s(r[o]);return s})({1:[function(require,module,exports){
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
},{"./src/matrix":2}],2:[function(require,module,exports){
'use strict';

module.exports = ($Array, $SIMD) => {
    function transpose(matrix) {
        const len = matrix.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = i; j < len; ++j) {
                res[i][j] = matrix[j][i];
                res[j][i] = matrix[i][j];
            }
        }
        return res;
    }

    function getMaxRowSum(matrix) {
        const len = matrix.length;
        let max = Number.MIN_VALUE;
        for (let i = 0; i < len; ++i) {
            let sum = 0;
            for (let j = 0; j < len; ++j) {
                sum += matrix[i][j];
            }
            if (sum > max) { max = sum; }
        }
        return max;
    }

    function getMaxColumnSum(matrix) {
        const len = matrix.length;
        let max = Number.MIN_VALUE;
        for (let i = 0; i < len; ++i) {
            let sum = 0;
            for (let j = 0; j < len; ++j) {
                sum += matrix[j][i];
            }
            if (sum > max) { max = sum; }
        }
        return max;
    }

    function map(matrix, func) {
        const len = matrix.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; ++j) {
                res[i][j] = func(matrix[i][j]);
            }
        }
        return res;
    }

    function mapVect(matrix, vectFunc) {
        const len = matrix.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; j += $SIMD.count) {
                const vect = simdLoad(matrix[i], j);
                const resVect = vectFunc(vect);
                simdStore(res[i], j, resVect);
            }
        }
        return res;
    }

    const simdLoad = (function () {
        return $SIMD.type.load
            || function (arr, from) {
                return $SIMD.type(...arr.slice(from, from + $SIMD.count));
            };
    } ());

    const simdStore = (function () {
        return $SIMD.type.store
            || function (target, index, value) {
                for (let k = 0; k < $SIMD.count; ++k) {
                    target[index + k] = $SIMD.type.extractLane(value, k);
                }
            };
    } ());

    function multiply(left, right) {
        const len = left.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; ++j) {
                for (let k = 0; k < len; ++k) {
                    res[i][j] += left[i][k] * right[k][j];
                }
            }
        }
        return res;
    }

    function multiplyTransposed(left, right) {
        const len = left.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; ++j) {
                for (let k = 0; k < len; ++k) {
                    res[i][j] += left[i][k] * right[j][k];
                }
            }
        }
        return res;
    }

    function multiplyTransposedVect(left, right) {
        const len = left.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; ++j) {
                for (let k = 0; k < len; k += $SIMD.count) {
                    const leftVect = simdLoad(left[i], k);
                    const rightVect = simdLoad(right[j], k);
                    const resVect = $SIMD.type.mul(leftVect, rightVect);
                    res[i][j] += $SIMD.type.extractLane(resVect, 0);
                    res[i][j] += $SIMD.type.extractLane(resVect, 1);
                    res[i][j] += $SIMD.type.extractLane(resVect, 2);
                    res[i][j] += $SIMD.type.extractLane(resVect, 3);
                }
            }
        }
        return res;
    }

    function zip(left, right, func) {
        const len = left.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; ++j) {
                res[i][j] = func(left[i][j], right[i][j]);
            }
        }
        return res;
    }

    function zipVect(left, right, vectFunc) {
        const len = left.length;
        const res = getMatrix(len);
        for (let i = 0; i < len; ++i) {
            for (let j = 0; j < len; j += $SIMD.count) {
                const leftVect = simdLoad(left[i], j);
                const rightVect = simdLoad(right[i], j);
                const resVect = vectFunc(leftVect, rightVect);
                simdStore(res[i], j, resVect);
            }
        }
        return res;
    }

    function pow(matrix, exp) {
        let res = Array.from(matrix);
        matrix = transpose(matrix);
        while (exp !== 1) {
            res = multiplyTransposed(res, matrix);
            exp -= 1;
        }
        return res;
    }

    function powVect(matrix, exp) {
        let res = Array.from(matrix);
        matrix = transpose(matrix);
        while (exp !== 1) {
            res = multiplyTransposedVect(res, matrix);
            exp -= 1;
        }
        return res;
    }

    function inverse(matrix, count) {
        const N = matrix.length, M = count;
        const I = getIdentityMatrix(N);
        const A = Array.from(matrix);
        const div = getMaxColumnSum(A) * getMaxRowSum(A);
        const B = map(transpose(A), x => x / div);
        const R = zip(I, multiply(B, A), (a, b) => a - b);

        const rowValues = [
            I,
            ...Array.from({ length: M - 1 }, (x, i) => pow(R, i + 1))
        ];

        const rowValuesSum = rowValues.reduce((p, c) =>
            zip(p, c, (a, b) => a + b)
        );

        return multiply(rowValuesSum, B);
    }

    function inverseWithSIMD(matrix, count) {
        const N = matrix.length, M = count;
        const I = getIdentityMatrix(N);
        const A = Array.from(matrix);
        const div = getMaxColumnSum(A) * getMaxRowSum(A);
        const B = mapVect(transpose(A),
            vect => $SIMD.type.div(vect, $SIMD.type.splat(div)));
        const R = zipVect(I, multiply(B, A), $SIMD.type.sub);

        const rowValues = [
            I,
            ...Array.from({ length: M - 1 }, (x, i) => powVect(R, i + 1))
        ];

        const rowValuesSum = rowValues.reduce((p, c) =>
            zipVect(p, c, $SIMD.type.add)
        );

        return multiply(rowValuesSum, B);
    }

    function getMatrix(len) {
        return Array.from({ length: len }, () => new $Array(len));
    }

    function getIdentityMatrix(len) {
        return Array.from({ length: len }, (_, i) => {
            const row = new $Array(len);
            row[i] = 1;
            return row;
        });
    }

    function getRandomMatrix(len) {
        const res = Array.from({ length: len }, () => new $Array(len));
        return map(res, () => Math.random() * 100.5 - 50.25);
    }

    function stringify(matrix) {
        return matrix.join('\n');
    }

    return {
        transpose,
        getMaxColumnSum,
        getMaxRowSum,
        map,
        mapVect,
        multiply,
        zip,
        zipVect,
        pow,
        getIdentityMatrix,
        stringify,
        getRandomMatrix,
        inverse,
        inverseWithSIMD
    };
};
},{}]},{},[1]);
