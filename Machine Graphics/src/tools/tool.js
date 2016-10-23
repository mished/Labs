import { point } from '../utils/point'
import { pipe } from 'ramda'
import drawLine from './line'
import drawCircle from './circle'
import drawEllipse from './ellipse'
import { drawBezierCurve } from './bezier-curve'
import drawBezierSurface from './bezier-surface'
import drawSmoothPolyline from './smooth-polyline'
import fill from './fill'

const drawFunctions = {
  'line': drawLine,
  'circle': drawCircle,
  'ellipse': drawEllipse,
  'bezier-curve': drawBezierCurve,
  'bezier-surface': drawBezierSurface,
  'smooth-polyline': drawSmoothPolyline,
  'fill': fill
}

const drawStrategies = {
  'single-click': singleClickStrategy,
  'multi-click': multiClickStrategy
}

export default function createTool (options) {
  if (!drawFunctions[options.shape]) {
    throw new Error(`Available shapes: ${Object.keys(drawFunctions).join(', ')}`)
  }

  const drawStrategy = drawStrategies[options.drawStrategy](options)

  return {
    on: drawStrategy.on,
    off: drawStrategy.off
  }
}

function singleClickStrategy (options) {
  const renderer = options.renderer
  const canvas = renderer.canvas
  let drawFunc
  let isInPreviewState = false

  function onMouseDown (event) {
    drawFunc = pipe(
      drawFunctions[options.shape].bind(null, options, point(event.offsetX, event.offsetY)),
      renderer.render
    )
    if (options.preview === false) return drawFunc()
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

function multiClickStrategy (options) {
  const renderer = options.renderer
  const canvas = renderer.canvas
  let drawFunc
  let step = 0
  let isInPreviewState = false

  function onMouseMove (event) {
    if (isInPreviewState) renderer.undo()
    isInPreviewState = true
    drawFunc(point(event.offsetX, event.offsetY))
    renderer.render()
  }

  function onMouseUp (event) {
    step += 1
    if (step === 1) {
      drawFunc = drawFunctions[options.shape].bind(null, options, point(event.offsetX, event.offsetY))
      if (options.preview) {
        canvas.addEventListener('mousemove', onMouseMove)
      }
    } if (step === options.steps) {
      if (isInPreviewState) renderer.undo()
      drawFunc(point(event.offsetX, event.offsetY))
      renderer.render()
      dispose()
    } else if (step > 1) {
      drawFunc = drawFunc.bind(null, point(event.offsetX, event.offsetY))
    }
  }

  function dispose () {
    isInPreviewState = false
    step = 0
    canvas.removeEventListener('mousemove', onMouseMove)
  }

  return {
    on: (newOpts) => {
      if (newOpts) Object.assign(options, newOpts)
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
