using System.Collections;
using UnityEngine;

namespace Phezu.Data {

    public static class DataHandler {

        public static bool CheckPlayerProfile(out PlayerProfile currentProfile, IDataMigrator migrator, string path, string encryptionKey, string iv) {
            currentProfile = Load(path, encryptionKey, iv);

            if (currentProfile == null) {
                currentProfile = new() {
                    Version = Application.version,
                    Data = new()
                };
            }

            bool dirtyFlag = false;

            while (currentProfile.Version != Application.version) {
                var upgrader = migrator.GetUpgrader(currentProfile.Version);
                if (upgrader == null)
                    break;
                currentProfile = upgrader.Invoke(currentProfile);
                dirtyFlag = true;
            }

            if (currentProfile.Version != Application.version) {
                Debug.LogError("Unable to read user data");
                return false;
            }

            if (dirtyFlag)
                Save(currentProfile, path, encryptionKey, iv);

            return true;
        }

        public static string GetProfileData(PlayerProfile profile, string key) {
            if (profile == null) {
                return null;
            }
            if (profile.Data == null)
                return null;

            profile.Data.TryGetValue(key, out var value);

            if (string.IsNullOrEmpty(value))
                return null;

            return value;
        }

        //this should maybe return bool and use out param for int
        public static int GetProfileInt(PlayerProfile profile, string key) {
            string intString = GetProfileData(profile, key);

            try {
                if (intString == null || string.IsNullOrEmpty(intString))
                    return 0;
                else
                    return int.Parse(intString);
            }
            catch {
                Debug.Log("Unable to parse progression data | key: " + key + ", value: " + intString);
                return 0;
            }
        }

        public static bool SetProfileData(PlayerProfile profile, string key, string value) {
            if (profile == null) {
                return false;
            }

            if (profile.Data == null)
                profile.Data = new();

            profile.Data[key] = value;

            return true;
        }

        public static PlayerProfile Load(string path, string encryptionKey, string iv) {
            if (EncryptedData.LoadEncryptedObject<PlayerProfile>(path, out var profile, encryptionKey, iv))
                return profile;
            else
                return null;
        }

        public static IEnumerator Load_Async(string path, string encryptionKey, string iv) {
            while (EncryptedData.ThreadStatus == EncryptedData.ThreadMessage.Running) {
                Debug.Log("waiting for another thread to finish");
                yield return null;
            }

            EncryptedData.LoadEncryptedObject_Thread(path, encryptionKey, iv);

            while (EncryptedData.ThreadStatus == EncryptedData.ThreadMessage.Running) {
                Debug.Log("waiting for thread response");
                yield return null;
            }
        }

        public static void Save(PlayerProfile profile, string path, string encryptionKey, string iv) {
            EncryptedData.SaveEncryptedObject(path, profile, encryptionKey, iv);
        }

        public static IEnumerator Save_Async(PlayerProfile profile, string path, string encryptionKey, string iv) {
            while (EncryptedData.ThreadStatus == EncryptedData.ThreadMessage.Running) {
                Debug.Log("waiting for another thread to finish");
                yield return null;
            }

            EncryptedData.SaveEncryptedObject_Thread(path, profile, encryptionKey, iv);

            while (EncryptedData.ThreadStatus == EncryptedData.ThreadMessage.Running) {
                Debug.Log("waiting for thread response");
                yield return null;
            }
        }
    }
}