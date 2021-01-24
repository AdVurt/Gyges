using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.MeshTilemapBuilder {

    [CreateAssetMenu(fileName = "Mesh Rule Rile", menuName = "Mesh Tilemap Builder/Mesh Rule Tile")]
    public class MeshRuleTile : ScriptableObject {
        public Mesh defaultMesh;
        public Material[] defaultMaterials;
        public List<MeshRule> lines;

        public void GetMeshAndMaterials(MeshTile tile, MeshRule.Directions neighbours, out Mesh mesh, out Material[] mats) {
            MeshRule selectedLine = null;
            for (int i = 0; i < lines.Count; i++) {
                if (lines[i].IsTriggered(neighbours)) {
                    selectedLine = lines[i];
                    break;
                }
            }

            mesh = tile.mode == MeshTile.TileMode.Override ? (tile.overrideMesh ?? defaultMesh) : (selectedLine == null ? defaultMesh : selectedLine.mesh);
            mats = defaultMaterials;
        }
    }

    [System.Serializable]
    public class MeshRule {

        [System.Flags]
        public enum Directions {
            None = 0,
            Northwest = 1,
            North = 2,
            Northeast = 4,
            West = 8,
            Central = 16,
            East = 32,
            Southwest = 64,
            South = 128,
            Southeast = 256
        }

        public int id = 0;
        public Mesh mesh;
        public MeshRuleRelation[] relations = new MeshRuleRelation[] {
            MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
            MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
            MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
            MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
            MeshRuleRelation.NoCheckValue
        };

        public void ResetRelations() {
            relations = new MeshRuleRelation[] {
                MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
                MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
                MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
                MeshRuleRelation.NoCheckValue, MeshRuleRelation.NoCheckValue,
                MeshRuleRelation.NoCheckValue
            };
        }

        /// <summary>
        /// Returns whether or not this rule should be used according to the designed logic.
        /// </summary>
        public bool IsTriggered(Directions neighbours) {
            for (int i = 0; i < relations.Length; i++) {
                if (i == 4)
                    continue;

                if (!relations[i].IsSatisfied( (neighbours & IDToDirection(i+1)) > 0 ))
                    return false;
            }
            return true;
        }


        private Directions IDToDirection(int id) {
            //This approach is required because the enum uses powers of two for its values.
            //Converting via System.Enum.GetValues incurs a massive performance hit.
            switch (id) {
                case 1: return Directions.Northwest;
                case 2: return Directions.North;
                case 3: return Directions.Northeast;
                case 4: return Directions.West;
                case 5: return Directions.Central;
                case 6: return Directions.East;
                case 7: return Directions.Southwest;
                case 8: return Directions.South;
                case 9: return Directions.Southeast;
            }
            return Directions.None;
        }

        /// <summary>
        /// Applies the values from the given dictionary as this Tile's relations
        /// </summary>
        /// <param name="dict">Dictionary to apply values from</param>
        public void ApplyRelations(MeshRuleRelation[] relations) {
            MeshRuleRelation[] rel = relations;
            if (rel.Length != 9) {
                Debug.LogError("Cannot have a number of relations other than 9.");
                return;
            }
            this.relations = rel;
        }

        public override string ToString() {
            return base.ToString();
        }
    }

    [System.Serializable]
    public struct MeshRuleRelation {

        public enum CheckType {
            NoCheck,
            Include,
            Exclude
        }

        public CheckType check;

        public MeshRuleRelation(CheckType checkType) {
            check = checkType;
        }

        /// <summary>
        /// Returns whether this relation is being satisfied.
        /// </summary>
        public bool IsSatisfied(bool hasNeighbour) {
            return check == CheckType.NoCheck ? true : check == (hasNeighbour ? CheckType.Include : CheckType.Exclude);
        }

        public static MeshRuleRelation NoCheckValue => new MeshRuleRelation(CheckType.NoCheck);
    }

}