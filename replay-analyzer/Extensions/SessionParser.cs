using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SingleStoreConnector;
using System.IO;
using System.IO.Compression;

namespace FortniteReplayAnalyzer.Extensions
{
  class SessionParser
  {

    public static string EpicId;
    public static string NanoEpicId;
    public static DataTable ParseSession(FortniteReplayReader.Models.FortniteReplay replay, string NanoGameSessionId)
    {
      DataTable sessionTable = new DataTable();
      sessionTable.Columns.Add("GameSessionId", typeof(string));
      sessionTable.Columns.Add("NanoGameSessionId", typeof(string));
      sessionTable.Columns.Add("WinningPlayerIds", typeof(string));
      sessionTable.Columns.Add("WinningTeamId", typeof(string));
      sessionTable.Columns.Add("mapInfo", typeof(string));
      sessionTable.Columns.Add("teamSize", typeof(int));
      sessionTable.Columns.Add("totalTeams", typeof(int));
      sessionTable.Columns.Add("totalBots", typeof(int));
      sessionTable.Columns.Add("gridCountX", typeof(float));
      sessionTable.Columns.Add("gridCountY", typeof(float));

      var gameData              = replay.GameData;
      var mapData               = replay.MapData;
      var mapInfo               = gameData.MapInfo;
      var teamSize              = gameData.TeamSize;
      var totalTeams            = gameData.TotalTeams;
      var totalBots             = gameData.TotalBots;
      var gridCountY            = mapData.GridCountY;
      var gridCountX            = mapData.GridCountX;
      string GameSessionId      = gameData.GameSessionId;
      string WinningPlayerIds   = String.Join(",", gameData.WinningPlayerIds);
      
      string WinningTeamId      = gameData.WinningTeam.ToString();
      sessionTable.Rows.Add(GameSessionId, NanoGameSessionId, WinningPlayerIds, WinningTeamId, mapInfo, teamSize, totalTeams, totalBots, gridCountX, gridCountY);
      return sessionTable;
    }

    public static DataTable ParsePlayers(FortniteReplayReader.Models.FortniteReplay replay)
    {
      
      DataTable playersTable = new DataTable();
      playersTable.Columns.Add("epicId", typeof(string));
      playersTable.Columns.Add("nanoEpicId", typeof(string));
      foreach (FortniteReplayReader.Models.PlayerData player in replay.PlayerData)
      {
        //Console.WriteLine($"isBot: {player.IsBot}\nbotId: {player.BotId}\niD:{player.Id}\nplayerId: {player.PlayerId}");
        if (player.IsBot == false)
        {
          NanoEpicId = Nanoid.Nanoid.Generate(size: 8).ToString(); 
        }
        playersTable.Rows.Add(player.EpicId, NanoEpicId);
      }
      return playersTable;
    }

    public static DataTable ParseEliminations(string GameSessionId, FortniteReplayReader.Models.Events.PlayerElimination elim) {
      DataTable eliminationsTable = new DataTable();
      eliminationsTable.Columns.Add("GameSessionId", typeof(string));
      eliminationsTable.Columns.Add("EliminationId", typeof(string));
      eliminationsTable.Columns.Add("EliminatedId", typeof(string));
      eliminationsTable.Columns.Add("EliminatorId", typeof(string));
      eliminationsTable.Columns.Add("ElimDistance", typeof(string));
      eliminationsTable.Columns.Add("SelfElim", typeof(string));
      eliminationsTable.Columns.Add("GunType", typeof(string));
      eliminationsTable.Columns.Add("IsValidLoc", typeof(string));
      eliminationsTable.Columns.Add("Knocked", typeof(string));
      eliminationsTable.Columns.Add("EliminatorIsBot", typeof(string));
      eliminationsTable.Columns.Add("EliminatorLocation", typeof(string));
      eliminationsTable.Columns.Add("EliminatedIsBot", typeof(string));
      eliminationsTable.Columns.Add("EliminatedLocation", typeof(string));

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
      eliminationsTable.Rows.Add(
        GameSessionId.ToString(),
        elimId.ToString(),
        eliminatedId.ToString(),
        eliminatorId.ToString(),
        elimDistance.ToString(),
        selfElim.ToString(),
        gunType.ToString(),
        validLoc.ToString(),
        knocked.ToString(),
        eliminatorIsBot.ToString(),
        eliminatorLoc.ToString(),
        eliminatedIsBot.ToString(),
        eliminatedLoc.ToString()) ;
      return eliminationsTable;
    }

