(function() {
	'use strict';
	
	let BI = require('big-integer');
	
	function generateKeys(capacity) {
		let keyLen = capacity || 1024;
		let min = BI(2).pow(keyLen / 2 - 1);
		let max = BI(2).pow(keyLen / 2);
		let minDiff = BI(10).pow(10);
		let p = getPrimeBetween(min, max);
		let q = getPrimeBetween(min, max);
		while(p.minus(q).abs().compareTo(minDiff) === -1) {
			q = getPrimeBetween(min, max);
		}
		let n = p.multiply(q);
		let f = p.minus(1).multiply(q.minus(1));
		let e = getPublicExp(f);
		let d = getPrivateExp(e, f);
		
		return {
			'public' : e.toString(32) + ':' + n.toString(32),
			'private': d.toString(32) + ':' + n.toString(32)
		};
	}
	
	function getPrivateExp(e, f) {
		let d = egcd(e, f).x;
		return d.isNegative() ? d.plus(f) : d; 
	}
	
	function getPublicExp(euler) {
		let e = BI.randBetween(2, euler);
		while(gcd(e, euler).compareTo(1) !== 0) {
			e = BI.randBetween(2, euler);
		}
		return e;
	}
	
	function getPrimeBetween(min, max) {
			let p = BI.randBetween(min, max);
			while(!p.isPrime()) {
				p = BI.randBetween(min, max);
			}
			return p;
		}
	
	function gcd(a, b) {
		if(a.compareTo(0) === 0) {
			return b;
		}
		return gcd(b.divmod(a).remainder, a);
	};
	
	function egcd(a, b) {		
		let xy = {};
		
		let gcd = (function rec(a, b, xy) {
			if(a.compareTo(0) === 0) {
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
		getPrivateExp: getPrivateExp,
		getPublicExp: getPublicExp,
		getPrimeBetween: getPrimeBetween,
		gcd: gcd,
		egcd: egcd
	};
	
}());