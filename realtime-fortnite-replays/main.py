import os
import argparse
import asyncio
import helpers
import replay_downloader
from pathlib import Path
from cloudpathlib import CloudPath

parser = argparse.ArgumentParser(description="Tool for parsing Fortnite replays into SingleStore")


def get_all_local_files(directory: Path):
  p = directory.glob('**/*')
  files = [x for x in p if x.is_file()]
  return files

if __name__ == '__main__':
  parser.add_argument("--s2-host", "-H", dest="S2HOST", help="The hostname of the SingleStore cluster", type=str)
  parser.add_argument("--s2-port", "-P", dest="S2PORT", help="The port of the cluster <3306>", default=3306, type=int)
  parser.add_argument("--s2-username", "-U", dest="S2USER", help="The username to use to connect to SingleStore <admin>", default="admin", type=str)
  parser.add_argument("--s2-password", "-p", dest="S2PASS", help="The password for the user to connect to SingleStore", type=str)
  parser.add_argument("--download-replays", "-D", dest="download_replays", help="Download fortnite replay data", action="store_true")
  parser.add_argument("--filesystem-type", "-F", dest="fs_type", help="Select from filesystem (fs), google cloud storage (gcs), Simple Storage System (s3)", choices=['fs','gcs','s3','azure'], default=False)
  parser.add_argument("--replays-location-folder", "-f", dest="replay_location", help="The file path or bucket path of where to store the replays", type=str)
  parser.add_argument("--access-key", "-a", dest="ACCESSKEY", help="The access key or username of the bucket path", type=str, default=None)
  parser.add_argument("--secret-key", "-s", dest="SECRETKEY", help="The secret key or username of the bucket path", type=str, default=None)
  parser.add_argument("--max-ingest-streams", "-m", dest="max_streams", help="The maximum ingest streams into SingleStore", type=int)
  args = parser.parse_args()
  # if the analyzer does not exists, create it
  if args.download_replays:
    pass
  if args.fs_type == 'fs':
    replay_files = get_all_local_files(Path(args.replay_location))
  if args.fs_type == 'gcs':
    pass
  if args.fs_type == 's3':
    pass
  if args.fs_type == 'azure':
    pass
  replay_chunks = helpers.chunks(replay_files, args.max_streams)
  for rc in enumerate(replay_chunks):
    print(f'Starting Replay Chunk: {rc[0]}\n')
    input_coroutines = []
    for r in rc[1]:
      input_coroutines.append(helpers.run_async(f".\\replay-analyzer\\bin\Debug\\net5.0\\FortniteReplayAnalyzer.exe {r} {args.S2HOST} {args.S2PORT} {args.S2USER} {args.S2PASS}"))
    asyncio.get_event_loop().run_until_complete(helpers.run_corroutines(input_coroutines))
    
