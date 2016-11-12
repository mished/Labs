export default function createRenderer (canvas) {
  const ctx = canvas.getContext('2d')
  const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height)
  const width = imageData.width
  const height = imageData.height
  const data = imageData.data
  const history = [new Uint8ClampedArray(data)]
  let positionInHistory = 0

  const nullColor = 'rgba'.split('').reduce((a, b) => {
    a[b] = null
    return a
  }, {})

  function get (x, y) {
    const i = getPixelPosition(x, y, width)
    return {
      r: data[i],
      g: data[i + 1],
      b: data[i + 2],
      a: data[i + 3]
    }
  }

  function getSafe (x, y) {
    return (x >= 0 && x < width && y >= 0 && y < height)
      ? get(x, y)
      : nullColor
  }

  function set (x, y, { r = 0, g = 0, b = 0, a = 255 }) {
    const i = getPixelPosition(x, y, width)
    data[i] = r
    data[i + 1] = g
    data[i + 2] = b
    data[i + 3] = a
  }

  function render () {
    ctx.putImageData(imageData, 0, 0)
    history.splice(positionInHistory + 1, history.length)
    history.push(new Uint8ClampedArray(data))
    positionInHistory = history.length - 1
  }

  function undo () {
    if (positionInHistory === 0) return
    positionInHistory -= 1
    data.set(history[positionInHistory])
    ctx.putImageData(imageData, 0, 0)
  }

  function redo () {
    if (positionInHistory === history.length - 1) return
    positionInHistory += 1
    data.set(history[positionInHistory])
    ctx.putImageData(imageData, 0, 0)
  }

  function clear () {
    data.set(new Uint8ClampedArray(data.length))
    render()
  }

  return { get, getSafe, set, render, undo, redo, clear, canvas }
}

function getPixelPosition (x, y, width) {
  return x * 4 + y * width * 4
}
