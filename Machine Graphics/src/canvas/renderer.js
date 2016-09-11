import BitmapWrapper from './bitmap-wrapper'

export default function createRenderer (canvas) {
  const ctx = canvas.getContext('2d')
  const imageData = ctx.getImageData(0, 0, canvas.clientWidth, canvas.clientHeight)
  const bitmap = new BitmapWrapper(imageData)

  return {
    renderPixels (pixels) {
      pixels.forEach(p => bitmap.set(p.x, p.y, p.color))
      ctx.putImageData(imageData, 0, 0)
    }
  }
}
