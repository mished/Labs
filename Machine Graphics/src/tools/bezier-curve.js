export function drawBezierCurve (options, ...points) {
  const bitmap = options.renderer
  const color = options.color

  const t = 0.0001
  for (let {x, y} of bezierCurvePoints(t, points)) {
    bitmap.set(x, y, color)
  }
}

export function * bezierCurvePoints (t, points) {
  if (points.length < 2) throw new Error()

  for (let i = 0; i <= 1; i = Math.round((i + t) * 10000) / 10000) {
    yield getPoint(i, points)
  }

  function getPoint (t, points) {
    if (points.length === 2) {
      const p = getControlPointPosition(t, ...points)
      return { x: Math.round(p.x), y: Math.round(p.y) }
    }
    return getPoint(t, reducePoints(t, points))
  }

  function reducePoints (t, points) {
    const res = []
    for (let i = 1; i < points.length; ++i) {
      res.push(getControlPointPosition(t, points[i - 1], points[i]))
    }
    return res
  }

  function getControlPointPosition (t, p1, p2) {
    const k = (p1.y - p2.y) / (p1.x - p2.x)
    const b = p1.y - k * p1.x
    const [xP, yP] = [Math.abs(p1.x - p2.x), Math.abs(p1.y - p2.y)]

    if (xP > yP) {
      const x = p1.x + (p2.x - p1.x) * t
      const y = (p1.y === p2.y) ? p1.y : k * x + b
      return { x, y }
    } else {
      const y = p1.y + (p2.y - p1.y) * t
      const x = (p1.x === p2.x) ? p1.x : (y - b) / k
      return { x, y }
    }
  }
}
