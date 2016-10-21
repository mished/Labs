from random import random as rand
import matplotlib.pyplot as plt
import math


def uniform_dist(a, b):
    r = rand()
    return a + (b - a) * r


def normal_dist(m, sigma):
    n = 6
    r = sum([rand() for _ in range(n)])
    return m + sigma * (12 / n)**0.5 * r - n / 2


def exp_dist(lambd):
    r = rand()
    return -1 / lambd * math.log(r)


def triangular_dist(a, b):
    l, u = a / 2, b / 2
    r1, r2 = uniform_dist(l, u), uniform_dist(l, u)
    return r1 + r2


def draw_normal(m = 0, sigma = 0.8):
    def expected_func(x):
        return 1 / (2 * sigma**2 * math.pi)**0.5 * math.e**(-(x - m)**2 / (2 * sigma**2))

    expected = [(x, expected_func(x)) for x in frange(-5, 6, 0.1)]
    actual_func = lambda: round(normal_dist(m, sigma))
    draw(expected, actual_func)


def draw_exp(lambd = 0.6):
    expected = [(x, lambd * math.e**(-lambd * x)) for x in frange(0, 12, 0.1)]
    actual_func = lambda: int(exp_dist(lambd))
    draw(expected, actual_func)


def draw_uniform(a = 2, b = 10):
    def expected_func(x):
        if x <= a or x >= b: return 0
        return 1.0 / (b - a)

    expected = [(x, expected_func(x)) for x in range(12)]
    actual_func = lambda: round(uniform_dist(a, b))
    draw(expected, actual_func)


def draw_triangular(a = 2, b = 10):
    c = (b - a) / 2 + a
    def expected_func(x):
        if x < a: return 0
        if a <= x < c: return 2 * (x - a) / ((b - a) * (c - a))
        if x == c: return 2 / (b - a)
        if c < x <= b: return 2 * (b - x) / ((b - a) * (b - c))
        return 0

    expected = [(x, expected_func(x)) for x in frange(0, 12, 0.1)]
    actual_func = lambda: round(triangular_dist(a, b))
    draw(expected, actual_func)


def draw(expected, actual_func):
    expected_x, expected_y = zip(*expected)
    actual_dict = {}
    plt.figure()
    plt.ion()
    for value in generate_seq(actual_func, 1000000):
        if (actual_dict.has_key(value)):
            actual_dict[value] += 1
        else:
            actual_dict[value] = 1
    values_count = sum(actual_dict.values())
    actual_x = sorted(actual_dict.keys())
    actual_y = [float(actual_dict[v]) / values_count for v in actual_x]
    plt.clf()
    plt.plot(expected_x, expected_y, 'r', actual_x, actual_y, 'b')
    plt.pause(0.05)


def generate_seq(func, n):
    while n > 0:
        yield func()
        n -= 1


def frange(l, r, s):
    c = l
    while c <= r:
        yield c
        c += s


draw_normal()
draw_exp()
draw_uniform()
draw_triangular()
plt.waitforbuttonpress()