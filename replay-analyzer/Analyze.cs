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

      // SingleStore Connection Details
      var S2ConnectionString = Extensions.S2ConnectString("localhost", 3306, "root", "fndemo");
      var S2Conn = Extensions.S2Connect(S2ConnectionString);
      S2Conn.Open();

      // SingleStore Schema Setup
      Ddls.CreateDDLs(S2Conn);

      // Process Replay File
      string replayFile;
      if (args.Length < 1) {
        System.Console.WriteLine("Command format: ConsoleReader <replay file folder path>");
        return;
      } else { replayFile = args[0]; }

      Console.WriteLine($"Analyzing Replay: {replayFile}");
      int firstDash = replayFile.IndexOf('-') + 1;
      int lastPeriod = replayFile.LastIndexOf('.');
      var timestamp = replayFile.Substring(firstDash, lastPeriod - firstDash);

      var sw = new Stopwatch();
      sw.Start();
      var reader = new ReplayReader(logger, ParseMode.Full);
      var replay = reader.ReadReplay(replayFile);

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