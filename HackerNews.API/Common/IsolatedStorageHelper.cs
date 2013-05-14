// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

namespace HackerNews.API.Common
{
    public static class IsolatedStorageHelper
    {
        public static T GetObject<T>(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                string serializedObject = IsolatedStorageSettings.ApplicationSettings[key].ToString();
                return Deserialize<T>(serializedObject);
            }
            return default(T);
        }

        public static bool Contains(string key)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(key);
        }

        public static void SaveObject<T>(string key, T objectToSave)
        {
            if (objectToSave != null)
            {
                string serializedObject = Serialize<T>(objectToSave);
                IsolatedStorageSettings.ApplicationSettings[key] = serializedObject;
            }
        }

        public static void DeleteObject(string key)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove(key);
        }

        private static string Serialize<T>(object objectToSerialize)
        {
            if (typeof(T) == typeof(string))
            {
                return (string)objectToSerialize;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer s = new XmlSerializer(typeof(T));
                s.Serialize(ms, objectToSerialize);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static T Deserialize<T>(string serializedString)
        {
            if (typeof(T) == typeof(string))
            {
                object o = serializedString;
                return (T)o;
            }

            XmlSerializer s = new XmlSerializer(typeof(T));

            using (XmlReader reader = XmlReader.Create(new StringReader(serializedString)))
            {
                return (T)s.Deserialize(reader);
            }
        }
    }
}
