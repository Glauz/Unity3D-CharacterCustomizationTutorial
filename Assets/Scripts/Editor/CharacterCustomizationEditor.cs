using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Glauz.Blendshapes
{
    [CustomEditor(typeof(CharacterCustomization))]
    public class CharacterCustomizationEditor : Editor
    {

        private int shapeBlendSelectedIndex = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Space
            EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();


            EditorGUILayout.LabelField("CREATE SLIDER", EditorStyles.boldLabel);
            var characterCustomization = (CharacterCustomization)target;

            //**Initialize Blendshapes and get from database
            if (characterCustomization.GetNumberOfEntries() <= 0)
                characterCustomization.Initialize();

            string[] blendShapeNames = characterCustomization.GetBlendShapeNames();

            if (blendShapeNames.Length <= 0)
                throw new System.Exception("Dictionary Amount is 0 !?");
            //

            shapeBlendSelectedIndex = EditorGUILayout.Popup("BlendShapeName", shapeBlendSelectedIndex, blendShapeNames);


            if (GUILayout.Button("Create Slider"))
            {
                var canvas = GameObject.FindObjectOfType<Canvas>();

                //If canvas doesn't exist, then make one
                if (canvas == null)
                {
                    //canvas = new GameObject("Canvas").AddComponent<Canvas>();
                    //canvas.gameObject.AddComponent<CanvasScaler>();
                    //canvas.gameObject.AddComponent<GraphicRaycaster>();

                    throw new System.Exception("Please add a canvas into your scene!");
                }

                //Instantiate Slider from root Resource folder with path Resources/"Blendshape Slider"
                GameObject sliderGO = Instantiate(Resources.Load("Blendshape Slider", typeof(GameObject))) as GameObject;

                //Get and set properties of slider (parent to canvas)
                var slider = sliderGO.GetComponent<BlendShapeSlider>();
                slider.blendShapeName = characterCustomization.GetBlendShapeNames()[shapeBlendSelectedIndex];   //Fill in the name of the selected Blendshape Name
                slider.transform.parent = canvas.transform;
                slider.name = "Slider " + slider.blendShapeName;
                slider.transform.GetComponentInChildren<Text>().text = slider.blendShapeName;   //Change the Label text for the blendshape
                slider.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);

                Debug.Log("Slider \"" + slider.blendShapeName + "\" Created!");
            }

        }
    } 
}
