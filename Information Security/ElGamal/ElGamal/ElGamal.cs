using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace ElGamal {

    public static class ElGamal {

        public static Keys GenerateKeys(int n) {
            var p = GetBigRandomInt(n);
            while (!p.isProbablePrime())
                p = GetBigRandomInt(n);

            var g = GetPrimitiveRoot(p);
            var x = GetBigRandomInt(n / 2);
            var y = BigInteger.ModPow(g, x, p);

            return new Keys {
                PublicKey = $"{p.ToString()}:{g.ToString()}:{y.ToString()}",
                PrivateKey = x.ToString()
            };
        }

        private static BigInteger GetPrimitiveRoot(BigInteger modulo) {
            var phi = modulo - 1;
            var fact = Factorize(phi);
            for (var i = new BigInteger(2); i < modulo; i++) {
                var ok = true;
                for (var j = 0; j < fact.Count && ok; j++)
                    ok &= (BigInteger.ModPow(i, phi / fact[j], modulo) != 1);
                if (ok)
                    return i;
            }
            return -1;
        }

        private static IList<BigInteger> Factorize(BigInteger num) {
            var res = new List<BigInteger>();
            var n = num;
            for (var i = 2; i * i <= n; i++)
                if (n % i == 0) {
                    res.Add(i);
                    while (n % i == 0)
                        n /= i;
                }
            if (n > 1)
                res.Add(n);

            return res;
        }

        private static bool isProbablePrime(this BigInteger num) {
            var exp = num - 1;
            for (var i = 1; i < 50; i++) {
                if (BigInteger.ModPow(i, exp, num) != 1)
                    return false;
            }
            return true;
        }

        private static BigInteger GetBigRandomInt(int n) {
            var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[n / 8];
            rng.GetBytes(bytes);
            var res = new BigInteger(bytes);
            while (res <= 0) {
                rng.GetBytes(bytes);
                res = new BigInteger(bytes);
            }
            return res;
        }
    }

    public class Keys {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}