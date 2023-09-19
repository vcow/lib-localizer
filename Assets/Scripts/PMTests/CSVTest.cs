using System.Text.RegularExpressions;
using Base.Localization.Template;
using NUnit.Framework;
using UnityEngine;

namespace PMTests
{
	public class CSVTest
	{
		[Test]
		public void CSVTestSimplePasses()
		{
			var patterns = new[]
			{
				new Regex(@"^col #\d{1,2}$"),
				new Regex(@"^row #\d{1,2}$"),
				new Regex(@"^This is a simple text \d{1,2} wo any suprises\.$"),
				new Regex(@"^This is the, text \d{1,2}, with, coma$"),
				new Regex(@"^This is ""text \d{1,2} with quotes""$"),
				new Regex(@"^The text \d{1,2} "" with ""quotes"", and "" comas\. $"),
				new Regex(@"^The multiline\r?\nand multiline\r?\nand multiline\r?\ntext \d{1,2}$", RegexOptions.Multiline),
				new Regex(@"^The multiline,\r?\nand multiline,\r?\nand multiline,\r?\ntext \d{1,2} with comas$", RegexOptions.Multiline),
				new Regex(@"^The multiline,\r?\nand ""multiline,\r?\nand multiline,\r?\ntext \d{1,2} with"" comas and quotes$", RegexOptions.Multiline),
			};

			var asset = Resources.Load<TextAsset>("test");
			var raw = asset.text;
			using (var csv = CSV.Parse(raw))
			{
				while (csv.MoveNext())
				{
					var cols = csv.Current;
					Assert.IsNotNull(cols);
					foreach (var row in cols)
					{
						if (row == string.Empty)
						{
							continue;
						}

						var success = false;
						foreach (var pattern in patterns)
						{
							var m = pattern.Match(row);
							if (m.Success)
							{
								success = true;
								break;
							}
						}

						Assert.IsTrue(success, "The string {0} isn't match any pattern.", row);
					}
				}
			}
		}
	}
}