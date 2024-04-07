using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;
using System.Diagnostics;

[assembly: CommandClass(typeof(AutoCad2025.ConMuve))]
namespace AutoCad2025
{
    public class ConMuve
    {
        public static Matrix3d MinPointOrigo(Region region)
        {
            Point3d minPoint = region.GeometricExtents.MinPoint;

            Vector3d translationVector = new Vector3d(-minPoint.X, -minPoint.Y, -minPoint.Z);
            Matrix3d translationMatrix = Matrix3d.Displacement(translationVector);
            return translationMatrix;
        }

        public static Matrix3d XYPlane(Region region)
        {
            Vector3d translationVector = new Vector3d(0, 0, -region.GeometricExtents.MinPoint.Z);
            Matrix3d translationMatrix = Matrix3d.Displacement(translationVector);
            return translationMatrix;
        }
    }
}