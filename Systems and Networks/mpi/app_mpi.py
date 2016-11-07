#!/usr/bin/env python
#
# Run with: mpirun -np 6 ./app_mpi.py

import numpy as np
from mpi4py import MPI
from PIL import Image

from bitmap import Bitmap
from constants import Constants

comm = MPI.COMM_WORLD
size = comm.size
rank = comm.rank

if rank == 0:
    image = Image.open(Constants.INPUT_PATH)
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

if rank != 0: # Chunk processing (in workers)
    bm = Bitmap(chunk_width, chunk_height, worker_chunk)
    res_bm = bm.scale(Constants.SCALING).roberts_cross()
    res_chunk = res_bm.get_data()
    res_chunk_width, res_chunk_height = res_bm.get_size()
    res_chunk_bytes = res_chunk_width * res_chunk_height
    res_chunk_info = (res_chunk_width, res_chunk_height, res_chunk_bytes)

# Send processed chunk info to master
if rank == 1:
    comm.send(res_chunk_info, dest=0, tag=1)
if rank == 0:
    res_chunk_width, res_chunk_height, res_chunk_bytes = comm.recv(source=1, tag=1)
    res_chunk = np.empty(res_chunk_bytes, dtype=np.uint8)

if rank == 0:
    result_buf = np.empty((size, res_chunk_bytes), dtype=np.uint8)
else:
    result_buf = None

comm.Gather([res_chunk, MPI.BYTE], [result_buf, MPI.BYTE], root=0)

if rank == 0: # Build processed image from gathered chunks
    result_width = res_chunk_width
    result_height = res_chunk_height * (size - 1)
    result_buf = result_buf[1::,:].flatten()
    result_image = Image.new('L', (result_width, result_height))
    result_image.putdata(result_buf)
    result_image.save(Constants.OUTPUT_PATH)
