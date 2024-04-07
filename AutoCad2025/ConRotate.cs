using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;
using System.Diagnostics;

[assembly: CommandClass(typeof(AutoCad2025.ConRotation))]
namespace AutoCad2025
{
    public class ConRotation
    {

        public static Matrix3d Rotation(double angle, Vector3d axis)
        {
            Matrix3d rotationMatrix = Matrix3d.Rotation(angle, axis, Point3d.Origin);
            return rotationMatrix;
        }

        public static Matrix3d XAxis(double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            Matrix3d rotationMatrix = Matrix3d.Rotation(angleInRadians, Vector3d.XAxis, Point3d.Origin);
            return rotationMatrix;
        }

        public static Vector3d ZAxis(Region region)
        {
            Vector3d normal = region.Normal;
            Vector3d axis = normal.CrossProduct(Vector3d.ZAxis);
            return axis;
        }

        public static double AngleXYPlane(Region region)
        {
            Vector3d normal = region.Normal;

            double angle = Math.Acos(normal.DotProduct(Vector3d.ZAxis) / (normal.Length * Vector3d.ZAxis.Length));
            return angle;
        }

        public static Matrix3d XYPlane(Region region)
        {
            // Normál vektor kiszámítása
            Vector3d normal = region.Normal;

            // Szög meghatározása az X-Y sík és a normál vektor között
            double angle = Math.Acos(normal.DotProduct(Vector3d.ZAxis) / (normal.Length * Vector3d.ZAxis.Length));

            // Forgatási tengely kiszámítása
            Vector3d axis = normal.CrossProduct(Vector3d.ZAxis);

            // Régió forgatása
            Matrix3d rotationMatrix = Matrix3d.Rotation(angle, axis, Point3d.Origin);
            //region.TransformBy(rotationMatrix);
            return rotationMatrix;
        }
    }
}
