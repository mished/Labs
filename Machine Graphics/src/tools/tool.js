import { fromRGBA } from '../utils/color'
import { pipe } from 'ramda'
import drawLine from './line'
import drawCircle from './circle'
import drawEllipse from './ellipse'

const drawFunctions = {
  line: drawLine,
  circle: drawCircle,
  ellipse: drawEllipse
}

export default function createTool (renderer, options) {
  const canvas = renderer.canvas
  let drawFunc
  if (!drawFunctions[options.shape]) {
    throw new Error(`Available shapes: ${Object.keys(drawFunctions).join(', ')}`)
  }
  let isInPreviewState = false
  let color = options.color || fromRGBA(0, 0, 0, 255)

  function onMouseDown (event) {
    drawFunc = pipe(
      drawFunctions[options.shape].bind(null, renderer, color, event.offsetX, event.offsetY),
      renderer.render
    )
    canvas.addEventListener('mousemove', onMouseMove)
    canvas.addEventListener('mouseup', onMouseUp)
  }

  function onMouseMove (event) {
    if (isInPreviewState) renderer.undo()
    isInPreviewState = true
    drawFunc(event.offsetX, event.offsetY)
  }

  function onMouseUp (event) {
    if (isInPreviewState) renderer.undo()
    drawFunc(event.offsetX, event.offsetY)
    isInPreviewState = false
    canvas.removeEventListener('mousemove', onMouseMove)
    canvas.removeEventListener('mouseup', onMouseUp)
  }

  return {
    color,
    on: () => canvas.addEventListener('mousedown', onMouseDown),
    off: () => canvas.removeEventListener('mousedown', onMouseDown)
  }
}


