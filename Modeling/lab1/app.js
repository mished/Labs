/**
 * t = 13
 */

function* getGenerator(t) {
    let x0 = 123;
    const lambda = Math.pow(8, t) * t - 3;
    const m = Math.pow(2, 16);
    while (true) {
        x0 = (lambda * x0) % m; 
        yield x0 / m;
    }
}

const myGenerator = getGenerator(13);
let range = 100;
while (range--) {
    console.log(myGenerator.next().value);
}