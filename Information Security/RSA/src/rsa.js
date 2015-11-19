'use strict';

let BI = require('big-integer');
let conv = require('binstring');

function generateKeys(capacity) {
  let keyLen = capacity || 1024;
  let min = BI(2).pow(keyLen / 2 - 1);
  let max = BI(2).pow(keyLen / 2);
  let minDiff = BI(10).pow(10);
  let p = getPrimeBetween(min, max);
  let q = getPrimeBetween(min, max);
  while (p.minus(q).abs().compareTo(minDiff) === -1) {
    q = getPrimeBetween(min, max);
  }
  let n = p.multiply(q);
  let f = p.minus(1).multiply(q.minus(1));
  //let e = getPublicExp(f);
  let e = getFermatPrime();
  let d = getPrivateExp(e, f);

  return {
    'public': e.toString(32) + ':' + n.toString(32),
    'private': d.toString(32) + ':' + n.toString(32)
  };
}

function encode(text, pubKey) {
  let key = pubKey.split(':');
  let e = BI(key[0], 32);
  let n = BI(key[1], 32);
  let byteArr = conv(text, { 'in': 'utf8', 'out': 'bytes' });
  let strArr = byteArr.map((x) => {
    let str = x.toString();
    while (str.length !== 3)
      str = '0' + str;
    return str;
  });
  return BI('1' + strArr.join('')).modPow(e, n).toString(32);
}

function decode(text, privKey) {
  let key = privKey.split(':');
  let d = BI(key[0], 32);
  let n = BI(key[1], 32);
  let strArr = BI(text, 32).modPow(d, n).toString().slice(1);
  let byteArr = [];
  for (let i = 0; i < strArr.length - 2; i += 3) {
    let ch = strArr.slice(i, i + 3);
    if (ch[0] === '0' && ch[1] === '0') {
      ch = ch.slice(2);
    } else if (ch[0] === '0') {
      ch = ch.slice(1);
    }
    byteArr.push(parseInt(ch, 10));
  }
  return conv(byteArr, { 'in': 'bytes', 'out': 'utf8' });
}

function getPrivateExp(e, f) {
  let d = egcd(e, f).x;
  return d.isNegative() ? d.plus(f) : d;
}

function getPublicExp(euler) {
  let e = BI.randBetween(2, euler);
  while (gcd(e, euler).compareTo(1) !== 0) {
    e = BI.randBetween(2, euler);
  }
  return e;
}

function getFermatPrime() {
  let primes = [3, 5, 13, 257, 65537];
  return BI(primes[Math.floor(Math.random() * (4))]);
}

function getPrimeBetween(min, max) {
  let p = BI.randBetween(min, max);
  while (!p.isPrime()) {
    p = BI.randBetween(min, max);
  }
  return p;
}

function gcd(a, b) {
  if (a.compareTo(0) === 0) {
    return b;
  }
  return gcd(b.divmod(a).remainder, a);
};

function egcd(a, b) {
  let xy = {};

  let gcd = (function rec(a, b, xy) {
    if (a.compareTo(0) === 0) {
      xy.x = BI(0);
      xy.y = BI(1);
      return b;
    }

    let tmp = {};
    let gcd = rec(b.divmod(a).remainder, a, tmp);

    xy.x = tmp.y.minus((b.divide(a)).multiply(tmp.x));
    xy.y = tmp.x;

    return gcd;
  }(a, b, xy));

  return {
    gcd: gcd,
    x: xy.x,
    y: xy.y
  };
};

module.exports = {
  generateKeys: generateKeys,
  encode: encode,
  decode: decode,
  getPrivateExp: getPrivateExp,
  getPublicExp: getPublicExp,
  getFermatPrime: getFermatPrime,
  getPrimeBetween: getPrimeBetween,
  gcd: gcd,
  egcd: egcd
};