package main

import (
	"fmt"
	"math"
)

func nums(x float64, n int) func() float64 {
	var exp float64 = 1
	return func() float64 {
		exp += 1
		return math.Pow(float64(-1), exp) * math.Pow(x, exp) / exp
	}
}

func main() {
	var res float64
	x, n := 0.5, 100000000

	getNext := nums(x, n)
    
	res = x
	for i := 2; i != n; i += 1 {
		res += getNext()
	}

	fmt.Println("Result: ", res)
}
