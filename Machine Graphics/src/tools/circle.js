export default function drawCircle (bitmap, color, { x: x1, y: y1 }, { x: x2, y: y2 }) {
  const r = Math.max(Math.abs(x1 - x2), Math.abs(y1 - y2))
  let [x, y] = [0, r]
  let p = 1 - r
  setSymmetricPoints(x, y)

  while (x < y) {
    x += 1
    if (p < 0) {
      p += 2 * x + 1
    } else {
      y -= 1
      p += 2 * (x - y) + 1
    }
    setSymmetricPoints(x, y)
  }

  function setSymmetricPoints (x, y) {
    [ { x: x1 + x, y: y1 + y },
      { x: x1 - x, y: y1 + y },
      { x: x1 + x, y: y1 - y },
      { x: x1 - x, y: y1 - y },
      { x: x1 + y, y: y1 + x },
      { x: x1 - y, y: y1 + x },
      { x: x1 + y, y: y1 - x },
      { x: x1 - y, y: y1 - x }
    ].forEach(p => bitmap.set(p.x, p.y, color))
  }
}
