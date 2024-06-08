using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using EscapeGuan.UI.MapGenerator;

using Pathfinding;

using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

namespace EscapeGuan.MapGenerator
{
    public class Generator : MonoBehaviour
    {
        public GenerationWaiter Waiter;

        public Tilemap Map;
        public Tile GrassTile;
        public BorderTileSet RoadTile;
        public AstarPath Path;
        public int RoadLength, BranchLength;

        public float StepRotateDeg = 5, StartScale = 5;

        public int Size;

        public Transform BorderT, BorderR, BorderB, BorderL;
        public float BorderWidth;

        private void Start()
        {
            StartCoroutine(GenerationTask());
        }

        public IEnumerator GenerationTask()
        {
            ((GridGraph)Path.graphs[0]).SetDimensions(Size, Size, 1);
            ((GridGraph)Path.graphs[0]).Scan();
            BorderT.localScale = new(Size + BorderWidth * 2, BorderWidth);
            BorderR.localScale = new(BorderWidth, Size + BorderWidth * 2);
            BorderB.localScale = BorderT.localScale;
            BorderL.localScale = BorderR.localScale;
            BorderT.position = new(0, Size / 2 + BorderWidth / 2);
            BorderR.position = new(Size / 2 + BorderWidth / 2, 0);
            BorderB.position = new(0, -(Size / 2 + BorderWidth / 2));
            BorderL.position = new(-(Size / 2 + BorderWidth / 2), 0);

            Stopwatch sw = Stopwatch.StartNew();
            Tile[] t = new Tile[Size * Size];
            Array.Fill(t, GrassTile);
            HashSet<Vector3Int> v = new();
            for (int i = -Size / 2; i < Size / 2; i++)
            {
                for (int j = -Size / 2; j < Size / 2; j++)
                    v.Add(new(i, j));
            }
            Map.SetTiles(v.ToArray(), t);
            yield return new WaitForEndOfFrame();
            // Road generation
            Vector2 roadHead = new(0, 0);
            float dir = 0;
            float scale = StartScale;
            HashSet<Vector3Int> roadTiles = new();
            for (int i = 0, br = 0; i < RoadLength; i++, br++)
            {
                if (br >= Random.Range(50, 100))
                {
                    CreateRoadBranch(roadTiles, roadHead, dir, scale);
                    br = 0;
                }
                dir += Random.Range(-StepRotateDeg, StepRotateDeg);
                scale += Random.Range(-(scale - StartScale) / 5 - 2, -(scale - StartScale) / 5 + 2);
                float rad = dir * Mathf.Deg2Rad;
                roadHead += new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                for (int y = (int)roadHead.y - (int)scale / 2; y < roadHead.y + scale / 2; y++)
                {
                    for (int x = (int)roadHead.x - (int)scale / 2; x < roadHead.x + scale / 2; x++)
                        roadTiles.Add(new(x, y));
                }
                if (roadHead.x > Size / 2 | roadHead.y > Size / 2)
                    break;
            }
            yield return new WaitForEndOfFrame();
            roadHead = new(0, 0);
            dir = 180;
            scale = StartScale;
            for (int i = 0, br = 0; i < RoadLength; i++, br++)
            {
                if (br >= Random.Range(100, 150))
                {
                    CreateRoadBranch(roadTiles, roadHead, dir, scale);
                    br = 0;
                }
                dir += Random.Range(-StepRotateDeg, StepRotateDeg);
                scale += Random.Range(-(scale - StartScale) / 5 - 2, -(scale - StartScale) / 5 + 2);
                float rad = dir * Mathf.Deg2Rad;
                roadHead += new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                for (int y = (int)roadHead.y - (int)scale / 2; y < roadHead.y + scale / 2; y++)
                {
                    for (int x = (int)roadHead.x - (int)scale / 2; x < roadHead.x + scale / 2; x++)
                        roadTiles.Add(new(x, y));
                }
                if (roadHead.x > Size / 2 | roadHead.y > Size / 2 | roadHead.x < -Size / 2 | roadHead.y < -Size / 2)
                    roadHead = new(0, 0);
            }
            roadTiles.Distinct();
            roadTiles.RemoveWhere((x) => { return x.x >= Size / 2 | x.x <= -Size / 2 | x.y >= Size / 2 | x.y <= -Size / 2; });
            yield return new WaitForEndOfFrame();
            IEnumerator<Vector3Int> enu = roadTiles.GetEnumerator();
            TileChangeData[] tcds = new TileChangeData[roadTiles.Count];
            for (int i = 0; i < roadTiles.Count && enu.MoveNext(); i++)
            {
                Vector3Int tile = enu.Current;
                Tile final = RoadTile.Full;
                DirBoolTuple d = new();
                if (roadTiles.Contains(tile + Vector3Int.left))
                    d.l = true;
                if (roadTiles.Contains(tile + Vector3Int.up))
                    d.t = true;
                if (roadTiles.Contains(tile + Vector3Int.right))
                    d.r = true;
                if (roadTiles.Contains(tile + Vector3Int.down))
                    d.b = true;

                if (d.Only(0))
                    final = RoadTile.None;
                else if (d.Only(DirBoolType.l))
                    final = RoadTile.L;
                else if (d.Only(DirBoolType.t))
                    final = RoadTile.T;
                else if (d.Only(DirBoolType.r))
                    final = RoadTile.R;
                else if (d.Only(DirBoolType.b))
                    final = RoadTile.B;

                else if (d.Only(DirBoolType.l | DirBoolType.t))
                    final = RoadTile.LT;
                else if (d.Only(DirBoolType.t | DirBoolType.r))
                    final = RoadTile.TR;
                else if (d.Only(DirBoolType.r | DirBoolType.b))
                    final = RoadTile.RB;
                else if (d.Only(DirBoolType.b | DirBoolType.l))
                    final = RoadTile.BL;

                else if (d.Only(DirBoolType.l | DirBoolType.t | DirBoolType.r))
                    final = RoadTile.LTR;
                else if (d.Only(DirBoolType.t | DirBoolType.r | DirBoolType.b))
                    final = RoadTile.TRB;
                else if (d.Only(DirBoolType.r | DirBoolType.b | DirBoolType.l))
                    final = RoadTile.RBL;
                else if (d.Only(DirBoolType.b | DirBoolType.l | DirBoolType.t))
                    final = RoadTile.BLT;

                else if (d.Only(DirBoolType.l | DirBoolType.r))
                    final = RoadTile.LR;
                else if (d.Only(DirBoolType.t | DirBoolType.b))
                    final = RoadTile.TB;
                tcds[i] = new(enu.Current, final, Color.white, Map.GetTransformMatrix(enu.Current));
            }
            Map.SetTiles(tcds, true);
            sw.Stop();
            Waiter.SetElapsed(Size * Size, (float)sw.Elapsed.TotalSeconds);
            yield return new WaitForSecondsRealtime(1);
            Waiter.Hide();
        }

