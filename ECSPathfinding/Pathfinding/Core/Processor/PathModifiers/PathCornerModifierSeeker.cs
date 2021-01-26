﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ME.ECS.Pathfinding {

    public class PathCornerModifierSeeker : PathModifierSeeker {

        public PathCornersModifier modifier;

        public override TMod GetModifier<TMod>() {

            return (TMod)(object)this.modifier;

        }

    }
    
    public struct PathCornersModifier : IPathModifier {
        
        public Path Run(Path path, Constraint constraint) {

            ref var nodes = ref path.nodes;
            var corners = PoolListCopyable<Node>.Spawn(10);
            
            var prevDir = -1;
            for (int i = 0; i < nodes.Count - 1; ++i) {

                var node = nodes[i];
                var next = nodes[i + 1];
                var dir = 0;
                var connections = node.GetConnections();
                for (int j = 0; j < connections.Length; ++j) {

                    if (connections[j].index == next.index) {

                        dir = j;
                        break;
                        
                    }
                    
                }

                if (prevDir != dir) {
                    
                    corners.Add(node);
                    prevDir = dir;

                }

            }
            corners.Add(path.graph.GetNearest(nodes[nodes.Count - 1].worldPosition, constraint));

            this.UpdateCorners(path, constraint, nodes, corners);
            path.nodesModified = corners;

            return path;

        }

        private void UpdateCorners(Path path, Constraint constraint, ME.ECS.Collections.ListCopyable<Node> nodes, ME.ECS.Collections.ListCopyable<Node> corners) {
            
            var cons = Constraint.Empty;
            cons.graphMask = constraint.graphMask;
            for (int iter = 0; iter < corners.Count; ++iter) {

                for (int i = 0; i < corners.Count - 2; ++i) {

                    var currentIndex = i;
                    var nextIndex = i + 2;

                    var current = corners[currentIndex];
                    var next = corners[nextIndex];
                    var allWalkable = true;
                    var pos = current.worldPosition;
                    do {

                        pos = Vector3.MoveTowards(pos, next.worldPosition, path.graph.GetNodeMinDistance());
                        
                        var node = path.graph.GetNearest(pos, cons);
                        if ( //node.walkable == false ||
                            Mathf.Abs(node.penalty - current.penalty) > Mathf.Epsilon ||
                            node.IsSuitable(constraint) == false) {

                            allWalkable = false;
                            break;

                        }
                        
                    } while ((pos - next.worldPosition).sqrMagnitude > 0.01f);
                    
                    if (allWalkable == true) {

                        if (i + 1 < corners.Count) {
                        
                            corners.RemoveAt(i + 1);
        
                        }
                        
                    }

                    //var distance = (next.worldPosition - c.worldPosition).magnitude;
                    /*if (Physics.CapsuleCast(
                            c.worldPosition,
                            c.worldPosition + Vector3.up * this.agentHeight,
                            this.agentRadius,
                            next.worldPosition - c.worldPosition,
                            distance,
                            this.collisionMask
                        ) == false) {
                        
                        nodes.RemoveAt(i + 1);
                        --i;

                    }*/

                }

            }

        }

    }

}
