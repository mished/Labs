'use strict';
let BI  = require('big-integer');
let rsa = require('../../src/rsa.js');

describe('gcd', function() {  
  
  it('should work for small integers', function() {
    let gcd = rsa.gcd(BI(25), BI(15)).value;
    expect(gcd).toBe(5);
  });
  
  it('should work for big integers', function() {
    let a = BI('90071992547409919007199254740991');
    let b = BI('123231212312345356476456324452354345245');
    let gcd = rsa.gcd(a, b).value;
    expect(gcd).toBe(1);
  });
  
  it('should work for big integers', function() {
    let a = BI('2555555555555555555555555555555');
    let b = BI('25555');
    let gcd = rsa.gcd(a, b).value;
    expect(gcd).toBe(5);
  });
});

describe('egcd', function() {
  it('should find Bezout coefficients', function() {
    let egcd = rsa.egcd(BI(17), BI(40));
    expect(egcd.x.value).toBe(-7);
    expect(egcd.y.value).toBe(3);
  });
});