package main

import (
	"fmt"
	"math"
	"time"
)

func nums(x float64, n int) func() float64 {
	var exp float64 = 1
	return func() float64 {
		exp += 1
		return math.Pow(float64(-1), exp) * math.Pow(x, exp) / exp
	}
}

func main() {
	var x, res float64
	var n int

	fmt.Println("x: ")
	fmt.Scanf("%f\n", &x)
	fmt.Println("n: ")
	fmt.Scanf("%d\n", &n)

	getNext := nums(x, n)

	t0 := time.Now()
	res = x
	for i := 1; i != n; i += 1 {
		res += getNext()
	}
	t1 := time.Now()

	fmt.Println("Result: ", res)
	fmt.Println("Elapsed time: ", t1.Sub(t0))
}
