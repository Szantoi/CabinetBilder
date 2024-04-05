using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace AutoCad2025
{
    internal class Parameter
    {
        public double EnduranceLimit { get; set; }

        public Parameter()
        {
            EnduranceLimit = 0.1;
        }

        public class Color()
        {
            public byte[] Info { get; } = [0, 0, 255];
        }
    }
}
