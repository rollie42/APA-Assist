using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    static public class ApplicationProperties
    {
        static ApplicationProperties()
        {
            var col = Get<ObservableCollection<Player>>("knownPlayers");

            col.CollectionChanged += async (sender, e) =>
            {
                Application.Current.Properties["knownPlayers"] = JsonConvert.SerializeObject(KnownPlayers);
                await Application.Current.SavePropertiesAsync();
            };

            KnownPlayers = col;
        }

        static public IList<Player> KnownPlayers { get; set; }

        static public Player LastQuickMatchPlayer1()
        {
            return Get<Player>("LastQuickMatchPlayer1", false);
        }

        static public async Task LastQuickMatchPlayer1(Player player)
        {
            await Set("LastQuickMatchPlayer1", player);
        }

        static public Player LastQuickMatchPlayer2()
        {
            return Get<Player>("LastQuickMatchPlayer2", false);
        }

        static public async Task LastQuickMatchPlayer2(Player player)
        {
            await Set("LastQuickMatchPlayer2", player);
        }

        static private T Get<T>(string key, bool create = true) where T : new()
        {
            return Application.Current.Properties.ContainsKey(key)
                    ? JsonConvert.DeserializeObject<T>((string)Application.Current.Properties[key])
                    : (create ? new T() : default(T));
        }

        static private async Task Set<T>(string key, T obj) where T : new()
        {
            Application.Current.Properties[key] = JsonConvert.SerializeObject(obj);
            await Application.Current.SavePropertiesAsync();
        }
    }
}
