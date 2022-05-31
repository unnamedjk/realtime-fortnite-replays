using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SingleStoreConnector;
namespace FortniteReplayAnalyzer.Extensions
{
  public static class Extensions
  {
    public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
    {
      foreach (var value in list)
      {
        await func(value);
      }
    }
    public static SingleStoreConnectionStringBuilder S2ConnectString(string host, uint port, string username, string password)
    {
      var ConnectionString = new SingleStoreConnectionStringBuilder
      {
        Server = host,
        UserID = username,
        Port = port,
        Password = password
      };
      return ConnectionString;
    }

    public static SingleStoreConnection S2Connect(SingleStoreConnectionStringBuilder ConnectionString)
    {
      var S2Connection = new SingleStoreConnection(ConnectionString.ToString());
      return S2Connection;
    }
    public static void ProccessPlayer(FortniteReplayReader.Models.GameData gameData, FortniteReplayReader.Models.PlayerData player, SingleStoreConnectionStringBuilder ConnectionString)
    {
      var S2Connection = new SingleStoreConnection(ConnectionString.ToString());
      S2Connection.Open();
      S2Connection.ChangeDatabase("fn_stats");
      var GameSessionId = gameData.GameSessionId;
      if (player.IsBot)
      {
        var playerId = player.BotId;
      }
      else
      {
        var playerId = player.Id;
      }
      // Player Details
      var isBot = player.IsBot;
      var playerCurWeapon = player.CurrentWeapon;
      var playerTeamKills = player.TeamKills;
      var playerTeamIdx = player.TeamIndex;
      var playerPlatformNetId = player.PlatformUniqueNetId;
      var playerPlatform = player.Platform;
      var playerPlacement = player.Placement;
      var playerLevel = player.Level;
      var playerKills = player.Kills;
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
      var deathTime = player.DeathTime;
      var deathTags = player.DeathTags;
      var deathLocation = player.DeathLocation;
      var deathCircumstance = player.DeathCircumstance;
      var deathCause = player.DeathCause;
      var playerInsert = new SingleStoreCommand($"INSERT IGNORE INTO SessionPlayers VALUES(" +
        $"'{GameSessionId}'," +
        $"'{player.EpicId}'," +
        $"'{player.IsBot}'," +
        $"'{playerKills.GetValueOrDefault(0)}'," +
        $"'{playerLevel.GetValueOrDefault(0)}'," +
        $"'{playerPlacement}'," +
        $"'{playerCurWeapon}'," +
        $"'{playerPlatform}'," +
        $"'{playerTeamIdx}'," +
        $"'{playerTeamKills}'," +
        $"'{deathCause}'," +
        $"'{deathCircumstance}'," +
        $"'{deathLocation}'," +
        $"'{deathTime}'" +
        $");", S2Connection);
      Console.WriteLine(playerInsert.CommandText);
      playerInsert.ExecuteNonQuery();
      // Collect Player Movement Data
      foreach (FortniteReplayReader.Models.PlayerMovement playerMove in locations)
      {
        // convert to async for each player  
        var crouched = playerMove.bIsCrouched;
        var movement = playerMove.ReplicatedMovement;
        var worldTime = playerMove.ReplicatedWorldTimeSeconds;
        var ziplining = playerMove.bIsZiplining;
        var waitingForEmote = playerMove.bIsWaitingForEmoteInteraction;
        var isTargeting = playerMove.bIsTargeting;
        var isSprinting = playerMove.bIsSprinting;
        var isSlopeSliding = playerMove.bIsSlopeSliding;
        var isDivingFromPad = playerMove.bIsSkydivingFromLaunchPad;
        var isDivingFromBus = playerMove.bIsSkydivingFromBus;
        var isDiving = playerMove.bIsSkydiving;
        var isEmoting = playerMove.bIsPlayingEmote;
        var ParachuteOpen = playerMove.bIsParachuteOpen;
        var ParachuteForced = playerMove.bIsParachuteForcedOpen;
        var isJumping = playerMove.bIsJumping;
        var isInWater = playerMove.bIsInWaterVolume;
        var isInStorm = playerMove.bIsInAnyStorm;
        var isHonking = playerMove.bIsHonking;
        var isDBNO = playerMove.bIsDBNO;
        var location = playerMove.ReplicatedMovement?.Location;
        var locX = location.X;
        var locY = location.Y;
        // float x = (float)playerMove.ReplicatedMovement?.Location.X;
        // float y = (float)playerMove.ReplicatedMovement?.Location.Y;
        // float z = (float)playerMove.ReplicatedMovement?.Location.Z;
        // var xyz = new String($"{x},{y},{z}");
        var playerMvInsert = new SingleStoreCommand($"INSERT INTO SessionPlayerMovement VALUES(" +
          $"'{player.EpicId}'," +
          $"'{GameSessionId}'," +
          $"'{crouched}'," +
          $"{worldTime}," +
          $"'{ziplining}'," +
          $"'{waitingForEmote}'," +
          $"'{isTargeting}'," +
          $"'{isSprinting}'," +
          $"'{isSlopeSliding}'," +
          $"'{isDiving}'," +
          $"'{isDivingFromBus}'," +
          $"'{isDivingFromPad}'," +
          $"'{ParachuteOpen}'," +
          $"'{ParachuteForced}'," +
          $"'{isJumping}'," +
          $"'{isInWater}'," +
          $"'{isInStorm}'," +
          $"'{isHonking}'," +
          $"'{isDBNO}'," +
          $"'{location}'" +
          //$"{x}," +
          //$"{y}," +
          //$"{z}" +
          $");", S2Connection);
        Console.WriteLine(playerMvInsert.CommandText);
        playerMvInsert.ExecuteNonQuery();
        // Thread.Sleep(333);
      }
      S2Connection.Close();
    }
  }
}