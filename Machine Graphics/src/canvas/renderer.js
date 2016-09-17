import BitmapWrapper from './bitmap-wrapper'

export default function createRenderer (canvas) {
  const ctx = canvas.getContext('2d')
  const imageData = ctx.getImageData(0, 0, canvas.clientWidth, canvas.clientHeight)
  const undoList = []
  let bitmap = new BitmapWrapper(imageData)

  function render (pixels) {
    saveState()
    pixels.forEach(p => bitmap.set(p.x, p.y, p.color))
    ctx.putImageData(imageData, 0, 0)
  }

  function undo () {
    if (!undoList.length) return
    bitmap = undoList.pop()
    imageData.data.set(bitmap.data)
    ctx.putImageData(imageData, 0, 0)
  }

  function saveState () {
    undoList.push(new BitmapWrapper({
      width: imageData.width,
      height: imageData.height,
      data: new Uint8ClampedArray(imageData.data)
    }))
  }

  return { render, undo, canvas }
}
