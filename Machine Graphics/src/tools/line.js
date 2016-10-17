export default function drawLine (options, { x: x1, y: y1 }, { x: x2, y: y2 }) {
  const bitmap = options.renderer
  const color = options.color

  const k = (y1 - y2) / (x1 - x2)
  const b = y1 - k * x1
  const [maxX, maxY] = [Math.max(x1, x2), Math.max(y1, y2)]
  const [minX, minY] = [Math.min(x1, x2), Math.min(y1, y2)]

  let min, max
  let setPixelFunc

  if (maxX - minX > maxY - minY) {
    [min, max] = [minX, maxX]
    if (y1 === y2) {
      setPixelFunc = x => bitmap.set(x, y1, color)
    } else {
      setPixelFunc = x => bitmap.set(x, Math.round(k * x + b), color)
    }
  } else {
    [min, max] = [minY, maxY]
    if (x1 === x2) {
      setPixelFunc = y => bitmap.set(x1, y, color)
    } else {
      setPixelFunc = y => bitmap.set(Math.round((y - b) / k), y, color)
    }
  }

  for (let i = min; i <= max; ++i) {
    setPixelFunc(i)
  }
}
