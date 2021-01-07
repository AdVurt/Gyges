using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(fileName = "Planet", menuName = "Gyges/DynaPlanet")]
    public class DynaPlanet : ScriptableObject {

        public string planetName;
        [Header("Planet")]
        public Texture2D landWaterMap;
        public Color fresnelColour = new Color(1f,1f,1f,0.2f);
        [Header("Water")]
        public Color waterColour1 = new Color(0.2784314f, 0.5019608f, 0.5647059f);
        public Color waterColour2 = new Color(0.1121396f, 0.2794245f, 0.3773585f);
        public Texture2D waterNormal1;
        public Texture2D waterNormal2;
        [Header("Land")]
        public Texture2D landNormalMap;
        [Header("Clouds")]
        public Texture2D clouds;
        public Color cloudColour = Color.white;
        public Vector2 cloudsMoveSpeed = new Vector2(0.01f,0f);
    }

}