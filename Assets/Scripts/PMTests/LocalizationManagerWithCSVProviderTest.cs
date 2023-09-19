using System;
using System.Collections;
using Base.Localization;
using ModestTree;
using Sample;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PMTests
{
	public class LocalizationManagerWithCSVProviderTest : SceneTestFixture
	{
		[UnityTest]
		public IEnumerator TestLocalizationManagerWithCSVProvider()
		{
			yield return LoadScene("SampleScene");

			var provider = SceneContainer.Resolve<ILocalizationProvider>();
			Assert.IsType<LocalizationProvider>(provider);
			Assert.That(((LocalizationProvider)provider).IsInitialized);

			var localizationManager = SceneContainer.Resolve<ILocalizationManager>();
			Assert.IsType<LocalizationManager>(localizationManager);

			localizationManager.SetCurrentLanguage(SystemLanguage.English);

			var localString1 = SceneContainer.Instantiate<LocalString>(new object[] { "key.1", Array.Empty<object>() });
			Assert.IsEqual(localString1.ToString(), "Some button");

			var localString2 = SceneContainer.Instantiate<LocalString>(new object[]
			{
				"key.2", new object[]
				{
					123,
					1.2345,
					"TEXT FROM ARGS"
				}
			});
			Assert.IsEqual(localString2.ToString(), "Some text 123 and 1.2345 and TEXT FROM ARGS.");

			var localString3 = SceneContainer.Instantiate<LocalString>(new object[] { "key.4", Array.Empty<object>() });
			Assert.IsEqual(localString3.ToString(), "Some text\nmultiline text");

			localizationManager.SetCurrentLanguage(SystemLanguage.Russian);

			Assert.IsEqual(localString1.ToString(), "Какая-то кнопка");
			Assert.IsEqual(localString2.ToString(), "Какой-то текст 123 и 1.2345 и TEXT FROM ARGS.");
			Assert.IsEqual(localString3.ToString(), "Какой-то текст\nмногострочный текст");
		}
	}
}