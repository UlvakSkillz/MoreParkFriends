using MelonLoader;
using MelonLoader.Preferences;

namespace MoreParkFriends
{
	public class Preferences
	{
		private const string CONFIG_FILE = "config.cfg";
		private const string USER_DATA = "UserData/MoreParkFriends/";
        internal static Dictionary<MelonPreferences_Entry, object> LastSavedValues = new();

        internal static MelonPreferences_Category MoreParkFriendsCategory;
		internal static MelonPreferences_Entry<int> PrefMultiplier;
		internal static MelonPreferences_Entry<bool> PrefShowExpandedPlayers;
        internal static void InitPrefs()
		{
			if (!Directory.Exists(USER_DATA)) { Directory.CreateDirectory(USER_DATA); }

			MoreParkFriendsCategory = MelonPreferences.CreateCategory("MoreParkFriends", "Settings");
			MoreParkFriendsCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

            PrefMultiplier = MoreParkFriendsCategory.CreateEntry("Multiplier", 2, "Multiplier", "Changes the Park Board Player Count Values to Default*Multiplier (Limit: 1-3)", validator: new ValueRange<int>(1, 3));
            PrefShowExpandedPlayers = MoreParkFriendsCategory.CreateEntry("ShowExpandedPlayers", true, "Show Expanded Players", "Scales the Player Tags on the Park Board to Show Every Player in the Park");
            StoreLastSavedPrefs();
		}

		internal static void StoreLastSavedPrefs()
		{
			List<MelonPreferences_Entry> prefs = new();
			prefs.AddRange(MoreParkFriendsCategory.Entries);
			foreach (MelonPreferences_Entry entry in  prefs) { LastSavedValues[entry] = entry.BoxedValue; }
		}

		public static bool AnyPrefsChanged()
		{
			foreach (KeyValuePair<MelonPreferences_Entry, object> pair in LastSavedValues)
			{
				if (!pair.Key.BoxedValue.Equals(pair.Value)) { return true; }
			}
			return false;
		}

        public static bool IsPrefChanged(MelonPreferences_Entry entry)
		{
			if (LastSavedValues.TryGetValue(entry, out object? lastValue)) { return !entry.BoxedValue.Equals(lastValue); }
			return false;
		}
    }
}