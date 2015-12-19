using UnityEngine;
using InControl.ReorderableList;
using System.Collections.Generic;

namespace UnityEditor
{
	[CustomEditor(typeof(QuestObjectivesData))]
    public class QuestObjectivesDataEditor : Editor
    {
        private QuestObjectivesData _objectives;
        
        private void OnEnable()
        {
            _objectives = (QuestObjectivesData)target;
            if (_objectives.QuestObjectives == null)
            {
                _objectives.QuestObjectives = new List<QuestObjective>();
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            ReorderableListGUI.Title("Objectives");
			ReorderableListGUI.ListField(_objectives.QuestObjectives, DrawObjectiveItem);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        private QuestObjective DrawObjectiveItem(Rect rect, QuestObjective objective)
		{
			return EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), objective, typeof(QuestObjective), false) as QuestObjective;
		}
    }
}