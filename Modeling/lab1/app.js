/**
 * t = 13
 */

function* getGenerator(t) {
    let x0 = 123;
    const lambda = Math.pow(8, t) * t - 3;
    const g = 16;
    const m = Math.pow(2, g);
    x0 = getNextStep(x0);
    while (true) {
        x0 = getNextStep(x0);
        yield x0 / m;
    }

    function getNextStep (x) {
        return parseInt((lambda * x).toString(2).slice(-g), 2);
    }
}

function getValues() {
    const myGenerator = getGenerator(13);
    let range = 100;
    while (range--) {
        console.log(myGenerator.next().value);
    }
}

module.exports = {
    createRandomGenerator: getGenerator
};