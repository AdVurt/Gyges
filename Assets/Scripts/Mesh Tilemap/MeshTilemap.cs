using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Game;
using System;

namespace Gyges.MeshTilemapBuilder {

    [ExecuteAlways]
    public class MeshTilemap : MonoBehaviour, IWaveObject, IOverrideOutOfBounds {

        [Tooltip("The camera to use. If left blank, the default will be used.")]
        public Camera renderCamera = null;
        public MeshRuleTile ruleTile;

        private static Dictionary<Vector2Int, MeshRule.Directions> _directionMap = new Dictionary<Vector2Int, MeshRule.Directions>() {
            { new Vector2Int(-1, 1), MeshRule.Directions.Northwest },
            { new Vector2Int(0, 1), MeshRule.Directions.North },
            { new Vector2Int(1, 1), MeshRule.Directions.Northeast },
            { new Vector2Int(-1, 0), MeshRule.Directions.West },
            { new Vector2Int(1, 0), MeshRule.Directions.East },
            { new Vector2Int(-1, -1), MeshRule.Directions.Southwest },
            { new Vector2Int(0, -1), MeshRule.Directions.South },
            { new Vector2Int(1, -1), MeshRule.Directions.Southeast }
        };

        public Vector2Int[] positions = new Vector2Int[0];
        public MeshTile[] tiles = new MeshTile[0];
        public HashSet<Vector2Int> uniquePositions = new HashSet<Vector2Int>();
        public Rect positionBounds = new Rect(0,0,0,0);

        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        public bool StartLogicBeforeGameplay => false;

        public Vector2 Velocity => Vector2.zero;

        void Awake() {
            SyncUniquePositions();
            if (Application.isPlaying && renderCamera == null)
                renderCamera = Camera.main;
        }

        void Update() {

            if (ruleTile == null)
                return;

            if (uniquePositions.Count != positions.Length) {
                SyncUniquePositions();
            }


            for (int i = 0; i < positions.Length && i < tiles.Length; i++) {
                Vector3 pos = CalculatePosition(i);

                ruleTile.GetMeshAndMaterials(tiles[i], GetNeighbours(positions[i]), out Mesh mesh, out Material[] mats);
                if (mesh != null) {
                    for (int j = 0; j < mesh.subMeshCount && j < mats.Length; j++) {
                        Graphics.DrawMesh(mesh, Matrix4x4.TRS(pos, transform.rotation, transform.lossyScale), mats[j], LayerMask.NameToLayer("Default"), renderCamera, j);
                    }
                }
            }
        }

        public void SyncUniquePositions() {
            uniquePositions = new HashSet<Vector2Int>(positions);

            Rect r = new Rect(0,0,0,0);
            foreach(Vector2Int pos in positions) {
                if (pos.x < r.xMin)
                    r.xMin = pos.x;
                if (pos.x > r.xMax)
                    r.xMax = pos.x;
                if (pos.y < r.yMin)
                    r.yMin = pos.y;
                if (pos.y > r.yMax)
                    r.yMax = pos.y;
            }
            r.width++;
            r.height++;
            positionBounds = r;
        }

        /// <summary>
        /// Calculates the central position of the given tile index.
        /// </summary>
        private Vector3 CalculatePosition(int tile) {
            return transform.position + transform.rotation * (Vector2)positions[tile];
        }

        /// <summary>
        /// Gets the directions of all neighbours of the provided position.
        /// </summary>
        /// <param name="position"></param>
        private MeshRule.Directions GetNeighbours(Vector2Int position) {
            MeshRule.Directions result = MeshRule.Directions.Central;

            foreach( Vector2Int pos in _directionMap.Keys ) {
                if (uniquePositions.Contains(position + pos))
                    result |= _directionMap[pos];
            }

            return result;
        }

        public bool IsOutOfBounds() => IsOutOfBounds(out Enemy.OutOfBoundsDirections dummy);

        public bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir) {

            oobDir = Enemy.OutOfBoundsDirections.None;

            if ((transform.position.x + positionBounds.xMax) < EnemyActions.borders.yMin) {
                oobDir |= Enemy.OutOfBoundsDirections.West;
            }
            if ((transform.position.x + positionBounds.xMin) > EnemyActions.borders.xMax) {
                oobDir |= Enemy.OutOfBoundsDirections.East;
            }
            if ((transform.position.y + positionBounds.yMin) > EnemyActions.borders.yMax) {
                oobDir |= Enemy.OutOfBoundsDirections.North;
            }
            if ((transform.position.y + positionBounds.yMax) < EnemyActions.borders.yMin) {
                oobDir |= Enemy.OutOfBoundsDirections.South;
            }

            return oobDir != Enemy.OutOfBoundsDirections.None;
        }

        public Transform GetTransform() => transform;

        public void Kill(bool killedByPlayer) {
            onDestroy?.Invoke(new IWaveObjectDestroyEventParams(this, false, 0));
            Destroy(gameObject);
        }
    }

}