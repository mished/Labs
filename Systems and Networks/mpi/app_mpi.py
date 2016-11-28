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

start_time = MPI.Wtime()

def log(message, op_time=""):
    print("|   {0:6}    |   {1:9.2f}   |   {2:14}   |   {3}"
        .format(rank, MPI.Wtime() - start_time, op_time, message))

def master_log(message, op_time=""):
    if rank == 0:
        log(message, op_time)

def worker_log(message, op_time=""):
    if rank != 0:
        log(message, op_time)

def master():
    return rank == 0

if master():
    print(MPI.Get_library_version())
    print("\n|   Process   |   Timestamp   |   Operation Time   |   Message\n")


if master():
    data_prep_time = MPI.Wtime()
    log("Opening image, preparing data for Scatter.")
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
    master_log("Data ready. Sending chunks info to workers.", MPI.Wtime() - data_prep_time)
else:
    chunk_info = None
    chunks = None

chunk_width, chunk_height, chunk_bytes = comm.bcast(chunk_info, root=0)

worker_chunk = np.empty(chunk_bytes, dtype=np.uint8)
master_log("Scattering pixels data.")
comm.Scatter([chunks, MPI.BYTE], [worker_chunk, MPI.BYTE], root=0)
worker_log("Received pixels data chunk.")

if not master(): # Chunk processing (in workers)
    chunk_start_time = MPI.Wtime()
    log("Starting chunk processing.")
    bm = Bitmap(chunk_width, chunk_height, worker_chunk)
    res_bm = bm.scale(Constants.SCALING).roberts_cross()
    res_chunk = res_bm.get_data()
    res_chunk_width, res_chunk_height = res_bm.get_size()
    res_chunk_bytes = res_chunk_width * res_chunk_height
    res_chunk_info = (res_chunk_width, res_chunk_height, res_chunk_bytes)
    log("Finished chunk processing.", MPI.Wtime() - chunk_start_time)

# Send processed chunk info to master
if rank == 1:
    log("Sending processed chunk info to master.")
    comm.send(res_chunk_info, dest=0, tag=1)
if master():
    res_chunk_width, res_chunk_height, res_chunk_bytes = comm.recv(source=1, tag=1)
    log("Received processed chunks info.")
    res_chunk = np.empty(res_chunk_bytes, dtype=np.uint8)

if master():
    result_buf = np.empty((size, res_chunk_bytes), dtype=np.uint8)
else:
    result_buf = None

master_log("Starting chunks gathering.")
comm.Gather([res_chunk, MPI.BYTE], [result_buf, MPI.BYTE], root=0)
master_log("Gathered processed chunks.")

if master(): # Build processed image from gathered chunks
    log("Building result image from chunks.")
    result_width = res_chunk_width
    result_height = res_chunk_height * (size - 1)
    result_buf = result_buf[1::,:].flatten()
    result_image = Image.new('L', (result_width, result_height))
    result_image.putdata(result_buf)
    result_image.save(Constants.OUTPUT_PATH)
    log("Result image saved.")
