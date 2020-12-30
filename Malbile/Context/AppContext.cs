using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace Malbile.Context
{
    public static class AppContext
    {
        private static IsolatedStorageSettings Settings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// Insere um registro no dispositivo
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        public static void StoreSetting(string settingName, string value)
        {
            StoreSetting<string>(settingName, value);
        }

        public static void StoreSetting<TValue>(string settingName, TValue value)
        {
            if (!Settings.Contains(settingName))
                Settings.Add(settingName, value);
            else
                Settings[settingName] = value;

            Settings.Save();
        }

        /// <summary>
        /// Tenta retornar o registro requerido, se houver
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetSetting<TValue>(string settingName, out TValue value)
        {
            if (Settings.Contains(settingName))
            {
                value = (TValue)Settings[settingName];
                return true;
            }

            value = default(TValue);
            return false;
        }
    }
}
