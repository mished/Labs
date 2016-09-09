import { describe, it } from 'mocha'
import { expect } from 'chai'
import Bitmap from '../../src/bitmap/bitmap'

describe('Bitmap', function () {
  it('Should return a string', function () {
    expect(Bitmap()).to.be.a('string')
  })
})
