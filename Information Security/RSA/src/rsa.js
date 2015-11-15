(function() {
	'use strict';
	
	let BI = require('big-integer');
	
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
				xy.x = 0;
				xy.y = 1;
				return b;
			}
			
			let tmp = {};
			let gcd = rec(b.divmod(a).remainder, a, tmp);
			
			xy.x = tmp.y - (b.divide(a)).multiply(tmp.x);
			xy.y = tmp.x;
			
			return gcd;
		}(a, b, xy));
		
		return {
			gcd: gcd,
			x: xy.x,
			y: xy.y
		};
	};
	
}());