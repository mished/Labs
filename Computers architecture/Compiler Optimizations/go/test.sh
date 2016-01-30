#!/bin/bash
 
logTest() {
    echo "------------------------------------------------------" >> results.txt
    echo "|""$1" >> results.txt
    echo "------------------------------------------------------" >> results.txt
    echo "$1"
}

echo "Starting tests..."
rm results.txt

run="(time ./nums) &>> results.txt"

cmd="go build nums.go"
logTest "$cmd"
$cmd
eval $run

cmd="go build -compiler gccgo -gccgoflags '-O0'"
logTest "$cmd"
$cmd
eval $run

cmd="go build -compiler gccgo -gccgoflags '-O1'"
logTest "$cmd"
$cmd
eval $run

cmd="go build -compiler gccgo -gccgoflags '-O2'"
logTest "$cmd"
$cmd
eval $run

cmd="go build -compiler gccgo -gccgoflags '-O3'"
logTest "$cmd"
$cmd
eval $run

cmd="go build -compiler gccgo -gccgoflags '-Ofast'"
logTest "$cmd"
$cmd
eval $run