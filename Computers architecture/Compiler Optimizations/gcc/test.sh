#!/bin/bash
 
logTest() {
    echo "------------------------------------------------------" >> results.txt
    echo "|""$1" >> results.txt
    echo "------------------------------------------------------" >> results.txt
    echo "$1"
}

test() {
    logTest "$1"
    $1
    eval "$2"
}

echo "Starting tests..."
rm results.txt

run="(time ./nums) &>> results.txt"

cmd="gcc nums.c -O0 -lm -o nums"
test "$cmd" "$run"

cmd="gcc nums.c -O1 -lm -o nums"
test "$cmd" "$run"

cmd="gcc nums.c -O2 -lm -o nums"
test "$cmd" "$run"

cmd="gcc nums.c -O3 -lm -o nums"
test "$cmd" "$run"

cmd="gcc nums.c -Os -lm -o nums"
test "$cmd" "$run"

cmd="gcc nums.c -Ofast -lm -o nums"
test "$cmd" "$run"

cmd="gcc nums.c -Og -lm -o nums"
test "$cmd" "$run"