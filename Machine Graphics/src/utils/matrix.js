export class Matrix {
  constructor (data, ...args) {
    if (Array.isArray(data)) {
      this.matrix = data
    } else {
      this.matrix = [].concat([data], args).map(x => [x])
    }
  }

  multiply (other) {
    const left = this.matrix
    const right = other.matrix
    const len = left[0].length
    const res = Matrix.getMatrix(left.length, right[0].length)
    for (let i = 0; i < len; ++i) {
      for (let j = 0; j < right[0].length; ++j) {
        for (let k = 0; k < len; ++k) {
          res[i][j] += left[i][k] * right[k][j]
        }
      }
    }
    return new Matrix(res)
  }

  static getMatrix (i, j) {
    return Array.from({ length: i }, () => Array.from({ length: j }, () => 0))
  }
}
