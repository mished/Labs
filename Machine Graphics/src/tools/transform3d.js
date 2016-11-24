import { point3d } from '../utils/point'
import { Matrix as M } from '../utils/matrix'

const { cos, sin, round } = Math
const toRad = Math.PI / 180

export default function transform3d (options) {
  const bitmap = options.renderer
  const { width, height } = bitmap
  const origin = point3d(0, 0, 0)
  const lineLength = 150

  const shape = [].concat(
    Array.from({length: lineLength}, (p, i) => point3d(i + origin.x, origin.y, origin.z)),
    Array.from({length: lineLength}, (p, i) => point3d(origin.x, origin.y, i + origin.z)),
    Array.from({length: lineLength}, (p, i) => point3d(i + origin.x, origin.y, lineLength + origin.z)),
    Array.from({length: lineLength}, (p, i) => point3d(lineLength + origin.x, origin.y, i + origin.z))
  )

  transform(1, 10)

  function transform (rotate, translate) {
    window.requestAnimationFrame(() => {
      bitmap.undo()
      shape.forEach(p => {
        const transformed = transformPoint(p, rotate, translate)
        const { x, y } = projectPoint(transformed)
        bitmap.set(x, y, options.color)
      })
      bitmap.render()
      transform(rotate < 359 ? rotate + 1 : 1, translate + 10)
    })
  }

  const rotationX = (a) => new M([
    [1, 0, 0, 0],
    [0, cos(a), sin(a), 0],
    [0, -sin(a), cos(a), 0],
    [0, 0, 0, 1]
  ])

  const rotationY = (a) => new M([
    [cos(a), 0, -sin(a), 0],
    [0, 1, 0, 0],
    [sin(a), 0, cos(a), 0],
    [0, 0, 0, 1]
  ])

  const rotationZ = (a) => new M([
    [cos(a), sin(a), 0, 0],
    [-sin(a), cos(a), 0, 0],
    [0, 0, 1, 0],
    [0, 0, 0, 1]
  ])

  const translation = (tx, ty, tz) => new M([
    [1, 0, 0, tx],
    [0, 1, 0, ty],
    [0, 0, 1, tz],
    [0, 0, 0, 1]
  ])

  function transformPoint ({x, y, z}, rotate, translate) {
    const vec = new M(x, y, z, 1)
    const res = translation(translate, translate, translate)
      .multiply(rotationX(rotate * toRad))
      .multiply(rotationY(rotate * toRad))
      .multiply(rotationZ(rotate * toRad))
      // .multiply(translation(-75, -75, -75))
      .multiply(vec).matrix

    return {
      x: res[0][0],
      y: res[1][0],
      z: res[2][0]
    }
  }
}

function projectPoint (p) {
  const a = 45 * toRad
  return {
    x: round(p.x - p.z * cos(a)),
    y: round(p.y - p.z * sin(a))
  }
}

    // const [l, m, n] = [1, 1, 0]
    // const rotationMatrix = new M([
    //   [l * l * (1 - cosa) + cosa, m * l * (1 - cosa) - n * sina, n * l * (1 - cosa) + m * sina],
    //   [l * m * (1 - cosa) + n * sina, m * m * (1 - cosa) + cosa, n * m * (1 - cosa) - l * sina],
    //   [l * n * (1 - cosa) - m * sina, m * n * (1 - cosa) + l * sina, n * n * (1 - cosa) + cosa]
    // ])
