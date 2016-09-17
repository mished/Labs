import { describe, it, beforeEach } from 'mocha'
import { expect } from 'chai'
import chai from 'chai'
import sinon from 'sinon'
import sinonChai from 'sinon-chai'
import { memoize } from 'ramda'
import createRenderer from '../../src/canvas/renderer'
import BitmapWrapper from '../../src/canvas/bitmap-wrapper'
import { fromRGBA } from '../../src/utils/color'
chai.use(sinonChai)

describe('renderer', function () {
  let canvas, ctx, renderer
  const blackPixel = fromRGBA(0, 0, 0, 255)
  const whitePixel = fromRGBA(255, 255, 255, 255)

  beforeEach(function () {
    canvas = {
      getContext: memoize(() => {
        const width = 3
        const height = 3
        let imageData = {
          width,
          height,
          data: Uint8ClampedArray.from({ length: width * height * 4 }, () => 255)
        }
        return {
          getImageData () {
            return imageData
          },
          putImageData (updatedImageData) {
            imageData = updatedImageData
          }
        }
      })
    }
    ctx = canvas.getContext('2d')
    renderer = createRenderer(canvas)
  })

  it('Should put Image Data to canvas after rendering', function () {
    const imageData = ctx.getImageData()
    sinon.spy(ctx, 'putImageData')
    renderer.render([])
    expect(ctx.putImageData).to.have.been.calledWith(imageData, 0, 0)
  })

  it('Should render specified pixels', function () {
    const color = blackPixel
    const pixels = [{ x: 0, y: 0 }, { x: 1, y: 1 }, { x: 2, y: 2 }]
      .map(x => Object.assign(x, { color }))
    renderer.render(pixels)
    const updatedImageDataBitmap = new BitmapWrapper(ctx.getImageData())
    const actualColors = pixels.map(x => updatedImageDataBitmap.get(x.x, x.y))
    const expectedColors = Array.from({ length: pixels.length }, () => color)
    expect(actualColors).to.be.eql(expectedColors)
  })

  it('Should undo rendered pixels', function () {
    const newColor = blackPixel
    const initialColor = whitePixel
    const pixels = [{ x: 0, y: 0 }, { x: 1, y: 1 }, { x: 2, y: 2 }]
      .map(x => Object.assign(x, { color: newColor }))
    renderer.render(pixels)
    renderer.undo()
    const updatedImageDataBitmap = new BitmapWrapper(ctx.getImageData())
    const actualColors = pixels.map(x => updatedImageDataBitmap.get(x.x, x.y))
    const expectedColors = Array.from({ length: pixels.length }, () => initialColor)
    expect(actualColors).to.be.eql(expectedColors)
  })
})
