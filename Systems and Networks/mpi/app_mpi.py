#!/usr/bin/env python
#
# Run with: mpirun -np 6 ./app_mpi.py

import numpy as np
from mpi4py import MPI
from PIL import Image

input_path = './image5k.bmp'
output_path = './image_out.bmp'

comm = MPI.COMM_WORLD
size = comm.size
rank = comm.rank

if comm.rank == 0:
    image = Image.open(input_path)
    image_data = list(image.getdata())
    width, height = image.size
    chunk_width = width
    chunk_height = height / (size - 1)
    chunk_bytes = chunk_height * chunk_width
    chunk_info = (chunk_width, chunk_height, chunk_bytes)

    chunks = np.empty((size, chunk_bytes), dtype=np.uint8)
    for i in range(1, size):
        offset = (i - 1) * chunk_bytes
        chunks[i,:] = image_data[offset : offset + chunk_bytes]
else:
    chunk_info = None
    chunks = None

chunk_width, chunk_height, chunk_bytes = comm.bcast(chunk_info, root=0)

worker_chunk = np.empty(chunk_bytes, dtype=np.uint8)
comm.Scatter([chunks, MPI.BYTE], [worker_chunk, MPI.BYTE], root=0)

# Fake chunk processing (invert colors)
worker_chunk = 255 - worker_chunk

if comm.rank == 0:
    result_buf = np.empty((size, chunk_bytes), dtype=np.uint8)
else:
    result_buf = None

comm.Gather([worker_chunk, MPI.BYTE], [result_buf, MPI.BYTE], root=0)

if comm.rank == 0: # Build processed image from gathered chunks
    result_buf = result_buf[1::,:].flatten()
    result_image = Image.new('L', (width, height))
    result_image.putdata(result_buf)
    result_image.save(output_path)
