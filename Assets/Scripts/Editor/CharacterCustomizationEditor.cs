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
        private bool runOnce;

        public Canvas canvas;

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

            //Space
            EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();

            var characterCustomization = (CharacterCustomization)target;

            //IF NO TARGET, THEN DON"T SHOW ANYTHING
            if (characterCustomization.target == null)
            {
                EditorGUILayout.LabelField("PLEASE SET A TARGET WITH BLENDSHAPES!", EditorStyles.boldLabel);

                //if (GUILayout.Button("REFRESH"))
                //    runOnce = false;

                return;
            }

            EditorGUILayout.LabelField("CREATE SLIDER", EditorStyles.boldLabel);


            //If target has been changed then update to new target
            if (characterCustomization.DoesTargetMatchSkmr())
                characterCustomization.ClearDatabase();

            //**Initialize Blendshapes and get from database
            if (characterCustomization.GetNumberOfEntries() <= 0)
                characterCustomization.Initialize();

            string[] blendShapeNames = characterCustomization.GetBlendShapeNames();

            //Check if there were any blendshapes on the target Object
            if (blendShapeNames.Length <= 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("NO BLENDSHAPES DETECTED ON THIS TARGET!", EditorStyles.boldLabel);
                characterCustomization.ClearDatabase();

                //if (GUILayout.Button("REFRESH"))
                //    runOnce = false;

                return;
                //throw new System.Exception("Dictionary Amount is 0 !?");
            }
            //

            shapeBlendSelectedIndex = EditorGUILayout.Popup("BlendShapeName", shapeBlendSelectedIndex, blendShapeNames);

            //Canvas selector
            canvas = EditorGUILayout.ObjectField("Manual Cavas Selection:", canvas, typeof(Canvas), true) as Canvas;

            if (GUILayout.Button("Create Slider"))
            {
                //Auto Find one if canvas is null
                if (canvas == null)
                    canvas = GameObject.FindObjectOfType<Canvas>();

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
                var BShapeSlider = sliderGO.GetComponent<BlendShapeSlider>();
                BShapeSlider.blendShapeName = characterCustomization.GetBlendShapeNames()[shapeBlendSelectedIndex];   //Fill in the name of the selected Blendshape Name
                BShapeSlider.transform.parent = canvas.transform;
                BShapeSlider.name = "Slider " + BShapeSlider.blendShapeName;
                BShapeSlider.transform.GetComponentInChildren<Text>().text = BShapeSlider.blendShapeName;   //Change the Label text for the blendshape
                BShapeSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);

                //Get Blendshape wrapper
                Blendshape blendShape = characterCustomization.GetBlendshape(BShapeSlider.blendShapeName);

                //Slider Component Properties
                var slider = BShapeSlider.GetComponent<Slider>();

                //Debug.Log(blendShape.negativeIndex);

                if (blendShape.negativeIndex == -1)                
                    slider.minValue = 0f;


                else if (blendShape.positiveIndex == -1)
                    slider.maxValue = 0f;

                else                
                    Debug.Log("Both Values for Blendshape are -1!?");

                slider.value = 0f;



                Debug.Log("Slider \"" + BShapeSlider.blendShapeName + "\" Created!");
            }

        }
    } 
}
