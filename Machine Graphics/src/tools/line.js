import * as R from 'ramda'
import { fromRGBA } from '../utils/color'

export default function createLineTool (renderer, options) {
  const canvas = renderer.canvas
  let isInPreviewState = false
  let color = options.color || fromRGBA(0, 0, 0, 255)
  let renderLine

  canvas.addEventListener('mousedown', onMouseDown)

  function onMouseDown (event) {
    renderLine = R.pipe(
      R.partial(getLinePixels(color, event.offsetX, event.offsetY)),
      renderer.render
    )
    canvas.addEventListener('mousemove', onMouseMove)
    canvas.addEventListener('mouseup', onMouseUp)
  }

  function onMouseMove (event) {
    if (isInPreviewState) renderer.undo()
    isInPreviewState = true
    renderLine(event.offsetX, event.offsetY)
  }

  function onMouseUp (event) {
    if (isInPreviewState) renderer.undo()
    renderLine(event.offsetX, event.offsetY)
    isInPreviewState = false
    canvas.removeEventListener('mousemove', onMouseMove)
    canvas.removeEventListener('mouseup', onMouseUp)
  }

  return () => canvas.removeEventListener('mousedown', onMouseDown)
}

function getLinePixels (color, x1, y1, x2, y2) {
  const k = (y1 - y2) / (x1 - x2)
  const b = y1 - k * x1
  const getPoint = (x) => {
    return {
      x,
      y: k * x + b,
      color
    }
  }
  return R.map(getPoint, R.range(R.juxt([Math.min, Math.max])(x1, x2)))
}