        public void CreateRoadBranch(HashSet<Vector3Int> roadTiles, Vector2 pos, float urot, float scale)
        {
            float dir = urot;
            int sr = Random.Range(0, 2);
            if (sr == 0)
                dir -= Random.Range(30f, 120f);
            else
                dir += Random.Range(30f, 120f);

            Vector2 roadHead = new(pos.x, pos.y);
            float lcscale = scale;
            for (int i = 0; i < BranchLength; i++)
            {
                dir += Random.Range(-StepRotateDeg, StepRotateDeg);
                lcscale += Random.Range(-(lcscale - StartScale) / 5 - 2, -(lcscale - StartScale) / 5 + 2);
                float rad = dir * Mathf.Deg2Rad;
                roadHead += new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                for (int y = (int)roadHead.y - (int)lcscale / 2; y < roadHead.y + lcscale / 2; y++)
                {
                    for (int x = (int)roadHead.x - (int)lcscale / 2; x < roadHead.x + lcscale / 2; x++)
                        roadTiles.Add(new(x, y));
                }
                if (roadHead.x > Size | roadHead.y > Size)
                    break;
            }
        }
    }

    public static class MathfExt
    {
        public static float Avg(params float[] v)
        {
            float f = 0;
            foreach (float i in v)
                f += i;
            return f / v.Length;
        }
    }

    public class DirBoolTuple
    {
        public bool t, r, b, l;

        public bool Only(DirBoolType dc)
        {
            return (byte)((t ? 0b1 : 0b0) | (r ? 0b10 : 0b0) | (b ? 0b100 : 0b0) | (l ? 0b1000 : 0b0)) == (byte)dc;
        }
    }

    public enum DirBoolType : byte
    {
        t = 0b1, r = 0b10, b = 0b100, l = 0b1000
    }

    [Serializable]
    public struct BorderTileSet
    {
        public Tile Full,
            RB, RBL, BL, BLT, LT, LTR, TR, TRB,
            LR, TB,
            L, T, R, B, None;
    }
}
