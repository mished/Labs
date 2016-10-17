import { bezierCurvePoints, drawBezierCurve } from './bezier-curve'

export default function drawBezierSurface (options, ...points) {
  const { rows: r, columns: c } = options
  const steps = 20
  const t = 1 / steps

  if (points.length !== r * c) throw new Error()

  for (const controlPoints of [getRows(r, c, points), getColumns(r, c, points)]) {
    const curves = controlPoints.map(r => bezierCurvePoints(t, r))
    for (let i = 0; i <= steps; ++i) {
      drawBezierCurve(options, ...curves.map(c => c.next().value))
    }
  }
}

function getColumns (r, c, points) {
  const res = []
  for (let x = 0; x < c; ++x) {
    let col = []
    for (let y = x; y < points.length; y += c) {
      col.push(points[y])
    }
    res.push(col)
  }
  return res
}

function getRows (r, c, points) {
  const res = []
  for (let i = 0; i < points.length; i += c) {
    res.push(points.slice(i, i + c))
  }
  return res
}
