using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Glauz.Blendshapes
{
    [RequireComponent(typeof(Slider))]
    public class BlendShapeSlider : MonoBehaviour
    {

        //Do not need suffix
        [Header("Do not include the suffixes of the BlendShape Name")]
        public string blendShapeName;
        private Slider slider;


        private void Start()
        {
            blendShapeName = blendShapeName.Trim();
            slider = GetComponent<Slider>();

            //When slider is moved, then call function based on the blendshape name and pass float of slider


            slider.onValueChanged.AddListener(value => CharacterCustomization.Instance.ChangeBlendshapeValue(blendShapeName, value));
        }

    } 
}
