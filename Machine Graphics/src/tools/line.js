export default function drawLine (bitmap, color, x1, y1, x2, y2) {
  const k = (y1 - y2) / (x1 - x2)
  const b = y1 - k * x1
  const [maxX, maxY] = [Math.max(x1, x2), Math.max(y1, y2)]
  const [minX, minY] = [Math.min(x1, x2), Math.min(y1, y2)]

  if (maxX - minX > maxY - minY) {
    for (let x = minX; x <= maxX; ++x) {
      bitmap.set(x, Math.round(k * x + b), color)
    }
  } else {
    for (let y = minY; y <= maxY; ++y) {
      bitmap.set(Math.round((y - b) / k), y, color)
    }
  }
}
