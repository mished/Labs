export function fromRGBA (r, g, b, a) {
  return { r, g, b, a }
}

export function invertRGBA (color) {
  return Object.keys(color).reduce((p, c) => {
    p[c] = (c !== 'a') ? 255 - color[c] : color[c]
    return p
  }, {})
}
