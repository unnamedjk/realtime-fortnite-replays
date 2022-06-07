using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

using System.Diagnostics;
using Unreal.Core.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using FortniteReplayAnalyzer.Extensions;

namespace Analyzer
{
  class Analyze
  {
    static void Main(string[] args)
    {
      // Setup Service Collection / Logging
      var serviceCollection = new ServiceCollection()
        .AddLogging(loggingBuilder => loggingBuilder
          .AddConsole()
          .SetMinimumLevel(LogLevel.Error));

      var provider = serviceCollection.BuildServiceProvider();
      var logger = provider.GetService<ILogger<Analyze>>();

      // Process Replay File
      string replayFile;
      string db_host;
      string db_port;
      string db_user;
      string db_pass;
      if (args.Length < 5) {
        Console.WriteLine("Command format: FortniteAnalyzer <replay_file> <db_host> <db_port> <db_user> <db_pass>");
        return;
      } else {
        replayFile = args[0];
        db_host    = args[1];
        db_port    = args[2];
        db_user    = args[3];
        db_pass    = args[4];
      }

      // SingleStore Connection Details
      var S2ConnectionString = Extensions.S2ConnectString(db_host, uint.Parse(db_port), db_user, db_pass);
      var S2Conn = Extensions.S2Connect(S2ConnectionString);
      S2Conn.Open();

      // SingleStore Schema Setup
      Ddls.CreateDDLs(S2Conn);
      Console.WriteLine($"\nAnalyzing Replay: {replayFile}");
      int firstDash = replayFile.IndexOf('-') + 1;
      int lastPeriod = replayFile.LastIndexOf('.');
      var timestamp = replayFile.Substring(firstDash, lastPeriod - firstDash);

      var sw = new Stopwatch();
      sw.Start();
      var reader = new ReplayReader(logger, ParseMode.Full);
      var replay = reader.ReadReplay(replayFile);
      var mapdata = replay.MapData;
      var llamas = mapdata.Llamas;
      var stats = replay.Stats;

      Console.WriteLine($"{stats.Revives.GetType()}");

      foreach (FortniteReplayReader.Models.SafeZone szone in mapdata.SafeZones)
      {
        Console.WriteLine($"radius: {szone.Radius} center: {szone.LastCenter} next_center: {szone.NextCenter} start_shrink: {szone.StartShrinkTime} n_rad: {szone.NextRadius} last_radius: {szone.LastRadius} ");
      }

      // Analyze Replay
      string GameSessionId = replay.GameData.GameSessionId;
      var Elims = replay.Eliminations;
      var Players = replay.PlayerData;
      sw.Stop();
      Console.WriteLine($"Read replay in: {sw.Elapsed} seconds");

      // Collect Game Session Data
      sw.Start();

      sw.Stop();
      Console.WriteLine($"Inserted Session Summary in {sw.Elapsed} seconds");

      // Collect Game Session Elimination Details
      sw.Start();
      foreach (FortniteReplayReader.Models.Events.PlayerElimination elim in Elims)
      {
        Dmls.InsertElimination(GameSessionId, elim, S2Conn);
      }
      sw.Stop();
      Console.WriteLine($"Inserted elimination data in {sw.Elapsed} seconds");

      // Collect Game Session Player Data
      sw.Start();
      var tasks = new List<Task>();
      foreach (FortniteReplayReader.Models.PlayerData player in Players)
      { tasks.Add(Task.Run(() => Extensions.ProccessPlayer(replay.GameData, player, S2ConnectionString))); }

      foreach (Task task in tasks)
      { task.Wait(); }
      sw.Stop();
      Console.WriteLine($"Inserted player data in {sw.Elapsed} seconds");

    }
  }
}