import numpy as np
from math import hypot

class Bitmap():
    def __init__(self, width, height, data):
        self._width = width
        self._height = height
        self._data = np.array(data, dtype=np.int16)


    def get(self, x, y):
        return self._data[self._width * y + x]


    def get_safe(self, x, y):
        if (x < 0): x = 0
        if (x >= self._width): x = self._width - 1
        if (y < 0): y = 0
        if (y >= self._height): y = self._height - 1
        return self.get(x, y)


    def set(self, x, y, color):
        self._data[self._width * y + x] = color


    def get_data(self):
        return np.array(self._data, dtype=np.uint8)


    def get_size(self):
        return (self._width, self._height)


    def roberts_cross(self):
        res = Bitmap(self._width, self._height, self._data)
        for y in range(self._height):
            for x in range(self._width):
                t1 = self.get_safe(x, y) - self.get_safe(x+1, y+1)
                t2 = self.get_safe(x+1, y) - self.get_safe(x, y+1)
                res.set(x, y, hypot(t1, t2))
        return res


    def scale(self, coef):
        def calc_pix(x, y):
            tx = x / coef
            ty = y / coef
            siblings = get_siblings(tx, ty)
            try:
                return next(self.get(s.x, s.y) for s in siblings if s.d == 0)
            except StopIteration:
                w = sum([1 / s.d for s in siblings])
                return reduce(lambda p, c: p + self.get(c.x, c.y) / (c.d * w), siblings, 0)

        def get_siblings(x, y):
            _tx = round(x)
            _ty = round(y)
            return [get_point(tx, ty, x, y) for tx, ty in siblings_list(_tx, _ty) if in_range(tx, ty)]

        def siblings_list(x, y):
            yield (x, y)
            yield (x - 1, y)
            yield (x + 1, y)
            yield (x, y - 1)
            yield (x, y + 1)

        def get_point(tx, ty, x, y):
            delta = abs(tx - x) + abs(ty - y)
            return Point(tx, ty, delta)

        def in_range(x, y):
            return (x >= 0 and x < self._width
                and y >= 0 and y < self._height)

        class Point():
            def __init__(self, x, y, d):
                self.x = int(x)
                self.y = int(y)
                self.d = d

        res_width = int(self._width * coef)
        res_height = int(self._height * coef)
        res = Bitmap(res_width, res_height, np.empty(res_height * res_width, dtype=np.int16))
        for y in range(res_height):
            for x in range(res_width):
                res.set(x, y, calc_pix(x, y))
        return res
