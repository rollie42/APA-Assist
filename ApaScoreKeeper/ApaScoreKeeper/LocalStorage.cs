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
        private const string _recentGamesName = "Recent Games";

        public static async Task<IEnumerable<Match>> GetRecentMatches()
        {
            var recent = await Read<UniqueIdentifierWrapper<IEnumerable<Match>>>(_recentGamesName);
            return recent.Data ?? new List<Match>();
        }

        public static async Task SaveRecentMatches(IEnumerable<Match> recentGames)
        {
            await Write(UniqueIdentifierWrapper.Create(_recentGamesName, recentGames));
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

        private static async Task Write(IUniqueIdentifier obj)
        {
            // Write object to storage
            var accessor = new StorageAccess();
            await accessor.Write(obj.Id, obj);
        }

        private static async Task<T> Read<T>(string id)
        {
            var accessor = new StorageAccess();
            return (T)(await accessor.Read(id, typeof(T)));
        }
    }
}
