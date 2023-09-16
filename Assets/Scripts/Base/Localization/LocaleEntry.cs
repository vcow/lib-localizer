using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Localization
{
	/// <summary>
	/// A map of the strings localized with specified language. Returns as IEnumerable the list of supported
	/// localization keys.
	/// </summary>
	public class LocaleEntry : IEnumerable<string>, IEnumerator<string>
	{
		private readonly Dictionary<string, string> _map = new Dictionary<string, string>();

		private Queue<string> _iteratorQueue;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="language">The language of LocaleEntry.</param>
		public LocaleEntry(SystemLanguage language)
		{
			Language = language;
		}

		/// <summary>
		/// The language of this LocaleEntry.
		/// </summary>
		public SystemLanguage Language { get; }

		/// <summary>
		/// Set local sting with the specified key.
		/// </summary>
		/// <param name="key">The key of string.</param>
		/// <param name="value">The string.</param>
		public void SetValue(string key, string value)
		{
			_map[key] = value;
		}

		/// <summary>
		/// Get locale string by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns locale string, or the key itself if there is no associated string.</returns>
		public string GetValue(string key)
		{
			try
			{
				var value = _map[key];
				return string.IsNullOrEmpty(value) ? key : value;
			}
			catch (KeyNotFoundException)
			{
				return key;
			}
		}

		// IEnumerable<string>

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return this;
		}

		// \IEnumerable<string>

		// IEnumerator<string>

		bool IEnumerator.MoveNext()
		{
			if (_iteratorQueue == null)
			{
				_iteratorQueue = new Queue<string>(_map.Keys);
			}
			else
			{
				_iteratorQueue.Dequeue();
			}

			return _iteratorQueue.Count > 0;
		}

		void IEnumerator.Reset()
		{
			((IDisposable)this).Dispose();
		}

		object IEnumerator.Current => _iteratorQueue?.FirstOrDefault();

		void IDisposable.Dispose()
		{
			if (_iteratorQueue == null)
			{
				return;
			}

			_iteratorQueue.Clear();
			_iteratorQueue = null;
		}

		string IEnumerator<string>.Current => _iteratorQueue?.FirstOrDefault();

		// \IEnumerator<string>

		public override string ToString()
		{
			return $"{Language} ({_map.Count} entries)";
		}
	}
}