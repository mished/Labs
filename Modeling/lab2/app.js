const createRandGen = require('../lab1/app').createRandomGenerator;
const log = console.log;

function getUniqValues(gen) {
    const res = [gen.next().value];
    let cur;
    while ((cur = gen.next().value) !== res[0]) {
        res.push(cur);
    }
    return res;
}

const uniqValues = getUniqValues(createRandGen(4));
const N = uniqValues.length;
const M = uniqValues.reduce((a, b) => a + b) / N;
const σ = uniqValues.reduce((a, b) => a + b * b, 0) / N - M * M;

const freqTable = Array.from({ length: 10 }, () => 0);
uniqValues.forEach(x => freqTable[x === 1 ? 9 : Math.trunc(x * 10)] += 1);

const χ = freqTable.reduce((a, b) => a + Math.pow((b - N * 0.1), 2) / (N * 0.1), 0);

let cor = 0;
for (let i = 1; i < N; ++i) {
    cor += (uniqValues[i] - 0.5) * (uniqValues[i - 1] - 0.5);
}
cor = cor * 12 / N;

const cor2 = cor * cor;
const fisher = cor2 / (1 - cor2) * (N - 2);

log('T: ', uniqValues.length);
log('M: ', M);
log('σ: ', σ);
log('Frequencies: ', freqTable);
log('χ: ', χ);
log('Autocorrelation: ', cor);
log('Fisher: ', fisher);
