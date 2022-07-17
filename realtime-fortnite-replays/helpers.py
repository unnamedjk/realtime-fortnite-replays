import os, re, os.path
import asyncio
import warnings
import argparse
warnings.filterwarnings("ignore", category=DeprecationWarning) 

async def run_async(cmd):
    proc = await asyncio.create_subprocess_shell(
        cmd,
        stdout=asyncio.subprocess.PIPE,
        stderr=asyncio.subprocess.PIPE)

    stdout, stderr = await proc.communicate()

    print(f'[{cmd!r} exited with {proc.returncode}]')
    if stdout:
        print(f'[stdout]\n{stdout.decode()}')
    if stderr:
        print(f'[stderr]\n{stderr.decode()}')

async def run_corroutines(input_coroutines):
  result = await asyncio.gather(*input_coroutines, return_exceptions=True)
  return result

def chunks(lst, n):
  chunks = []
  for i in range(0, len(lst), n):
    chunks.append(lst[i:i + n])
  return chunks    
    
