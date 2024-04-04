using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace CabinetBilder_AutoCad2025
{
    internal class Name
    {
        public static (string name, bool parallel) Surface(Region region)
        {
            string name = "Surface";
            bool parallel = true;

            string sheet = "Sheet";
            string edge = "Edge";
            double X = region.Normal.X, Y = region.Normal.Y, Z = region.Normal.Z;

            if (1 == Z) { name = $"{sheet}1"; }
            else if (-1 == Z) { name = $"{sheet}2"; }
            else if (1 == Y) { name = $"{edge}1"; }
            else if (1 == X) { name = $"{edge}2"; }
            else if (-1 == Y) { name = $"{edge}3"; }
            else if (-1 == X) { name = $"{edge}4"; }
            else
            {
                if (Y >= X && Y >= Z) { name += "1"; }
                else if (X >= Y && X >= Z) { name += $"2"; }
                else if (Y <= X && Y <= Z) { name += $"3"; }
                else if (X <= Y && X <= Z) { name += $"4"; }

                parallel = false;
            }
            return (name, parallel);
        }
        public static (string name, string namePostfix, string RegionNorlamV) SurfaceFileName(Region region, int drawCount)
        {
            string RegionNorlamV = $"({Math.Round(region.Normal.X, 2)},{Math.Round(region.Normal.Y, 2)},{Math.Round(region.Normal.Z, 2)})";
            (string name, _) = Name.Surface(region);

            string namePostfix = $"0{drawCount}";
            if (drawCount < 10)
            {
                namePostfix = $"0{drawCount}";
            }
            namePostfix += $"_{RegionNorlamV}";

            return (name, namePostfix, RegionNorlamV);
        }
    }
}
