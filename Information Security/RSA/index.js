var rsa = require('./src/rsa.js');
module.exports = {
	generateKeys: rsa.generateKeys,
	encode:				rsa.encode,
	decode:				rsa.decode
};