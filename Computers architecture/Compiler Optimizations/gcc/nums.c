#include <stdio.h>
#include <math.h>

int main() {
    double x = 0.5;
    double n = 100000000;
    
    double res = x;
    double exp = 2;
    for (exp; exp <= n; exp++) {
        res += pow(-1, exp) * pow(x, exp) / exp;
    }
    printf("Result: %f", res);
    
    return 0;
}