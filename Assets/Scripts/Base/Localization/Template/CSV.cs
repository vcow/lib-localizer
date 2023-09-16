using System;
using System.Collections.Generic;
using System.IO;

namespace Base.Localization.Template
{
	/// <summary>
	/// Simple CSV parser.
	/// </summary>
	public static class CSV
	{
		private const char Separator = ',';

		/// <summary>
		/// Get raw CSV formatted text and returns a list of columns by row.
		/// </summary>
		/// <param name="raw">Raw CSV formatted text.</param>
		/// <returns>Returns a list of columns by row.</returns>
		public static IEnumerator<IReadOnlyList<string>> Parse(string raw)
		{
			if (string.IsNullOrEmpty(raw))
			{
				yield break;
			}

			var columns = new List<string>();
			using (var reader = new StringReader(raw))
			{
				string line = null;
				while (reader.ReadLine() is { } nextLine)
				{
					if (string.IsNullOrEmpty(line))
					{
						line = nextLine;
					}
					else
					{
						line += $"\n{nextLine}";
					}

					if (SeparateLine(line, columns))
					{
						line = null;
						yield return columns;
					}
				}
			}
		}

		private static bool SeparateLine(string line, List<string> columns)
		{
			columns.Clear();

			var buffer = new List<char>(line.Length);
			var prev = Separator;
			var quoted = false;

			Action b2s = () =>
			{
				if (quoted && prev == '"')
				{
					buffer.RemoveAt(buffer.Count - 1);
				}

				var s = new string(buffer.ToArray());
				columns.Add(s);
				buffer.Clear();
			};

			foreach (var c in line)
			{
				switch (c)
				{
					case Separator:
						if (quoted)
						{
							if (prev == '"')
							{
								b2s();
								quoted = false;
							}
							else
							{
								buffer.Add(c);
							}
						}
						else
						{
							b2s();
						}

						prev = c;

						break;
					case '"':
						if (prev == Separator)
						{
							quoted = true;
							prev = '\0';
						}
						else if (prev == '"')
						{
							prev = '\0';
						}
						else
						{
							buffer.Add(c);
							prev = c;
						}

						break;
					default:
						buffer.Add(c);
						prev = c;
						break;
				}
			}

			b2s();
			quoted &= prev != '"';

			return !quoted;
		}
	}
}