using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleStoreConnector;
namespace FortniteReplayAnalyzer.Extensions
{
  class Dmls
  {
    public static void InsertElimination(string GameSessionId, FortniteReplayReader.Models.Events.PlayerElimination elim, SingleStoreConnection S2Conn) {
      // Elimination Details
      var elimId = elim.Info.Id;
      var elimDistance = elim.Distance;
      var eliminated = elim.Eliminated;
      var eliminator = elim.Eliminator;
      var selfElim = elim.IsSelfElimination;
      var gunType = elim.GunType;
      var validLoc = elim.IsValidLocation;
      var knocked = elim.Knocked;
      var eliminatorInfo = elim.EliminatorInfo;
      var eliminatedInfo = elim.EliminatedInfo;

      // Eliminated Details
      var eliminatedIsBot = eliminatedInfo.IsBot;
      var eliminatedLoc = eliminatedInfo.Location;
      var eliminatedId = eliminatedInfo.Id;

      // Eliminator Details
      var eliminatorIsBot = eliminatorInfo.IsBot;
      var eliminatorLoc = eliminatorInfo.Location;
      var eliminatorId = eliminatorInfo.Id;
      var elimInsert = new SingleStoreCommand($"INSERT IGNORE INTO SessionEliminations VALUES( " +
        $"'{elimId}'," +
        $"'{GameSessionId}'," +
        $"'{eliminated}'," +
        $"'{eliminator}'," +
        $"'{selfElim}'," +
        $"'{gunType}'," +
        $"'{validLoc}'," +
        $"'{knocked}'," +
        $"'{eliminatedIsBot}'," +
        $"'{eliminatorIsBot}'," +
        $"'{eliminatedLoc}'," +
        $"'{eliminatorLoc}');",
        S2Conn
      );

      // Console.WriteLine(elimInsert.CommandText);
      elimInsert.ExecuteNonQuery();
    
    }
    public static void InsertSession(FortniteReplayReader.Models.GameData gameData, FortniteReplayReader.Models.MapData mapData, SingleStoreConnection S2Conn) {
      var mapInfo = gameData.MapInfo;
      var teamSize = gameData.TeamSize;
      var totalTeams = gameData.TotalTeams;
      var totalBots = gameData.TotalBots;
      var gridCountY = mapData.GridCountY;
      var gridCountX = mapData.GridCountX;
      string GameSessionId = gameData.GameSessionId;
      string WinningPlayerIds = String.Join(",", gameData.WinningPlayerIds);
      string WinningTeamId = gameData.WinningTeam.ToString();

      var sessionInsert = new SingleStoreCommand($"INSERT IGNORE INTO Sessions VALUES( " +
        $"'{GameSessionId}'," +
        $"'{WinningPlayerIds}'," +
        $"'{WinningTeamId}'," +
        $"'{mapInfo}'," +
        $"'{teamSize}'," +
        $"'{totalTeams}'," +
        $"'{totalBots}'," +
        $"'{gridCountX}'," +
        $"'{gridCountY}'," +
        $");");
      Console.WriteLine(sessionInsert.CommandText);
    }
    public static void InsertPlayer(SingleStoreConnection S2Conn) { }
    public static void InsertPlayerMovement(SingleStoreConnection S2Conn) { }


  }
}
