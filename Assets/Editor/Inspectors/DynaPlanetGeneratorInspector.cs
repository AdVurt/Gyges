using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;


namespace Gyges.CustomEditors {

    [CustomEditor(typeof(DynaPlanetGenerator))]
    public class DynaPlanetGeneratorInspector : Editor {

        private DynaPlanetGenerator _target;
        private Editor _subEditor;

        void OnEnable() {
            _target = (DynaPlanetGenerator)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (_target.planet != null) {

                if (GUILayout.Button("Generate")) {
                    _target.UpdatePlanet();
                }

                _target.planetFoldout = EditorGUILayout.InspectorTitlebar(_target.planetFoldout, _target.planet);
                if (_target.planetFoldout) {
                    CreateCachedEditor(_target.planet, null, ref _subEditor);
                    _subEditor.OnInspectorGUI();
                }
            }
        }

    }

}