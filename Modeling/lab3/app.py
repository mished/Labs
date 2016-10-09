from random import random as rand
import matplotlib.pyplot as plt
import math

def uniform_dist(a, b):
    r = rand()
    return (a + (b - a) * r, r)

def normal_dist(m, sigma):
    n = 6
    r = sum([rand() for _ in range(n)])
    return (m + sigma * (12 / n)**0.5 * r - n / 2, r)

def exp_dist(lambd):
    r = rand()
    return (-1 / lambd * math.log(r), r)

def triangular_dist(a, b):
    l, u = a / 2, b / 2
    r1, r2 = uniform_dist(l, u), uniform_dist(l, u)
    return (r1[0] + r2[0], r1[1] + r2[1])

def draw_normal():
    m = 0
    sigma = 0.8
    def expected_func(x):
        return 1 / (2 * sigma**2 * math.pi)**0.5 * math.e**(-(x - m)**2 / (2 * sigma**2))

    expected = [(x, expected_func(x)) for x in frange(-5, 6, 0.1)]
    actual = sorted(generate_seq(lambda: normal_dist(m, sigma), 100))
    draw(expected, actual)

def draw_exp():
    lambd = 0.8
    expected = [(x, lambd * math.e**(-lambd * x)) for x in frange(0, 6, 0.1)]
    actual = sorted(generate_seq(lambda: exp_dist(lambd), 100))
    draw(expected, actual)

def draw_uniform():
    a, b = 2, 10
    def expected_func(x):
        if x < a or x > b: return 0
        return 1 / (b - a)

    expected = [(x, expected_func(x)) for x in range(12)]
    actual = sorted(generate_seq(lambda: uniform_dist(a, b), 100))
    draw(expected, actual)

def draw(expected, actual):
    expected_x, expected_y = zip(*expected)
    actual_x, actual_y = zip(*actual)
    plt.plot(expected_x, expected_y, 'r', actual_x, actual_y, 'b')
    plt.show()

def generate_seq(func, n):
    while n > 0:
        yield func()
        n -= 1

def frange(l, r, s):
    c = l
    while c <= r:
        yield c
        c += s

def get_pirson_criteria(rand_func, n):
    intervals_count = 10
    values = [rand_func() for _ in range(n)]
    min_val, max_val = math.floor(min(values)), math.ceil(max(values))
    step = float(max_val - min_val) / intervals_count
    cur = min_val + step
    pirson = 0
    while cur <= max_val:
        count = sum([1 for v in values if cur - step < v <= cur])
        pirson += (count - n * step)**2 / (n * step)
        cur += step

    return pirson