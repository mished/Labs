from PIL import Image

from bitmap import Bitmap
from constants import Constants

image = Image.open(Constants.INPUT_PATH)
image_data = image.getdata()
width, height = image.size

bm = Bitmap(width, height, image_data)

res_bm = bm.scale(Constants.SCALING).roberts_cross()
res_data = res_bm.get_data()
res_width, res_height = res_bm.get_size()

result_image = Image.new('L', (res_width, res_height))
result_image.putdata(res_data)
result_image.save(Constants.OUTPUT_PATH)
