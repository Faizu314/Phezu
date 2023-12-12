using System.IO;
using UnityEngine;
using System.Threading;

namespace Phezu.Data {

    public static class EncryptedData {

        public enum ThreadMessage {
            Ok = 0,
            Running = 1,
            FileNotFound = 2,
            UnableToReadData = 3
        }

        private static ThreadPackage<object> m_ThreadPackage;

        public static object ThreadReturn => m_ThreadPackage.data;
        public static ThreadMessage ThreadStatus => m_ThreadPackage.message;

        public static void SaveEncryptedObject<T>(string path, T value, string key, string iv) {
            string jsonValue = JsonUtility.ToJson(value);
            Debug.Log("Saving: " + jsonValue);
            string encryptedValue = Encryptor.AESEncrypt(jsonValue, key, iv);
            string filePath = Application.persistentDataPath + "/" + path + ".phezu";

            FileInfo file = new(filePath);
            file.Directory.Create();

            File.WriteAllText(filePath, encryptedValue);
        }

        public static bool LoadEncryptedObject<T>(string path, out T value, string key, string iv) {
            value = default;

            string filePath = Application.persistentDataPath + "/" + path + ".phezu";

            if (!File.Exists(filePath))
                return false;

            string encryptedValue = File.ReadAllText(filePath);
            string jsonValue = Encryptor.AESDecrypt(encryptedValue, key, iv);
            Debug.Log("Loading: " + jsonValue);

            value = JsonUtility.FromJson<T>(jsonValue);

            return true;
        }

        private static void LoadEncryptedObject_Async(string path, string key, string iv) {
            lock (m_ThreadPackage) {
                m_ThreadPackage = null;
            }

            if (!LoadEncryptedObject(path, out object value, key, iv)) {
                lock (m_ThreadPackage) {
                    object data = null;
                    m_ThreadPackage = new(data, ThreadMessage.FileNotFound);
                }
                return;
            }

            lock (m_ThreadPackage) {
                object data = value;
                m_ThreadPackage = new(data, ThreadMessage.Ok);
            }
        }

        private static void SaveEncryptedObject_Async<T>(string path, T value, string key, string iv) {
            lock (m_ThreadPackage) {
                m_ThreadPackage = null;
            }

            SaveEncryptedObject(path, value, key, iv);

            lock (m_ThreadPackage) {
                object data = null;
                m_ThreadPackage = new(data, ThreadMessage.Ok);
            }
        }

        public static void SaveEncryptedObject_Thread<T>(string path, T value, string key, string iv) {
            ThreadStart threadStart = () => SaveEncryptedObject_Async(path, value, key, iv);
            Thread thread = new(threadStart);
            thread.Start();
        }

        public static void LoadEncryptedObject_Thread(string path, string key, string iv) {
            ThreadStart threadStart = () => LoadEncryptedObject_Async(path, key, iv);
            Thread thread = new(threadStart);
            thread.Start();
        }

        private class ThreadPackage<T> {
            public readonly T data;
            public readonly ThreadMessage message;

            public ThreadPackage(T data, ThreadMessage message) {
                this.data = data;
                this.message = message;
            }
        }
    }
}