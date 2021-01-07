using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class DynaPlanetGenerator : MonoBehaviour {

#if UNITY_EDITOR
        [HideInInspector]
        public bool planetFoldout = false;
#endif

        public DynaPlanet planet;

        private const float _waterSmoothness = 0.488f;
        private const int _waterNormalTiling = 32;
        private const float _waterNormalStrength = 0.569f;

        [SerializeField, HideInInspector]
        private Vector2 _cloudsMoveSpeed = Vector2.zero;
        private Vector2 _cloudOffset = Vector2.zero;
        private Material _material;

        void Awake() {
            UpdatePlanet();
            _material = GetComponent<MeshRenderer>().material;
        }

        void Update() {
            _cloudOffset += _cloudsMoveSpeed * Time.deltaTime;
            _material.SetVector("_CloudsOffset",new Vector4(_cloudOffset.x, _cloudOffset.y));
        }

        public void UpdatePlanet() {
            if (_material == null) {
                _material = GetComponent<MeshRenderer>().sharedMaterial;
            }
            _material.SetTexture("_LandWaterMap", planet.landWaterMap);
            _material.SetColor("_Fresnel", planet.fresnelColour);

            _material.SetColor("_WaterColour1", planet.waterColour1);
            _material.SetColor("_WaterColour2", planet.waterColour2);
            _material.SetTexture("_WaterNormalMap", planet.waterNormal1);
            _material.SetTexture("_WaterNormalMap2", planet.waterNormal2);

            _material.SetTexture("_LandNormalMap", planet.landNormalMap);

            _material.SetTexture("_CloudsTexture", planet.clouds);
            _material.SetColor("_CloudColour", planet.cloudColour);

            _cloudsMoveSpeed = planet.cloudsMoveSpeed;

            //Consts
            _material.SetFloat("_WaterSmoothness", _waterSmoothness);
            _material.SetInt("_WaterNormalTiling", _waterNormalTiling);
            _material.SetFloat("_WaterNormalStrength", _waterNormalStrength);
        }
    }

}