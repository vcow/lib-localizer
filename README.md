# Localization Manager
**CAUTION:** <u>If you want to launch sample code from this repository, install Extentject plugin first!</u>

The **Localization Manager** is the basis of a simple text localization manager.

## Installation
You can download and install <code>localizer.unitypackage</code> from this repository or add the Localization Manager base from **Github** as a dependency.

### Github
Go to the <code>manifest.json</code> and in the section <code>dependencies</code> add next dependencies:
```
  "dependencies": {
    "vcow.base.locale": "https://github.com/vcow/lib-localizer.git?path=/Assets/Scripts/Base/Localization#2.0.1",
    "vcow.base.assignments": "https://github.com/vcow/lib-logicality.git?path=/Assets/Scripts/Base/Assignments#3.0.1",
    ...
  }
```

## How to use LocalizationManager
The Localization Manager consists of two basic parts: the **Localization Provider** and the **Localization Manager** itself. The Localization Source is passed to the Manager at the time of its initialization and must be initialized with string values before the manager first requests localization.

### LocalizationProvider
The **Localization Provider** can get its data from any sources depending on its implementation. That can be plain text, XML or CSV text, binary data, local or remote - doesn't matter. The Provider must implements <code>ILocalizationProvider</code> interface.<br/>
Typically, the Provider requires initial initialization to load data, but may not, so the initialization isn't reflected in the <code>ILocalizationProvider</code> interface. You can use the <code>IInitable</code> interface from the <a href="https://github.com/vcow/lib-logicality/tree/master">Logicality</a> library to support Provider initialization.

#### CSVLocalizationProvider
This library includes the <code>CSVLocalizationProvider</code> - provider, that works with tables in the CSV format. It received the references to text resources containing CSV tables as an initialization arguments. The first line of that CSV table represents a header with the language indices, and the left column contains localization keys.

See <a href="https://raw.githack.com/vcow/lib-localizer/master/docs/html/namespaces.html">documentation</a> for details.

### LocaleTextController and LocaleTMPController
These are the components that are added manually or automatically to the <code>Text</code> and <code>TextMeshProUGUI</code> components respectively, and provide UI interaction with the Localization Manager. These components can contain formatted text arguments and interactively track changes of the Localization Manager's current language.<br/>
These components must be inherited from the <code>LocaleTextControllerBase</code> and <code>LocaleTMPControllerBase</code> respectively. Child objects pass a reference to the instance of the Localization Manager to their parent. For exampe, if your Localization Manager is a singleton these controllers can looks like this:
```csharp
using Base.Localization;
using Base.Localization.Template;

public sealed class LocaleTextController : LocaleTextControllerBase
{
	protected override ILocalizationManager LocalizationManager => global::LocalizationManager.Instance;
}
```
and this:
```csharp
using Base.Localization;
using Base.Localization.Template;

public sealed class LocaleTMPController : LocaleTMPControllerBase
{
	protected override ILocalizationManager LocalizationManager => global::LocalizationManager.Instance;
}
```
See <a href="https://raw.githack.com/vcow/lib-localizer/master/docs/html/namespaces.html">documentation</a> for details.

### LocalizationManager
After you create your Localization Provider and text controllers, you need create your own LocalizationManager derived from the <code>LocalizationManagerBase&lt;TTextController, TTMPController></code>. For example:
```csharp
private class LocalizationManagerImpl : LocalizationManagerBase<LocaleTextController, LocaleTMPController>
{
	public LocalizationManagerImpl(ILocalizationProvider localizationProvider, SystemLanguage defaultLanguage) :
		base(localizationProvider, defaultLanguage)
	{
	}
}
```
The implementation of your singleton then can looks like this:
```csharp
using System.Collections.Generic;
using Base.Assignments.Initable;
using Base.Localization;
using Base.Localization.Template;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class LocalizationManager : MonoBehaviour
{
	private class LocalizationManagerImpl : LocalizationManagerBase<LocaleTextController, LocaleTMPController>
	{
		public LocalizationManagerImpl(ILocalizationProvider localizationProvider, SystemLanguage defaultLanguage) :
			base(localizationProvider, defaultLanguage)
		{
		}
	}

	[SerializeField] private List<TextAsset> _localizationTables;

	public static ILocalizationManager Instance { get; private set; }

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		var localizationProvider = new CSVLocalizationProvider();
		localizationProvider.InitCompleteEvent += OnLocalizationProviderInitComplete;
		localizationProvider.Init(_localizationTables);
	}

	private void OnLocalizationProviderInitComplete(IInitable initable)
	{
		var localizationProvider = (CSVLocalizationProvider)initable;
		localizationProvider.InitCompleteEvent -= OnLocalizationProviderInitComplete;

		Instance = new LocalizationManagerImpl(localizationProvider, Application.systemLanguage);
	}
}
```
Create in your first scene the GameObject with that component and add to their <code>Localization Tables</code> list all of your CSV localization tables.

See <a href="https://raw.githack.com/vcow/lib-localizer/master/docs/html/namespaces.html">documentation</a> for details.<br/>
In the sample code from this repository you can also find an implementation for the Dependency Injection (Zenject) project.

After that you can use your Localization Manager from the code as singleton, or just add <code>LocaleTextController</code> and <code>LocaleTMPController</code> to the UI text elements manually. The text from the UI text field will be used like the localization key if <code>Key</code> field of component will not be specified explicitly.

#### How to localize text fields
From code you can get localized string by the localization key with <code>GetLocalized()</code> method.<br/>
Besides you can explicitly add <code>LocaleTextController</code> and <code>LocaleTMPController</code> components to the relevant text objects, or you can automatically localize whole UI element (for example window) with the method <code>Localize()</code>. This method can either simply localize all texts or automatically add <code>LocaleTextController</code> and <code>LocaleTMPController</code> components to text objects in the UI element.
