using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Glauz.Blendshapes
{
    [CustomEditor(typeof(BlendShapeSlider))]
    public class BlendShapeSliderEditor : Editor
    {

        public enum State { auto, manual }
        public State state;
        private BlendShapeSlider blendShapeSlider;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            if (GUILayout.Button("Auto")) state = State.auto;
            if (GUILayout.Button("Manual")) state = State.manual;

            EditorGUILayout.EndHorizontal();

            blendShapeSlider = (BlendShapeSlider)target;

            switch (state)
            {
                case State.auto: GUI_Auto(); break;
                case State.manual: GUI_Manual(); break;
                default: GUI_Auto(); break;
            }


        }

        private void GUI_Manual()
        {
            base.OnInspectorGUI();
        }

        private void GUI_Auto()
        {
            //Find CharacterCustomization in the Scene
            //Get Dictionary
            //Display List of keys as options for popup

            //CharacterCustomization.Instance.Initialize();
            CharacterCustomization characterCustomization = GameObject.FindObjectOfType<CharacterCustomization>();

            if (characterCustomization == null)
            {
                EditorGUILayout.LabelField("Please have the CharacterCustomizer in your scene!");
                throw new System.Exception("Please have the CharacterCustomizer in your scene!");
            }

            if (characterCustomization.GetNumberOfEntries() <= 0)
                characterCustomization.Initialize();

            string[] blendShapeNames = characterCustomization.GetBlendShapeNames();

            if (blendShapeNames.Length <= 0)
                throw new System.Exception("Dictionary Amount is 0 !?");

            int blendShapeID = 0;   //used to check what the manual is set to of order of dictionary

            for (int i = 0; i < blendShapeNames.Length; i++)
                if (blendShapeSlider.blendShapeName == blendShapeNames[i])
                    blendShapeID = i;

            blendShapeID = EditorGUILayout.Popup("BlendShapeName", blendShapeID, blendShapeNames);
            blendShapeSlider.blendShapeName = blendShapeNames[blendShapeID];

        }

    } 
}
