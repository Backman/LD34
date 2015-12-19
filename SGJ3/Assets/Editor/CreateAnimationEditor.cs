using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditorInternal;
using InControl.ReorderableList;

namespace UnityEditor
{
	public class CreateAnimationEditor : EditorWindow
	{
		private List<Sprite> _sprites = new List<Sprite>();
		private static CreateAnimationEditor _window;
		private ReorderableList _list;
		private string _animationName;
        private Vector2 _scrollPos;

        [MenuItem("Tools/Create Sprite Animation")]
		private static void OpenWindow()
		{
			_window = GetWindow<CreateAnimationEditor>("Create Animation");
		}

		private void OnGUI()
		{
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.Separator();

			ReorderableListGUI.Title("Sprites");
			ReorderableListGUI.ListField(_sprites, DrawSpriteItem);

			_animationName = EditorGUILayout.TextField("Name", _animationName);

			var objects = Selection.objects;
			List<Sprite> sprites = new List<Sprite>();
			for (int i = 0; i < objects.Length; i++)
			{
				if(objects[i].GetType()  != typeof(Sprite))
				{
					continue;
				}
				var sprite = (Sprite)objects[i];
				if(sprite != null)
				{
					sprites.Add(sprite);
				}
			}
			
            GUI.enabled = sprites.Count > 0;
            if(GUILayout.Button("Add Selected Sprites"))
			{
                _sprites.Clear();
                var sort = new Dictionary<int, Sprite>();
                var spriteIndex = new List<int>();
                var sortedSprites = new List<Sprite>();
                foreach (var sprite in sprites)
				{
                    var split = sprite.name.Split('_');
                    var index = int.Parse(split[split.Length - 1]);
                    spriteIndex.Add(index);
                    sort.Add(index, sprite);
                }
                spriteIndex.Sort();
                foreach (var index in spriteIndex)
				{
                    sortedSprites.Add(sort[index]);
                }
				
				_sprites.AddRange(sortedSprites);
			}

			GUI.enabled = _sprites.Count > 0 && !_sprites.Any(s => s == null) && !string.IsNullOrEmpty(_animationName);

			if (GUILayout.Button("Create"))
			{
				CreateAnimation(_sprites, _animationName);
			}
            GUI.enabled = _sprites.Count > 0;
			if (GUILayout.Button("Clear"))
			{
                _sprites.Clear();
            }

            GUILayout.EndScrollView();

            Repaint();
		}

		private Sprite DrawSpriteItem(Rect rect, Sprite sprite)
		{
			return EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), sprite, typeof(Sprite), false) as Sprite;
		}

		private static void CreateAnimation(List<Sprite> sprites, string name)
		{
			var frameCount = sprites.Count;
			var frameLength = 1f / 30f;

			var clip = new AnimationClip();
			clip.frameRate = 30f;
			clip.name = "Anim_" + name;
			//AnimationUtility.GetAnimationClipSettings(clip);

			EditorCurveBinding curveBinding = new EditorCurveBinding();
			curveBinding.type = typeof(SpriteRenderer);
			curveBinding.propertyName = "m_Sprite";

			ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameCount];

			for (int i = 0; i < frameCount; i++)
			{
				ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
				kf.time = i * frameLength;
				kf.value = sprites[i];
				keyFrames[i] = kf;
			}

			AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

			AssetDatabase.CreateAsset(clip, "Assets/Animations/" + clip.name + ".anim");
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
		}
	}
}