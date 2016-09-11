import { describe, it, beforeEach, before } from 'mocha'
import { expect } from 'chai'
import BitmapWrapper from '../../src/canvas/bitmap-wrapper'

describe('BitmapWrapper', function () {
  let imageData, bitmap, whitePixel, blackPixel

  before(function () {
    whitePixel = { r: 255, g: 255, b: 255, a: 255 }
    blackPixel = { r: 0, g: 0, b: 0, a: 255 }
  })

  beforeEach(function () {
    const width = 3
    const height = 3
    imageData = {
      width,
      height,
      data: Uint8ClampedArray.from({ length: width * height * 4 }, () => 255)
    }
    imageData.data[4] = 0
    imageData.data[5] = 0
    imageData.data[6] = 0
    imageData.data[7] = 255
    bitmap = new BitmapWrapper(imageData)
  })

  it('Should create object from imageData', function () {
    expect(bitmap).to.have.all.keys({
      width: imageData.width,
      height: imageData.height,
      data: imageData.data
    })
  })

  it('get() should return pixel [0, 0]', function () {
    expect(bitmap.get(0, 0)).to.be.eql(whitePixel)
  })

  it('get() should return pixel [3, 3]', function () {
    expect(bitmap.get(2, 2)).to.be.eql(whitePixel)
  })

  it('get() should return pixel [1, 0]', function () {
    expect(bitmap.get(1, 0)).to.be.eql(blackPixel)
  })

  it('set() should update imageData', function () {
    bitmap.set(0, 0, blackPixel)
    expect({
      r: imageData.data[0],
      g: imageData.data[1],
      b: imageData.data[2],
      a: imageData.data[3]
    }).to.be.eql(blackPixel)
  })
})
