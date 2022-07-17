using SingleStoreConnector;
namespace FortniteReplayAnalyzer.Extensions
{
  class Schemas
  {
    public static SingleStoreConnection CreateDB(SingleStoreConnection S2Conn)
    {
      new SingleStoreCommand("CREATE DATABASE IF NOT EXISTS fn_stats", S2Conn).ExecuteNonQuery();
      S2Conn.ChangeDatabase("fn_stats");
      return S2Conn;
    }
    public static void DropTables(SingleStoreConnection S2Conn)
    {
        new SingleStoreCommand("" +
          "DROP TABLE IF EXISTS Sessions;" +
          "DROP TABLE IF EXISTS SessionLlamas;" +
          "DROP TABLE IF EXISTS SessionDrops;" +
          "DROP TABLE IF EXISTS SessionEliminations;" +
          "DROP TABLE IF EXISTS SessionPlayers;",
          S2Conn).ExecuteNonQuery();
    }
    public static void CreateTables(SingleStoreConnection S2Conn, bool force)
    {
     if (force == true) { DropTables(S2Conn); }
     new SingleStoreCommand("" +
        "CREATE TABLE IF NOT EXISTS GameSessions(" +
          "GameSessionId VARCHAR(36)," +
          "NanoGameSessionId CHAR(8)," +
          "WinningPlayerIds VARCHAR(34)," +
          "WinningTeamId VARCHAR(34)," +
          "mapInfo VARCHAR(255)," +
          "teamSize VARCHAR(255)," +
          "totalTeams VARCHAR(255)," +
          "totalBots VARCHAR(255)," +
          "gridCountX VARCHAR(255)," +
          "gridCountY VARCHAR(255)," +
          "UNIQUE KEY (GameSessionId)," +
          "SHARD KEY (GameSessionId)" +
        ");",
        S2Conn).ExecuteNonQuery();

      new SingleStoreCommand("" +
         "CREATE TABLE IF NOT EXISTS Players (" +
            "EpicId VARCHAR(36)," +
            "NanoEpicId VARCHAR(8)," +
            "UNIQUE KEY (EpicId)," +
            "SHARD KEY (EpicId)" +
         ");",
         S2Conn)
         .ExecuteNonQuery();

      new SingleStoreCommand("" +
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
        ");",
        S2Conn)
        .ExecuteNonQuery();

     new SingleStoreCommand("" +
        "CREATE TABLE IF NOT EXISTS SessionPlayers (" +
          "sessionId VARCHAR(255)," +
          "epicId VARCHAR(255)," +
          "nanoEpicId VARCHAR(255)," +
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
        ");", S2Conn).ExecuteNonQuery();

     new SingleStoreCommand("" +
        "CREATE TABLE IF NOT EXISTS SessionPlayerMovement (" +
          "tinyEpicId CHAR(22)," +
          "tinySessionId CHAR(22)," +
          "worldTime DECIMAL(6,4) DEFAULT NULL," +
          "pState CHAR(17) NOT NULL," +
          "pitch DECIMAL(6,3) DEFAULT NULL," +
           "yaw DECIMAL(6,3) DEFAULT NULL," +
           "roll DECIMAL(6,3) DEFAULT NULL," +
           "x DECIMAL(6,3) DEFAULT NULL," +
           "y DECIMAL(6,3) DEFAULT NULL," +
           "z DECIMAL(6,3) DEFAULT NULL," +
          "SHARD KEY (tinyEpicId, tinySessionId)" +
        ");", S2Conn).ExecuteNonQuery();
    }
  }
}
