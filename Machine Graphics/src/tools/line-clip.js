import drawLine from './line'
import { point } from '../utils/point'

export default function lineClip (options, ...points) {
  const winWidth = 500
  const winHeight = 300

  const wxLeft = points[0].x - winWidth / 2
  const wxRight = points[0].x + winWidth / 2
  const wyBot = points[0].y + winHeight / 2
  const wyTop = points[0].y - winHeight / 2

  drawWindow(point(wxLeft, wyTop), point(wxRight, wyBot))
  if (points.length < 3) return

  let { x: x0, y: y0 } = points[1]
  let { x: x1, y: y1 } = points[2]

  let [t0, t1] = [0, 1]
  let dx, dy
  dx = x1 - x0
  if (check(-dx, x0 - wxLeft)) {
    if (check(dx, wxRight - x0)) {
      dy = y1 - y0
      if (check(dy, wyBot - y0)) {
        if (check(-dy, y0 - wyTop)) {
          if (t1 < 1) {
            x1 = x0 + t1 * dx
            y1 = y0 + t1 * dy
          }
          if (t0 > 0) {
            x0 = x0 + t0 * dx
            y0 = y0 + t0 * dy
          }
          drawLine(options, point(x0, y0), point(x1, y1))
        }
      }
    }
  }

  function check (p, q) {
    let r
    if (p === 0) {
      if (q < 0) return false
    } else {
      r = q / p
      if (p < 0) {
        if (r > t1) return false
        else if (r > t0) t0 = r
      } else {
        if (r < t0) return false
        else if (r < t1) t1 = r
      }
    }
    return true
  }

  function drawWindow (topLeft, botRight) {
    const line = drawLine.bind(null, options)
    line(topLeft, point(botRight.x, topLeft.y))
    line(topLeft, point(topLeft.x, botRight.y))
    line(point(topLeft.x, botRight.y), botRight)
    line(point(botRight.x, topLeft.y), botRight)
  }
}
