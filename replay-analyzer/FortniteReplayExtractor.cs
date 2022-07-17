using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Unreal.Core.Models.Enums;
using System.Collections.Generic;
using FortniteReplayAnalyzer.Extensions;
using Nanoid;
using System.IO;
using System.Data;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Analyzer
{
  class FortniteReplayExtractor
  {
    
    static public string replaysPath;
    static void Main(string[] args)
    {
      int coreCount = Environment.ProcessorCount;

      //Console.WriteLine($"\nAnalyzing Replay: {replayFile}");

      Stopwatch sw = new Stopwatch();
      sw.Start();

      // Setup Service Collection & Logging
      var serviceCollection = new ServiceCollection()
        .AddLogging(loggingBuilder => loggingBuilder
          .AddConsole()
          .SetMinimumLevel(LogLevel.Error));

      var provider  = serviceCollection.BuildServiceProvider();
      var logger    = provider.GetService<ILogger<FortniteReplayExtractor>>();
      var reader = new ReplayReader(logger, ParseMode.Full);
      replaysPath = args[0];
      DirectoryInfo replaysDir = new DirectoryInfo(replaysPath);
      List<FileInfo> replayFiles = new List<FileInfo>(replaysDir.GetFiles());
      List<List<FileInfo>> replayChunks = replayFiles.partition(50);

      int currentChunk = 0;
      foreach (List<FileInfo> replayChunk in replayChunks)
      {
        List<Task<FortniteReplayReader.Models.FortniteReplay>> readReplaysTasks = new List<Task<FortniteReplayReader.Models.FortniteReplay>>();
        Console.WriteLine($"Processing Chunk: {currentChunk}");
        DataSet replaysDataset = new DataSet();
        replaysDataset.Tables.Add("sessionTable");
        replaysDataset.Tables.Add("players");
        replaysDataset.Tables.Add("sessionPlayers");
        replaysDataset.Tables.Add("sessionPlayerMovement");
        foreach (FileInfo replayChunkFiles in replayChunk)
        {
          Console.WriteLine($"Extracting: {replayChunkFiles.ToString()}");
          // Parse Replay
          readReplaysTasks.Add(Task.Run(() => reader.ReadReplay(replayChunkFiles.FullName)));
        }
        foreach (Task<FortniteReplayReader.Models.FortniteReplay> readReplay in readReplaysTasks) {
          // Console.WriteLine($"Extracting: {replayChunkFiles}");
          string nanoGameSessionId = Nanoid.Nanoid.Generate(size: 8);
          readReplay.Wait();
          var replay = readReplay.Result;
          replaysDataset.Tables["sessionTable"].Merge(SessionParser.ParseSession(replay, nanoGameSessionId));
          replaysDataset.Tables["players"].Merge(SessionParser.ParsePlayers(replay));
          replaysDataset.Tables["sessionPlayers"].Merge(SessionParser.ParseSessionPlayers(replay, nanoGameSessionId, replaysDataset.Tables["players"]));
          replaysDataset.Tables["sessionPlayerMovement"].Merge(SessionParser.ParsePlayerMovements(replay, nanoGameSessionId, replaysDataset.Tables["players"]));
        }

          
       foreach (DataTable tbl in replaysDataset.Tables)
          {
            string tblName = new string(tbl.TableName);
            string filePath = $".\\{currentChunk}\\";
            string fileName = $".\\{currentChunk}\\{tblName}.csv";
            bool pathExists = System.IO.Directory.Exists(filePath);
            if (pathExists == false) { System.IO.Directory.CreateDirectory(filePath); }
            tbl.ToCSV(fileName);
            FileInfo fileToCompress = new FileInfo(fileName);

            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
              using (FileStream compressedFileStream = File.Create(fileName + ".gz"))
              {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                  originalFileStream.CopyTo(compressionStream);
                }
              }
            }
            fileToCompress.Delete();
          }
        currentChunk = currentChunk + 1;
      }


      /*
     // Get Replay Metadata
        int replayLength = Convert.ToInt32(replay.Info.LengthInMs);
        var mapdata = replay.MapData;
        var llamas = mapdata.Llamas;
        var safezones = mapdata.SafeZones;
        var stats = replay.Stats;
        var Elims = replay.Eliminations;
        var Players = replay.PlayerData;

      // Write Replay Safe Zone Data
      foreach (FortniteReplayReader.Models.SafeZone szone in safezones) {
        Console.WriteLine($"radius: {szone.Radius} center: {szone.LastCenter} next_center: {szone.NextCenter} start_shrink: {szone.StartShrinkTime} n_rad: {szone.NextRadius} last_radius: {szone.LastRadius} ");
      }

      Write Replay Elimination Data
      foreach (FortniteReplayReader.Models.Events.PlayerElimination elim in Elims)
      {
        Inserts.SessionEliminationsWriter(NanoGameSessionId, elim, S2Conn);
      }
      // Write Replay Player Movement Data
      SessionParser.ParsePlayerMovements(replay, NanoGameSessionId, playersDict);
      */
    }
  }
}