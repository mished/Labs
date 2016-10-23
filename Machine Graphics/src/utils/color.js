export function fromRGBA (r, g, b, a) {
  return { r, g, b, a }
}

export function invertRGBA (color) {
  return Object.keys(color).reduce((p, c) => {
    p[c] = (c !== 'a') ? 255 - color[c] : color[c]
    return p
  }, {})
}

export function randomRGBA () {
  return ['r', 'g', 'b', 'a'].reduce((p, c) => {
    p[c] = Math.random() * 255
    return p
  }, {})
}

export function equals (c1, c2) {
  return Object.keys(c1).every(x => c1[x] === c2[x])
}

