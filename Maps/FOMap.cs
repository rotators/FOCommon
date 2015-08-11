﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FOCommon.Maps
{
    public static class MapObjectType
    {
        public const int Critter   = 0;
        public const int Item = 1;
        public const int Scenery = 2;
    }

    public class MapHeader
    {
        public UInt16 Version { get; set; }
        public UInt16 MaxHexX { get; set; }
        public UInt16 MaxHexY { get; set; }
        public UInt16 WorkHexX { get; set; }
        public UInt16 WorkHexY { get; set; }
        public string ScriptModule { get; set; }
        public string ScriptFunc { get; set; }
        public bool NoLogOut { get; set; }
        public string Time { get; set; }
        public string DayTime { get; set; }
        public string DayColor0 { get; set; }
        public string DayColor1 { get; set; }
        public string DayColor2 { get; set; }
        public string DayColor3 { get; set; }

        public MapHeader() { }
        public MapHeader(string ScriptFunc, string ScriptModule, string Time, bool NoLogout)
        {
            this.ScriptFunc = ScriptFunc;
            this.ScriptModule = ScriptModule;
            this.Time = Time;
            this.NoLogOut = NoLogout;
        }
    }

    public class Tile : IComparable<Tile>,  ICloneable
    {
        public bool Roof;
        public UInt16 X;
        public UInt16 Y;
        public string Path;
        public int Layer;
        public int OffsX;
        public int OffsY;
        public int colorOverlay = 0;

        public Tile() { }

        public Tile(bool roof, UInt16 x, UInt16 y, string path)
        {
            Roof = roof;
            X = x;
            Y = y;
            Path = path;
            Layer = 0;
            OffsX = 0;
            OffsY = 0;
        }

        public Tile(bool roof, UInt16 x, UInt16 y, string path, int layer, int offsX, int offsY)
        {
            Roof = roof;
            X = x;
            Y = y;
            Path = path;
            Layer = layer;
            OffsX = offsX;
            OffsY = offsY;
        }

        public int CompareTo(Tile obj)
        {
            int ret = this.Y.CompareTo(obj.Y);
            if (ret != 0) return ret;
            ret = this.X.CompareTo(obj.X);
            if (ret != 0) return ret;
            return this.Roof.CompareTo(obj.Roof);
        }

        public object Clone()
        {
            return new Tile(Roof, X, Y, Path, Layer, OffsX, OffsY);
        }
    }

    public class MapObject : ICloneable
    {
        public int MapObjType;
        public UInt16 ProtoId;
        public UInt16 MapX;
        public UInt16 MapY;
        public Dictionary<string, string> Properties;
        public Dictionary<string, int> CritterParams;
        public int colorOverlay = 0;

        public MapObject()
        {
            MapObjType = 0;
            ProtoId = 0;
            MapX = 0;
            MapY = 0;
            Properties = new Dictionary<string, string>();
            CritterParams = new Dictionary<string, int>();
        }

        public MapObject(int mapObjType, UInt16 protoId, UInt16 mapX, UInt16 mapY)
        {
            MapObjType = mapObjType;
            ProtoId = protoId;
            MapX = mapX;
            MapY = mapY;
            Properties = new Dictionary<string, string>();
            CritterParams = new Dictionary<string, int>();
        }

        public object Clone()
        {
            MapObject obj = new MapObject(MapObjType, ProtoId, MapX, MapY);
            foreach (KeyValuePair<string, string> kvp in Properties)
                obj.Properties.Add(kvp.Key, kvp.Value);
            foreach (KeyValuePair<string, int> kvp in CritterParams)
                obj.CritterParams.Add(kvp.Key, kvp.Value);
            return obj;
        }
    }

    public class Selection : ICloneable
    {
        private bool added;
        public bool Added { get { return added; } set { added = value; } }

        public List<Tile> Tiles = new List<Tile>();
        public List<MapObject> Objects = new List<MapObject>();

        /// <summary>
        /// Get offset
        /// </summary>
        /// <param name="MX"></param>
        /// <param name="MY"></param>
        /// <returns></returns>
        public Point GetOffset(int MX, int MY)
        {
            int X = 0;
            int Y = 0;
            if (Tiles.Count > 0)
            {
                X = MX - Tiles[0].X;
                Y = MY - Tiles[0].Y;
            }
            else
            {
                X = MX - Objects[0].MapX;
                Y = MY - Objects[0].MapY;
            }
            return new Point(X, Y);
        }

        /// <summary>
        /// Move selection object
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void MoveObject(int X, int Y)
        {
            Tiles.ForEach(a =>
            {
                a.X = (ushort)(a.X + X);
                a.Y = (ushort)(a.Y + Y);
            });

            Objects.ForEach(a =>
            {
                a.MapX = (ushort)(a.MapX + X);
                a.MapY = (ushort)(a.MapY + Y);
            });
        }

        public void clear()
        {
            Tiles.Clear();
            Objects.Clear();
        }

        public bool hasAny()
        {
            return Tiles.Count > 0 || Objects.Count > 0;
        }

        public object Clone()
        {
            Selection sel = new Selection();
            Tiles.ForEach((item) => { sel.Tiles.Add((Tile)item.Clone()); });
            Objects.ForEach((item) => { sel.Objects.Add((MapObject)item.Clone()); });

            return sel;
        }
    }

    public class FOMap
    {
        public MapHeader Header;
        public List<Tile> Tiles;
        public List<MapObject> Objects;

        public FOMap()
        {
            Header = new MapHeader();
            Tiles = new List<Tile>();
            Objects = new List<MapObject>();
        }

        public void AddTile(Tile tile) { Tiles.Add(tile); }
        public void AddObject(MapObject obj) { Objects.Add(obj); }
        List<Tile> GetTiles(UInt16 tx, UInt16 ty)
        {
            List<Tile> ret = new List<Tile>();
            Tiles.ForEach(t => {
                    if (t.X == tx && t.Y == ty) ret.Add(t);
                });
            return ret;
        }

        List<Tile> GetTiles(UInt16 tx, UInt16 ty, bool roof)
        {
            List<Tile> ret = new List<Tile>();
            Tiles.ForEach(t =>
            {
                if (t.X == tx && t.Y == ty && t.Roof == roof) ret.Add(t);
            });
            return ret;
        }

        List<MapObject> GetObjects(UInt16 hx, UInt16 hy, int type)
        {
            List<MapObject> ret = new List<MapObject>();
            Objects.ForEach(o => {
                    if (o.MapX == hx && o.MapY == hy && o.MapObjType == type) ret.Add(o);
                });
            return ret;
        }

        List<MapObject> GetObjects(int type)
        {
            List<MapObject> ret = new List<MapObject>();
            Objects.ForEach(o =>
            {
                if (o.MapObjType == type) ret.Add(o);
            });
            return ret;
        }
    }
}