export default function drawEllipse (options, { x: x1, y: y1 }, { x: x2, y: y2 }) {
  const bitmap = options.renderer
  const color = options.color

  const [Rx, Ry] = [Math.abs(x1 - x2), Math.abs(y1 - y2)]
  const [Rx2, Ry2] = [Rx * Rx, Ry * Ry]
  const [twoRx2, twoRy2] = [2 * Rx2, 2 * Ry2]

  let p
  let [x, y] = [0, Ry]
  let [px, py] = [0, twoRx2 * y]

  setSymmetricPoints(x, y)

  p = Math.round(Ry2 - (Rx2 * Ry) + (0.25 * Rx2))
  while (px < py) {
    x += 1
    px += twoRy2
    if (p < 0) {
      p += Ry2 + px
    } else {
      y -= 1
      py -= twoRx2
      p += Ry2 + px - py
    }
    setSymmetricPoints(x, y)
  }

  p = Math.round(Ry2 * (x + 0.5) * (x + 0.5) + Rx2 * (y - 1) * (y - 1) - Rx2 * Ry2)
  while (y > 0) {
    y -= 1
    py -= twoRx2
    if (p > 0) {
      p += Rx2 - py
    } else {
      x += 1
      px += twoRy2
      p += Rx2 - py + px
    }
    setSymmetricPoints(x, y)
  }

  function setSymmetricPoints (x, y) {
    [ { x: x1 + x, y: y1 + y },
      { x: x1 - x, y: y1 + y },
      { x: x1 + x, y: y1 - y },
      { x: x1 - x, y: y1 - y }
    ].forEach(p => bitmap.set(p.x, p.y, color))
  }
}
