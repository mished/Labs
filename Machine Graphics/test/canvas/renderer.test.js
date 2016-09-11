import { describe, it, beforeEach } from 'mocha'
import { expect } from 'chai'
import { memoize } from 'ramda'
import createRenderer from '../../src/canvas/renderer'
import BitmapWrapper from '../../src/canvas/bitmap-wrapper'

describe('renderer', function () {
  let canvas, ctx, renderer
  const blackPixel = { r: 0, g: 0, b: 0, a: 255 }

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

  it('Should render specified pixels', function () {
    const color = blackPixel
    const pixels = [{ x: 0, y: 0 }, { x: 1, y: 1 }, { x: 2, y: 2 }]
      .map(x => Object.assign(x, { color }))
    renderer.renderPixels(pixels)
    const updatedImageDataBitmap = new BitmapWrapper(ctx.getImageData())
    const actualColors = pixels.map(x => updatedImageDataBitmap.get(x.x, x.y))
    const expectedColors = Array.from({ length: pixels.length }, () => color)
    expect(actualColors).to.be.eql(expectedColors)
  })
})
