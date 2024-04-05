using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;
using System.Diagnostics;

[assembly: CommandClass(typeof(AutoCad2025.ConLine))]

namespace AutoCad2025
{
    public class ConMuveRot
    {

        public static (Matrix3d translationMatrix, Matrix3d rotationMatrix) MoveAndRotate(Region region, double angleInDegrees)
        {

            Point3d minPoint = region.GeometricExtents.MinPoint;
            Point3d maxpoint = region.GeometricExtents.MaxPoint;

            //Edge1
            Vector3d translationVector = new Vector3d(-minPoint.X, -minPoint.Y , -maxpoint.Z);
            Matrix3d translationMatrix = Matrix3d.Displacement(translationVector);

            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            Matrix3d rotationMatrix = Matrix3d.Rotation(angleInRadians, Vector3d.XAxis, Point3d.Origin);


            return (translationMatrix, rotationMatrix);
        }


        public static double GetRegionRollAngle(Region region)
        {
            Vector3d normalVector = region.Normal;

            // Ha a normál vektor Z komponense közel van a nullához, akkor a régió párhuzamos az X-Y síkkal
            if (Math.Abs(normalVector.Z) < Tolerance.Global.EqualPoint)
            {
                return 0.0;
            }
            else
            {
                // Különben meghatározzuk a roll szöget az X-Y sík és a normál vektor közötti szögként
                Vector3d xyPlaneNormal = new Vector3d(0, 0, 1); // Az X-Y sík normál vektora
                double angle = normalVector.GetAngleTo(xyPlaneNormal); // A normál vektor és az X-Y sík normál vektora közötti szög
                return angle; // A roll szög radiánban
            }
        }

        public static void AlignWithXYPlane(Region region)
        {
            // Régió elmozdítása az XY síkra
            Vector3d translationVector = new Vector3d(0, 0, -region.GeometricExtents.MinPoint.Z);
            Matrix3d translationMatrix = Matrix3d.Displacement(translationVector);
            region.TransformBy(translationMatrix);

            // Normál vektor kiszámítása
            Vector3d normal = region.Normal;

            // Szög meghatározása az X-Y sík és a normál vektor között
            double angle = Math.Acos(normal.DotProduct(Vector3d.ZAxis) / (normal.Length * Vector3d.ZAxis.Length));

            // Forgatási tengely kiszámítása
            Vector3d axis = normal.CrossProduct(Vector3d.ZAxis);

            // Régió forgatása
            Matrix3d rotationMatrix = Matrix3d.Rotation(angle, axis, Point3d.Origin);
            region.TransformBy(rotationMatrix);
        }
    }
}


            //string name = "Surface";
            //bool parallel = true;

            //string sheet = "Sheet";
            //string edge = "Edge";
            //double X = region.Normal.X, Y = region.Normal.Y, Z = region.Normal.Z;

            //if (1 == Z) 
            //{ 
            //    name = $"{sheet}1"; 
            //}
            //else if (-1 == Z) 
            //{ 
            //    name = $"{sheet}2"; 
            //}
            //else if (1 == Y) 
            //{ 
            //    name = $"{edge}1"; 
            //}
            //else if (1 == X) 
            //{ 
            //    name = $"{edge}2"; 
            //}
            //else if (-1 == Y) 
            //{ 
            //    name = $"{edge}3"; 
            //}
            //else if (-1 == X)
            //{ 
            //    name = $"{edge}4"; 
            //}
            //else
            //{
            //    if (Y >= X && Y >= Z) 
            //    { 
            //        name += "1"; 
            //    }
            //    else if (X >= Y && X >= Z)
            //    { 
            //        name += $"2"; 
            //    }
            //    else if (Y <= X && Y <= Z) 
            //    { 
            //        name += $"3"; 
            //    }
            //    else if (X <= Y && X <= Z) 
            //    { 
            //        name += $"4"; 
            //    }

            //    parallel = false;
            //}