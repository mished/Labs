import { equals, randomRGBA } from '../utils/color'
import { point } from '../utils/point'
import drawLine from './line'

export default function fill (options, p) {
  const bitmap = options.renderer
  options.color = randomRGBA()
  const seedColor = bitmap.get(p.x, p.y)
  if (equals(seedColor, options.color)) return

  let maxDist = 0
  const fillPixels = []
  const queue = []

  const line = (!options.gradient)
    ? drawLine
    : (options, p1, p2) => {
      for (let i = p1.x; i <= p2.x; ++i) {
        fillPixels.push(point(i, p1.y))
        bitmap.set(i, p1.y, options.color)
      }
    }

  queue.push(p)
  while (queue.length) {
    const seed = queue.shift()
    if (options.gradient) maxDist = Math.max(maxDist, getDistance(seed.x, seed.y))
    const {xl, xr} = getLineBoundaries(seed.x, seed.y)
    line(options, point(xl, seed.y), point(xr, seed.y))
    findSeedPixels(xl, xr, seed.y - 1)
    findSeedPixels(xl, xr, seed.y + 1)
  }

  if (options.gradient) {
    const s = scale(maxDist)
    fillPixels.forEach(p => bitmap.set(p.x, p.y, s(getDistance(p.x, p.y))))
  }

  function getLineBoundaries (x, y) {
    let xl = x
    let xr = x
    while (equals(bitmap.get(xl, y), seedColor)) xl -= 1
    while (equals(bitmap.get(xr, y), seedColor)) xr += 1
    xl += 1
    xr -= 1
    return { xl, xr }
  }

  function findSeedPixels (xl, xr, y) {
    while (xl < xr) {
      const initial = xl
      while (equals(bitmap.get(xl, y), seedColor)) xl += 1
      if (xl !== initial) queue.push(point(xl - 1, y))
      xl += 1
    }
  }

  function getDistance (x, y) {
    return Math.hypot(Math.abs(x - p.x), Math.abs(y - p.y))
  }

  function scale (maxDist) {
    const color = randomRGBA()
    const colorCoefs = Object.keys(options.color).reduce((p, c) => {
      p[c] = options.color[c] - color[c]
      return p
    }, {})
    return x => Object.keys(options.color).reduce((p, c) => {
      p[c] = (c === 'a') ? options.color[c] : (colorCoefs[c] * x / maxDist + color[c])
      return p
    }, {})
  }
}
