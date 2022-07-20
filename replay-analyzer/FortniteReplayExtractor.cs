using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Unreal.Core.Models.Enums;
using System.Collections.Generic;
using FortniteReplayExtractor.Extensions;
using System.IO;
using System.Data;
using System.IO.Compression;
using System.Threading.Tasks;

namespace FortniteReplayExtractor
{
  class FortniteReplayExtractor
  {
    static public string replaysPath;
    static public string replaysDstFolder;
    static public FortniteReplayReader.Models.FortniteReplay GetReplay(string replayPath) {
      var serviceCollection = new ServiceCollection()
        .AddLogging(loggingBuilder => loggingBuilder
          .AddConsole()
          .SetMinimumLevel(LogLevel.Error));

      var provider = serviceCollection.BuildServiceProvider();
      var logger = provider.GetService<ILogger<FortniteReplayExtractor>>();
      var reader = new ReplayReader(logger, ParseMode.Full);
      var replay = reader.ReadReplay(replayPath);
      return replay;
    }

    static public DataSet GetFortniteReplayDataSet(FortniteReplayReader.Models.FortniteReplay replay)
    {
      string nanoGameSessionId = Nanoid.Nanoid.Generate(size: 8);
      DataSet replayDataset = new DataSet();
      replayDataset.Tables.Add("sessionTable");
      replayDataset.Tables.Add("players");
      replayDataset.Tables.Add("sessionPlayers");
      replayDataset.Tables.Add("sessionPlayerMovement");
      replayDataset.Tables["sessionTable"].Merge(SessionParser.ParseSession(replay, nanoGameSessionId));
      replayDataset.Tables["players"].Merge(SessionParser.ParsePlayers(replay));
      replayDataset.Tables["sessionPlayers"].Merge(SessionParser.ParseSessionPlayers(replay, nanoGameSessionId, replayDataset.Tables["players"]));
      replayDataset.Tables["sessionPlayerMovement"].Merge(SessionParser.ParsePlayerMovements(replay, nanoGameSessionId, replayDataset.Tables["players"]));
      return replayDataset;
    }

    static void Main(string[] args)
    {
      int coreCount = Environment.ProcessorCount;

      Stopwatch sw = new Stopwatch();
      sw.Start();

      // Setup Service Collection & Logging
      var serviceCollection = new ServiceCollection()
        .AddLogging(loggingBuilder => loggingBuilder
          .AddConsole()
          .SetMinimumLevel(LogLevel.Error));

      var provider = serviceCollection.BuildServiceProvider();
      var logger = provider.GetService<ILogger<FortniteReplayExtractor>>();
      var reader = new ReplayReader(logger, ParseMode.Full);
      int replayChunkSize;
      replaysPath = Convert.ToString(args[0]);
      replayChunkSize = Convert.ToInt32(args[1]);
      replaysDstFolder = Convert.ToString(args[2]);
      DirectoryInfo replaysDir = new DirectoryInfo(replaysPath);
      List<FileInfo> dirFiles = new List<FileInfo>(replaysDir.GetFiles());
      List<FileInfo> replayFiles = new List<FileInfo>();
      foreach (FileInfo file in dirFiles) {
        if (file.Extension == ".replay") {
          replayFiles.Add(file);
        }
      }
      List<List<FileInfo>> replayChunks = replayFiles.partition(replayChunkSize);

      int chunkIndex = 1;
      foreach (List<FileInfo> replayChunk in replayChunks)
      {
        List<Task<FortniteReplayReader.Models.FortniteReplay>> readReplayTasks = new List<Task<FortniteReplayReader.Models.FortniteReplay>>();
        List<Task<DataSet>> replayCollectionTasks = new List<Task<DataSet>>();
        List<Task<DataSet>> replayMergeTasks = new List<Task<DataSet>>();
        List<Task> getReplayDatasets = new List<Task>();
        DataSet replayCollection = new DataSet();
        Console.WriteLine($"\nChunk: {chunkIndex}/{replayChunks.Count}");

        foreach (FileInfo replayFile in replayChunk)
        {
          Console.WriteLine($"Adding [{replayFile.Name}] to replayReader task list...");
          readReplayTasks.Add(Task.Run(function: () => GetReplay(replayFile.FullName)));
        }

        Console.WriteLine($"Waiting for {readReplayTasks.Count} replays(s) to be read...");
        foreach (Task readReplayTask in readReplayTasks)
        {
          readReplayTask.Wait();
        }
        Console.WriteLine("Collecting Replay Datasets and merging...");
        foreach (Task<FortniteReplayReader.Models.FortniteReplay> replay in readReplayTasks)
        {
          replayCollectionTasks.Add(Task.Run(function: () => GetFortniteReplayDataSet(replay.Result)));

          foreach (Task<DataSet> replayDataset in replayCollectionTasks)
          {
            replayDataset.Wait();
            replayCollection.Merge(replayDataset.Result,true);
          }
        }

      
      foreach (DataTable tbl in replayCollection.Tables)
        {
          string tblName = new string(tbl.TableName);
          string filePath = $".\\{replaysDstFolder}\\{chunkIndex}\\";
          string fileName = $".\\{replaysDstFolder}\\{chunkIndex}\\{tblName}.csv";
          bool pathExists = System.IO.Directory.Exists(filePath);
          if (pathExists == false) { System.IO.Directory.CreateDirectory(filePath); }
          Console.WriteLine($"Writing {fileName} CSV...");
          tbl.ToCSV(fileName);
          FileInfo fileToCompress = new FileInfo(fileName);

          using (FileStream originalFileStream = fileToCompress.OpenRead())
          {
            using (FileStream compressedFileStream = File.Create(fileName + ".gz"))
            {
              using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
              {
                Console.WriteLine($"Compressing {fileName}");
                originalFileStream.CopyTo(compressionStream);
              }
            }
          }
          Console.WriteLine("Deleting original CSV...");
          fileToCompress.Delete();
        }
        Console.WriteLine("Finished chunk...");
        chunkIndex++;
      }
      Console.WriteLine("Completed");
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