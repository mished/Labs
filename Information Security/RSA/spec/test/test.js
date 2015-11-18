'use strict';

describe("RSA module", function() {
  let BI  = require('big-integer');
  let rsa = require('../../src/rsa.js');
  
  it("gcd should work for small integers", function() {
    expect(rsa.gcd(BI(25), BI(15)).value).toBe(BI(5).value);
  });
});