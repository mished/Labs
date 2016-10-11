import { fromRGBA } from '../utils/color'
import { point } from '../utils/point'
import { pipe } from 'ramda'
import drawLine from './line'
import drawCircle from './circle'
import drawEllipse from './ellipse'
import drawBezierCurve from './bezier-curve'

const drawFunctions = {
  'line': drawLine,
  'circle': drawCircle,
  'ellipse': drawEllipse,
  'bezier-curve': drawBezierCurve
}

const drawStrategies = {
  'single-click': singleClickStrategy,
  'multi-click': multiClickStrategy
}

export default function createTool (renderer, options) {
  if (!drawFunctions[options.shape]) {
    throw new Error(`Available shapes: ${Object.keys(drawFunctions).join(', ')}`)
  }

  const drawStrategy = drawStrategies[options.drawStrategy](renderer, options)

  return {
    on: drawStrategy.on,
    off: drawStrategy.off
  }
}

function singleClickStrategy (renderer, options) {
  let drawFunc
  const canvas = renderer.canvas
  let isInPreviewState = false
  let color = options.color || fromRGBA(0, 0, 0, 255)

  function onMouseDown (event) {
    drawFunc = pipe(
      drawFunctions[options.shape].bind(null, renderer, color, point(event.offsetX, event.offsetY)),
      renderer.render
    )
    canvas.addEventListener('mousemove', onMouseMove)
    canvas.addEventListener('mouseup', onMouseUp)
  }

  function onMouseMove (event) {
    if (isInPreviewState) renderer.undo()
    isInPreviewState = true
    drawFunc(point(event.offsetX, event.offsetY))
  }

  function onMouseUp (event) {
    if (isInPreviewState) renderer.undo()
    drawFunc(point(event.offsetX, event.offsetY))
    isInPreviewState = false
    canvas.removeEventListener('mousemove', onMouseMove)
    canvas.removeEventListener('mouseup', onMouseUp)
  }

  return {
    on: () => canvas.addEventListener('mousedown', onMouseDown),
    off: () => canvas.removeEventListener('mousedown', onMouseDown)
  }
}

function multiClickStrategy (renderer, options) {
  let drawFunc
  let step = 0
  const canvas = renderer.canvas
  let isInPreviewState = false
  let color = options.color || fromRGBA(0, 0, 0, 255)

  function onMouseMove (event) {
    if (isInPreviewState) renderer.undo()
    isInPreviewState = true
    drawFunc(point(event.offsetX, event.offsetY))
    renderer.render()
  }

  function onMouseUp (event) {
    step += 1
    if (step === 1) {
      drawFunc = drawFunctions[options.shape].bind(null, renderer, color, point(event.offsetX, event.offsetY))
      canvas.addEventListener('mousemove', onMouseMove)
    } if (step === options.steps) {
      if (isInPreviewState) renderer.undo()
      drawFunc(point(event.offsetX, event.offsetY))
      renderer.render()
      dispose()
    } else {
      drawFunc = drawFunc.bind(null, point(event.offsetX, event.offsetY))
    }
  }

  function dispose () {
    isInPreviewState = false
    step = 0
    canvas.removeEventListener('mousemove', onMouseMove)
  }

  return {
    on: (order) => {
      if (order) options.steps = order
      canvas.addEventListener('mouseup', onMouseUp)
    },
    off: () => {
      canvas.removeEventListener('mouseup', onMouseUp)
      if (step !== 0) {
        renderer.undo()
        dispose()
      }
    }
  }
}
