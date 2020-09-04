namespace Capture.Core
{
    using System.Configuration;

    /// <summary>
    /// Конфигурационные настройки.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Чтение ключа значения.
        /// </summary>
        /// <param name="key">Наименование</param>
        /// <returns>В случае успеха вернет значение, в противном случае - <c>null</c></returns>
        public static string Read(string key)
        {
            //Получение настроек.
            var appSettings = ConfigurationManager.AppSettings;
            //Чтение значения настройки по ключу.
            return appSettings[key] ?? null;
        }
        /// <summary>
        /// Запись ключа значения.
        /// </summary>
        /// <param name="key">Наименование</param>
        /// <param name="value">Значение</param>
        public static void Write(string key, string value)
        {
            //Открытие настоечного файла.
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Получение ключ значения настроек.
            var settings = configFile.AppSettings.Settings;
            //Проверка на наличие.
            if (settings[key] == null)
            {
                //Добавление.
                settings.Add(key, value);
            }
            else
            {
                //Запись.
                settings[key].Value = value;
            }
            //Сохранение.
            configFile.Save(ConfigurationSaveMode.Modified);
            //Обновление.
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
