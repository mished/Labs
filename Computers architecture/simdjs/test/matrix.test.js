/* global describe, it, before, SIMD */
'use strict';

const expect = require('chai').expect;
const $Array = Float32Array;
const $SIMD = { type: SIMD.Float32x4, count: 4 };
const _ = require('../src/matrix')($Array, $SIMD);

describe('task', () => {
    let testData;

    function checkDimensions(actualMatrix, expectedMatrix) {
        const expectedHeight = expectedMatrix.length;
        const expectedWidth = expectedMatrix[0].length;
        const actualHeight = actualMatrix.length;
        const actualWidth = actualMatrix[0].length;
        return actualHeight === actualWidth
            && actualHeight === expectedHeight
            && actualWidth === expectedWidth;
    }

    before(() => {
        testData = [
            [
                new $Array([1, 2, 3]),
                new $Array([4, 5, 6]),
                new $Array([7, 8, 9])
            ],
            [
                new $Array([14, 16, 18]),
                new $Array([8, 10, 12]),
                new $Array([2, 4, 6])
            ]
        ];
    });

    describe('transpose', () => {
        it('should transpose matrix', () => {
            const expectedData = [
                [
                    new $Array([1, 4, 7]),
                    new $Array([2, 5, 8]),
                    new $Array([3, 6, 9])
                ],
                [
                    new $Array([14, 8, 2]),
                    new $Array([16, 10, 4]),
                    new $Array([18, 12, 6])
                ]
            ];
            testData.forEach((data, i) => {
                const actual = _.transpose(data);
                expect(actual).to.be.deep.equal(expectedData[i]);
                expect(checkDimensions(actual, expectedData[i])).to.be.true;
            });
        });
    });

    describe('util functions', () => {
        it('should find max line\'s sum', () => {
            const expectedData = [24, 48];
            testData.forEach((data, i) => {
                expect(_.getMaxRowSum(data)).to.be.equal(expectedData[i]);
            });
        });

        it('should find max column\'s sum', () => {
            const expectedData = [18, 36];
            testData.forEach((data, i) => {
                expect(_.getMaxColumnSum(data)).to.be.equal(expectedData[i]);
            });
        });

        it('should generate identity matrix', () => {
            const expected = [
                new $Array([1, 0, 0]),
                new $Array([0, 1, 0]),
                new $Array([0, 0, 1])
            ];
            expect(_.getIdentityMatrix(3)).to.be.deep.equal(expected);
        });
    });

    describe('matrix vs number operations', () => {
        it('should divide matrix on number', () => {
            const numbers = [1, 2];
            const expectedData = [
                testData[0],
                [
                    new $Array([7, 8, 9]),
                    new $Array([4, 5, 6]),
                    new $Array([1, 2, 3])
                ]
            ];
            testData.forEach((data, i) => {
                const actual = _.map(data, x => x / numbers[i]);
                expect(actual).to.be.deep.equal(expectedData[i]);
                expect(checkDimensions(actual, expectedData[i])).to.be.true;
            });
        });
    });

    describe('matrix vs matrix operations', () => {
        it('should multiply matrix', () => {
            const expectedData = [
                new $Array([36, 48, 60]),
                new $Array([108, 138, 168]),
                new $Array([180, 228, 276])
            ];
            const actual = _.multiply(testData[0], testData[1]);
            expect(actual).to.be.deep.equal(expectedData);
            expect(checkDimensions(actual, expectedData)).to.be.true;
        });

        it('should add matrix', () => {
            const expectedData = [
                new $Array([15, 18, 21]),
                new $Array([12, 15, 18]),
                new $Array([9, 12, 15])
            ];
            const actual = _.zip(testData[0], testData[1], (a, b) => a + b);
            expect(actual).to.be.deep.equal(expectedData);
            expect(checkDimensions(actual, expectedData)).to.be.true;
        });

        it('should be raised to the power', () => {
            const expectedData = [
                new $Array([468, 576, 684]),
                new $Array([1062, 1305, 1548]),
                new $Array([1656, 2034, 2412])
            ];
            const actual = _.pow(testData[0], 3);
            expect(actual).to.be.deep.equal(expectedData);
            expect(checkDimensions(actual, expectedData)).to.be.true;
        });
    });

    describe('simd', () => {
        it('should return the same result as non-vect function', () => {
            const N = 20, M = 10;
            const matrix = _.getRandomMatrix(N);
            const expected = _.inverse(matrix, M);
            const actual = _.inverseWithSIMD(matrix, M);
            for (let i = 0; i < N; ++i) {
                for (let j = 0; j < N; ++j) {
                    expect(actual[i][j]).to.be
                        .closeTo(expected[i][j], 1e-6);
                }
            }
        });
    });
});