    public static DataTable ParseSessionPlayers(FortniteReplayReader.Models.FortniteReplay replay, String NanoGameSessionId, DataTable playersTable)
    {
      var gameData = replay.GameData;
      var players = replay.PlayerData;
      DataTable playerSessionTable = new DataTable();
      playerSessionTable.Columns.Add("sessionId", typeof(string));
      playerSessionTable.Columns.Add("epicId", typeof(string));
      playerSessionTable.Columns.Add("isBot", typeof(string));
      playerSessionTable.Columns.Add("playerKills", typeof(int));
      playerSessionTable.Columns.Add("playerLevel", typeof(int));
      playerSessionTable.Columns.Add("playerPlacement", typeof(int));
      playerSessionTable.Columns.Add("playerCurWeapon", typeof(int));
      playerSessionTable.Columns.Add("playerPlatform", typeof(string));
      playerSessionTable.Columns.Add("playerTeamIdx", typeof(int));
      playerSessionTable.Columns.Add("playerTeamKills", typeof(int));
      playerSessionTable.Columns.Add("deathCause", typeof(int));
      playerSessionTable.Columns.Add("deathCircumstance", typeof(int));
      playerSessionTable.Columns.Add("deathLocation", typeof(string));
      playerSessionTable.Columns.Add("deathTime", typeof(float));
      foreach (FortniteReplayReader.Models.PlayerData player in players)
      {
        // Find matching NanoId
        if (player.IsBot == false) {
          EpicId = player.EpicId.ToString();
          string expression = $"epicId = '{player.EpicId}'";
          NanoEpicId = playersTable.Select(expression)[0][1].ToString();
        }

        // Player Details
        var isBot = player.IsBot;
        var playerCurWeapon = Convert.ToInt32(player.CurrentWeapon);
        var playerTeamKills = Convert.ToInt32(player.TeamKills);
        var playerTeamIdx = Convert.ToInt32(player.TeamIndex);
        var playerPlatformNetId = player.PlatformUniqueNetId;
        var playerPlatform = player.Platform;
        var playerPlacement = Convert.ToInt32(player.Placement);
        var playerLevel = Convert.ToInt32(player.Level);
        var playerKills = Convert.ToInt32(player.Kills);
        var locations = player.Locations;

        // Player Costmetics
        var contrail = player.Cosmetics.SkyDiveContrail;
        var pickaxe = player.Cosmetics.Pickaxe;
        var petskin = player.Cosmetics.PetSkin;
        var parts = player.Cosmetics.Parts;
        var musicpack = player.Cosmetics.MusicPack;
        var loadingscreen = player.Cosmetics.LoadingScreen;
        var itemwraps = player.Cosmetics.ItemWraps;
        var isdefaultchar = player.Cosmetics.IsDefaultCharacter;
        var herotype = player.Cosmetics.HeroType;
        var glider = player.Cosmetics.Glider;
        var dances = player.Cosmetics.Dances;
        var gender = player.Cosmetics.CharacterGender;
        var bodyType = player.Cosmetics.CharacterBodyType;
        var bannerIcon = player.Cosmetics.BannerIconId;
        var charecter = player.Cosmetics.Character;
        var bannerColor = player.Cosmetics.BannerColorId;
        var backpack = player.Cosmetics.Backpack;

        // Player Death Details
        float deathTime = Convert.ToInt32(player.DeathTime);
        var deathTags = player.DeathTags;
        var deathLocation = Convert.ToString(player.DeathLocation);
        var deathCircumstance = Convert.ToInt32(player.DeathCircumstance);
        var deathCause = Convert.ToInt32(player.DeathCause);
        playerSessionTable.Rows.Add(
          NanoGameSessionId,
          NanoEpicId,
          Convert.ToString(isBot),
          playerKills,
          playerLevel,
          playerPlacement,
          playerCurWeapon,
          playerPlatform,
          playerTeamIdx,
          playerTeamKills,
          deathCause,
          deathCircumstance,
          deathLocation,
          deathTime
        );
      }
      return playerSessionTable;
    }
    public static DataTable ParsePlayerMovements(FortniteReplayReader.Models.FortniteReplay replay, string NanoGameSessionId, DataTable playersTable)
    {
      var players       = replay.PlayerData;
      DataTable playerMovementTable = new DataTable();
      playerMovementTable.Columns.Add("nanoEpicId", typeof(string));
      playerMovementTable.Columns.Add("nanoGameSessionId", typeof(string));
      playerMovementTable.Columns.Add("worldTime", typeof(float));
      playerMovementTable.Columns.Add("pState", typeof(int));
      playerMovementTable.Columns.Add("yaw", typeof(float));
      playerMovementTable.Columns.Add("x", typeof(float));
      playerMovementTable.Columns.Add("y", typeof(float));
      playerMovementTable.Columns.Add("z", typeof(float));
      
      foreach (FortniteReplayReader.Models.PlayerData player in players)
      {
        //KeyValuePair<string, string> results = playersDict.FirstOrDefault(v => v.Key.Equals(player.EpicId));
        //string NanoEpicId = results.Value;
        if (player.IsBot == false)
        {
          EpicId = player.EpicId.ToString();
          string expression = $"epicId = '{player.EpicId}'";
          NanoEpicId = playersTable.Select(expression)[0][1].ToString();
        }
        var locations = player.Locations;
        foreach (FortniteReplayReader.Models.PlayerMovement playerMove in locations)
        {
          // Obtain state of player
          int crouched = Convert.ToInt16(playerMove.bIsCrouched);
          var movement = playerMove.ReplicatedMovement;
          var pitch = Convert.ToInt32(movement?.Rotation.Pitch);
          var yaw = Convert.ToInt32(movement?.Rotation.Yaw);
          var roll = Convert.ToInt32(movement?.Rotation.Roll);
          var worldTime = playerMove.ReplicatedWorldTimeSeconds;
          int ziplining = Convert.ToInt16(playerMove.bIsZiplining);
          int waitingForEmote = Convert.ToInt16(playerMove.bIsWaitingForEmoteInteraction);
          int isTargeting = Convert.ToInt16(playerMove.bIsTargeting);
          int isSprinting = Convert.ToInt16(playerMove.bIsSprinting);
          int isSlopeSliding = Convert.ToInt16(playerMove.bIsSlopeSliding);
          int isDivingFromPad = Convert.ToInt16(playerMove.bIsSkydivingFromLaunchPad);
          int isDivingFromBus = Convert.ToInt16(playerMove.bIsSkydivingFromBus);
          int isDiving = Convert.ToInt16(playerMove.bIsSkydiving);
          int isEmoting = Convert.ToInt16(playerMove.bIsPlayingEmote);
          int ParachuteOpen = Convert.ToInt16(playerMove.bIsParachuteOpen);
          int ParachuteForced = Convert.ToInt16(playerMove.bIsParachuteForcedOpen);
          int isJumping = Convert.ToInt16(playerMove.bIsJumping);
          int isInWater = Convert.ToInt16(playerMove.bIsInWaterVolume);
          int isInStorm = Convert.ToInt16(playerMove.bIsInAnyStorm);
          int isHonking = Convert.ToInt16(playerMove.bIsHonking);
          int isDBNO = Convert.ToInt16(playerMove.bIsDBNO);

          // Return player state code
          int pState = 0;
          if (crouched        == 1) { pState = 1; }
          if (ziplining       == 1) { pState = 2; }
          if (waitingForEmote == 1) { pState = 3; }
          if (isTargeting     == 1) { pState = 4; }
          if (isSprinting     == 1) { pState = 5; }
          if (isSlopeSliding  == 1) { pState = 6; }
          if (isDiving        == 1) { pState = 7; }
          if (isEmoting       == 1) { pState = 8; }
          if (isDivingFromPad == 1) { pState = 9; }
          if (isDivingFromBus == 1) { pState = 10; }
          if (ParachuteOpen   == 1) { pState = 11; }
          if (ParachuteForced == 1) { pState = 12; }
          if (isJumping       == 1) { pState = 13; }
          if (isInWater       == 1) { pState = 14; }
          if (isInStorm       == 1) { pState = 15; }
          if (isHonking       == 1) { pState = 16; }
          if (isDBNO          == 1) { pState = 17; }

          
          // Get player coordinates
          var location = playerMove.ReplicatedMovement?.Location;
          float x = location.X;
          float y = location.Y;
          float z = location.Z;

          playerMovementTable.Rows.Add(
            NanoEpicId,
            NanoGameSessionId,
            worldTime,
            pState,
            yaw,
            x,
            y,
            z
          );
        }
      }
      return playerMovementTable;
    }
  }
}