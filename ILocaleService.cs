using System;
using Base.GameService;
using UnityEngine;

namespace Base.LocaleService
{
	public class CurrentLanguageChangedEventArgs : EventArgs
	{
		public SystemLanguage CurrentLanguage { get; }
		public SystemLanguage PreviousLanguage { get; }

		public CurrentLanguageChangedEventArgs(SystemLanguage currentLanguage, SystemLanguage previousLanguage)
		{
			CurrentLanguage = currentLanguage;
			PreviousLanguage = previousLanguage;
		}
	}

	public interface ILocaleService : IGameService
	{
		/// <summary>
		/// Ключ текущей локализации.
		/// </summary>
		SystemLanguage CurrentLanguage { get; }

		/// <summary>
		/// Событие смены текущего языка локализации.
		/// </summary>
		event EventHandler CurrentLanguageChangedEvent;

		/// <summary>
		/// Задать текущий язык локализации.
		/// </summary>
		/// <param name="lang">Новый текущий язык локализации.</param>
		void SetCurrentLanguage(SystemLanguage lang);

		/// <summary>
		/// Получить локализованную строку по ее ключу.
		/// </summary>
		/// <param name="key">Ключ.</param>
		/// <returns>Локализованное значение, или ключ, если значение для текущей локализации отсутствует.</returns>
		string GetLocalized(string key);

		/// <summary>
		/// Получить локализованную строку по ее ключу.
		/// </summary>
		/// <param name="key">Ключ.</param>
		/// <param name="language">Локализация, для которой запрашивается значение.</param>
		/// <returns></returns>
		string GetLocalized(string key, SystemLanguage language);

		/// <summary>
		/// Локализовать указанный UI.
		/// </summary>
		/// <param name="ui">Корневой объект локализуемого UI.</param>
		/// <param name="applyController">Флаг, указывающий применить ко всем найденным текстовым
		/// элементам контроллер с целью отслеживания смены локализации пользователем.</param>
		void Localize(GameObject ui, bool applyController = false);

		/// <summary>
		/// Добавить к локализациям новый CSV файл. 
		/// </summary>
		/// <param name="rawData">Текстовое содержимое CSV файла в формате UTF-8.</param>
		/// <returns>Возвращает <code>true</code>, если содержимое файла успешно добавлено в локализатор.</returns>
		bool AddLocaleCsv(string rawData);
	}
}