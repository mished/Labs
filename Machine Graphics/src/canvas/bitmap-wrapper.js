export default class BitmapWrapper {
  constructor ({ width, height, data }) {
    this.width = width
    this.height = height
    this.data = data
  }

  get (x, y) {
    const i = getPixelPosition(x, y, this.width)
    return {
      r: this.data[i],
      g: this.data[i + 1],
      b: this.data[i + 2],
      a: this.data[i + 3]
    }
  }

  set (x, y, { r = 0, g = 0, b = 0, a = 255 }) {
    const i = getPixelPosition(x, y, this.width)
    this.data[i] = r
    this.data[i + 1] = g
    this.data[i + 2] = b
    this.data[i + 3] = a
  }
}

function getPixelPosition (x, y, width) {
  return x * 4 + y * width * 4
}
