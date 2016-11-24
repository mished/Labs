import { point } from '../utils/point'

const { atan2, cos, sin, round } = Math

export default function transform2d (options, ...points) {
  const bitmap = options.renderer
  const transform = options.transform
  const { width, height } = bitmap
  const { x: x1, y: y1 } = points[0]
  const { x: x2, y: y2 } = points[1]
  const origin = point(width / 2, height / 2)

  const transforms = {
    translate: (function () {
      const [dx, dy] = [x2 - x1, y2 - y1]

      return (x, y) => point(x + dx, y + dy)
    }()),
    rotate: (function () {
      const ox = origin.x
      const oy = origin.y
      const a = atan2(y2 - oy, x2 - ox) - atan2(y1 - oy, x1 - ox)
      const sina = sin(a)
      const cosa = cos(a)

      return (x, y) => {
        const rx = ox + (x - ox) * cosa - (y - oy) * sina
        const ry = oy + (x - ox) * sina + (y - oy) * cosa
        return point(round(rx), round(ry))
      }
    }())
  }

  const shape = Array.from({length: 250}, (p, i) => point(i + origin.x, i + origin.y))
  shape.forEach(p => bitmap.set(p.x, p.y, options.color))

  const transformFunc = transforms[transform]
  shape.forEach(p => {
    const { x, y } = transformFunc(p.x, p.y)
    bitmap.translate(p.x, p.y, x, y)
  })
}
