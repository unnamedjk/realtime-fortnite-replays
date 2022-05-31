using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleStoreConnector;
namespace FortniteReplayAnalyzer.Extensions
{
  class Ddls
  {
    public static void CreateDDLs(SingleStoreConnection S2Conn)
    {
      string schemaDDL = new string("CREATE DATABASE IF NOT EXISTS fn_stats");
      S2Conn.ChangeDatabase("fn_stats");
      string sessionsDDL = new String("" +
        "CREATE TABLE IF NOT EXISTS GameSessions(" +
          "GameSessionId VARCHAR(255)," +
          "WinningPlayerIds VARCHAR(255)," +
          "WinningTeamId VARCHAR(255)," +
          "mapInfo VARCHAR(255)," +
          "teamSize VARCHAR(255)," +
          "totalTeams VARCHAR(255)," +
          "totalBots VARCHAR(255)," +
          "gridCountX VARCHAR(255)," +
          "gridCountY VARCHAR(255)," +
          "SHARD KEY (GameSessionId)" +
        ");");

      string llamasDDL = new String("" +
        "CREATE TABLE IF NOT EXISTS llamas(" +
        "" +
      ");");
      string dropsDDL = new String("" +
        "CREATE TABLE IF NOT EXISTS drops(" +
        "" +
      ");");
      string eliminationsDDL = new String("" +
        "CREATE TABLE IF NOT EXISTS SessionEliminations (" +
          "eliminationId VARCHAR(36)," +
          "sessionId VARCHAR(36)," +
          "eliminatedId VARCHAR(36)," +
          "eliminatorId VARCHAR(36)," +
          "selfElim VARCHAR(6)," +
          "gunType VARCHAR(255)," +
          "validLoc VARCHAR(255)," +
          "isKnocked VARCHAR(255)," +
          "eliminatedIsBot VARCHAR(255)," +
          "eliminatorIsBot VARCHAR(255)," +
          "eliminatedLoc VARCHAR(255)," +
          "eliminatorLoc VARCHAR(255)," +
          "SHARD KEY (eliminationId, sessionId)" +
        ");");
      string playersDDL = new String("" +
        "CREATE TABLE IF NOT EXISTS SessionPlayers (" +
          "sessionId VARCHAR(255)," +
          "epicId VARCHAR(255)," +
          "IsBot VARCHAR(255) DEFAULT NULL," +
          "playerKills VARCHAR(255) DEFAULT NULL," +
          "playerLevel VARCHAR(255) DEFAULT NULL," +
          "playerPlacement VARCHAR(255) DEFAULT NULL," +
          "playerCurWeapon VARCHAR(255) DEFAULT NULL," +
          "playerPlatform VARCHAR(255) DEFAULT NULL," +
          "playerTeamIdx VARCHAR(255) DEFAULT NULL," +
          "playerTeamKills VARCHAR(255) DEFAULT NULL," +
          "deathCause VARCHAR(255) DEFAULT NULL," +
          "deathCircumstance VARCHAR(255) DEFAULT NULL," +
          "deathLocation VARCHAR(255) DEFAULT NULL," +
          "deathTime VARCHAR(255) DEFAULT NULL," +
          "SHARD KEY (sessionId, epicId)" +
        ");");
      string playerMvDDL = new String("" +
        "CREATE TABLE IF NOT EXISTS SessionPlayerMovement (" +
          "epicId VARCHAR(255)," +
          "sessionId VARCHAR(255)," +
          "crouched VARCHAR(255) DEFAULT NULL," +
          "worldTime VARCHAR(255) DEFAULT NULL," +
          "ziplining VARCHAR(255) DEFAULT NULL," +
          "waitingForEmote VARCHAR(255) DEFAULT NULL," +
          "isTargeting VARCHAR(255) DEFAULT NULL," +
          "isSprinting VARCHAR(255) DEFAULT NULL," +
          "isSlopeSliding VARCHAR(255) DEFAULT NULL," +
          "isDiving VARCHAR(255) DEFAULT NULL," +
          "isDivingFromBus VARCHAR(255) DEFAULT NULL," +
          "isDivingFromPad VARCHAR(255) DEFAULT NULL," +
          "ParachuteOpen VARCHAR(255) DEFAULT NULL," +
          "ParachuteForced VARCHAR(255) DEFAULT NULL," +
          "isJumping VARCHAR(255) DEFAULT NULL," +
          "isInWater VARCHAR(255) DEFAULT NULL," +
          "isInStorm VARCHAR(255) DEFAULT NULL," +
          "isHonking VARCHAR(255) DEFAULT NULL," +
          "isDBNO VARCHAR(255) DEFAULT NULL," +
          "x VARCHAR(255) DEFAULT NULL," +
          "y VARCHAR(255) DEFAULT NULL," +
          "z VARCHAR(255) DEFAULT NULL," +
          "SHARD KEY (epicId, sessionId)" +
        ");");
      var schemaDDLQ = new SingleStoreCommand(schemaDDL, S2Conn);
      var sessionsDDLQ = new SingleStoreCommand(sessionsDDL, S2Conn);
      var eliminationDDLQ = new SingleStoreCommand(eliminationsDDL, S2Conn);
      var playersDDLQ = new SingleStoreCommand(playersDDL, S2Conn);
      var playersMvDDLQ = new SingleStoreCommand(playerMvDDL, S2Conn);
      schemaDDLQ.ExecuteNonQuery();
      sessionsDDLQ.ExecuteNonQuery();
      eliminationDDLQ.ExecuteNonQuery();
      playersDDLQ.ExecuteNonQuery();
      playersMvDDLQ.ExecuteNonQuery();
    }
  }
}
