import { drawBezierCurve } from './bezier-curve'
import drawLine from './line.js'
import { invertRGBA } from '../utils/color'

export default function drawSmoothPolyline (options, ...points) {
  if (points.length < 3) {
    return drawLine(options, points[0], points[1])
  }
  const a = 1.5
  const bezierOptions = Object.assign({}, options, { color: invertRGBA(options.color) })

  let cur, next
  for (let i = 0; i <= points.length - 2; ++i) {
    if (i === 0) {
      cur = getDerivative(points[0], points[1])
      next = getDerivative(points[0], points[2])
    } else if (i === points.length - 2) {
      cur = next
      next = getDerivative(points[i], points[i + 1])
    } else {
      cur = next
      next = getDerivative(points[i], points[i + 2])
    }
    const [c1, c2] = getControlPoints(points[i], points[i + 1], cur, next)
    drawSegment(points[i], points[i + 1], c1, c2)
  }

  function drawSegment (p1, p2, c1, c2) {
    drawLine(options, p1, p2)
    drawBezierCurve(bezierOptions, p1, c1, c2, p2)
  }

  function getDerivative (p1, p2) {
    const [x, y] = [(p2.x - p1.x) / a, (p2.y - p1.y) / a]
    return { x, y }
  }

  function getControlPoints (p1, p2, pd1, pd2) {
    const [c1x, c1y] = [p1.x + pd1.x / 3, p1.y + pd1.y / 3]
    const [c2x, c2y] = [p2.x - pd2.x / 3, p2.y - pd2.y / 3]
    return [
      { x: c1x, y: c1y },
      { x: c2x, y: c2y }
    ]
  }
}
