using ICSharpCode.SharpZipLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Node
{
    public int Index { get; set; }
    public string Point { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<int> ConnectedNodes { get; set; }
    public double GCost { get; set; }
    public double HCost { get; set; }
    public double FCost => GCost + HCost;
    public Node Parent { get; set; }
}
public class PathFind : MonoBehaviour
{
    public TextAsset pathTextAsset;
    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    public Test test;

    public int start;
    public int end;
    private void Start()
    {
        LoadNodesFromCSV(pathTextAsset);
    }
    public void PathFinding()
    {
        var path = FindPath(start, end);
        for (int i = 0; i < path.Count - 1; i++)
        {
            double[] v = new double[4];
            v[0] = path[i].Latitude;
            v[1] = path[i].Longitude;
            v[2] = path[i + 1].Latitude;
            v[3] = path[i + 1].Longitude;
            test.DropTestLine(v);
        }
    }

    private double CalculateDistance(Node a, Node b)
    {
        const double R = 6371; // 지구 반지름(km)
        var dLat = ToRadian(b.Latitude - a.Latitude);
        var dLon = ToRadian(b.Longitude - a.Longitude);

        var a1 = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadian(a.Latitude)) * Math.Cos(ToRadian(b.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a1), Math.Sqrt(1 - a1));
        return (R * c) * 1000; // 미터 단위로 변환
    }

    private double ToRadian(double degree)
    {
        return degree * Math.PI / 180;
    }

    public List<Node> FindPath(int startIdx, int endIdx)
    {
        // 시작 전 모든 노드의 비용을 초기화
        foreach (var node in nodes.Values)
        {
            node.GCost = double.MaxValue;
            node.Parent = null;
        }

        var openSet = new List<Node>();
        var closedSet = new HashSet<int>();

        var startNode = nodes[startIdx];
        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, nodes[endIdx]);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(x => x.FCost).First();
            Debug.Log(current.Index + " --- " + endIdx);
            if (current.Index == endIdx)
            {
                return RetracePath(startNode, nodes[endIdx]);
            }

            openSet.Remove(current);
            closedSet.Add(current.Index);

            foreach (var neighborIdx in current.ConnectedNodes)
            {
                if (!nodes.ContainsKey(neighborIdx) || closedSet.Contains(neighborIdx))
                    continue;

                var neighbor = nodes[neighborIdx];
                var newGCost = current.GCost + CalculateDistance(current, neighbor);

                if (newGCost < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = CalculateDistance(neighbor, nodes[endIdx]);
                    neighbor.Parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private List<Node> RetracePath(Node start, Node end)
    {
        var path = new List<Node>();
        var current = end;

        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        return path.Count > 0 ? path : null;
    }

    public void LoadNodesFromCSV(TextAsset _csv)
    {
        Dictionary<string, Dictionary<string, object>> path = CSVReader.ReadStringIndex(_csv);
        foreach(var p in path)
        {
            var node = new Node
            {
                Index = int.Parse(p.Key),
                Point = p.Value["point"].ToString(),
                Latitude = double.Parse(p.Value["latitude"].ToString()),
                Longitude = double.Parse(p.Value["longitude"].ToString()),
                ConnectedNodes = p.Value["node"].ToString().Split('.').Select(int.Parse).ToList(),
                GCost = double.MaxValue
            };
            nodes.Add(node.Index, node);
        }

    }
}
