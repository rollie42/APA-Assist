using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public static class LocalStorage
    {
        private const string _folderName = "APA Assist";
        private const string _recentGamesName = "Recent Games";

        public static async Task<IEnumerable<Match>> GetRecentMatches()
        {
            var appFolder = await ApplicationFolder();
            var recentGamesFile = await appFolder.CreateFileAsync(_recentGamesName, CreationCollisionOption.OpenIfExists); // TODO OpenIfExists
            var serialized = await recentGamesFile.ReadAllTextAsync();
            return string.IsNullOrEmpty(serialized) ? new List<Match>() : JsonConvert.DeserializeObject<List<Match>>(serialized);
        }

        public static async Task SaveRecentMatches(IEnumerable<Match> recentGames)
        {
            var appFolder = await ApplicationFolder();
            var recentGamesFile = await appFolder.CreateFileAsync(_recentGamesName, CreationCollisionOption.OpenIfExists); // TODO OpenIfExists
            var serialized = JsonConvert.SerializeObject(recentGames);
            await recentGamesFile.WriteAllTextAsync(serialized);
        }

        public static async Task AddRecentMatch(Match game)
        {
            var games = await GetRecentMatches();
            await SaveRecentMatches(games.Concat(new List<Match> { game }));
        }

        public static async Task<Match> GetMostRecentMatch()
        {
            var matches = await GetRecentMatches();
            return matches.FirstOrDefault();
        }

        private static async Task<IFolder> ApplicationFolder()
        {
            return await FileSystem.Current.LocalStorage.CreateFolderAsync(_folderName, CreationCollisionOption.OpenIfExists);
        }
    }
}
