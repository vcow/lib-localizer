#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Base.Localization.Template.Editor
{
	[CustomEditor(typeof(LocaleTMPControllerBase), true)]
	public class LocaleTMPControllerEditor : UnityEditor.Editor
	{
		private ReorderableList _formatArgumentsList;

		private void OnEnable()
		{
			var argsProp = serializedObject.FindProperty("_formatArguments");
			_formatArgumentsList = new ReorderableList(serializedObject, argsProp, true, true, true, true);
			_formatArgumentsList.drawHeaderCallback += rect =>
			{
				GUI.Label(rect, new GUIContent(argsProp.displayName));
			};
			_formatArgumentsList.drawElementCallback += (rect, index, active, focused) =>
			{
				EditorGUI.PropertyField(rect, argsProp.GetArrayElementAtIndex(index), GUIContent.none, true);
			};
		}

		public override void OnInspectorGUI()
		{
			using (new LocalizationGroup(target))
			{
				EditorGUI.BeginChangeCheck();
				serializedObject.UpdateIfRequiredOrScript();
				var iterator = serializedObject.GetIterator();
				var hideSettings = false;
				for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
				{
					switch (iterator.propertyPath)
					{
						case "m_Script":
							break;
						case "_localizeText":
							EditorGUILayout.PropertyField(iterator, true);
							hideSettings = !iterator.boolValue;
							break;
						case "_formatArguments":
							if (!hideSettings)
							{
								_formatArgumentsList.DoLayoutList();
							}

							break;
						default:
							if (!hideSettings)
							{
								EditorGUILayout.PropertyField(iterator, true);
							}

							break;
					}
				}

				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
#endif