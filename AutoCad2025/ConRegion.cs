using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Reflection.Emit;
using Autodesk.AutoCAD.Colors;

[assembly: CommandClass(typeof(AutoCad2025.ConRegion))]
namespace AutoCad2025
{
    public class ConRegion
    {

        [CommandMethod("CheckCircleInRegion")]
        public static void CheckCircleInRegion()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // Kör kiválasztása
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect a circle: ");
            peo.SetRejectMessage("\nSelected object is not a circle.");
            peo.AddAllowedClass(typeof(Circle), false);
            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK) return;

            ObjectId circleId = per.ObjectId;
            Point3d circleCenter;

            // Kör középpontjának meghatározása
            using (Transaction trans = doc.Database.TransactionManager.StartTransaction())
            {
                Circle circle = trans.GetObject(circleId, OpenMode.ForRead) as Circle;
                circleCenter = circle.Center;
                trans.Commit();
            }

            // Régió kiválasztása
            PromptEntityOptions regionPeo = new PromptEntityOptions("\nSelect a region: ");
            regionPeo.SetRejectMessage("\nSelected object is not a region.");
            regionPeo.AddAllowedClass(typeof(Region), false);
            PromptEntityResult regionPer = ed.GetEntity(regionPeo);
            if (regionPer.Status != PromptStatus.OK) return;

            ObjectId regionId = regionPer.ObjectId;

            // Régió határainak vizsgálata
            using (Transaction trans = doc.Database.TransactionManager.StartTransaction())
            {
                Region region = trans.GetObject(regionId, OpenMode.ForRead) as Region;
                if (region != null)
                {
                    bool isInside = IsPointInsideRegionExtent(circleCenter, region);
                    trans.Commit();

                    if (isInside)
                    {
                        ed.WriteMessage("\nThe circle is inside the region.");
                    }
                    else
                    {
                        ed.WriteMessage("\nThe circle is outside the region.");
                    }
                }
            }
        }

        [CommandMethod("NumberRegionsTheModel")]
        public static void NumberRegionsTheModel()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                List<ObjectId> regionIds = new List<ObjectId>();

                foreach (ObjectId objId in btr)
                {
                    using Entity entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;
                    if (entity != null && entity is Region)
                    {
                        regionIds.Add(objId);
                    }
                }

                trans.Commit();
                if (regionIds.Count > 0)
                {
                    ed.WriteMessage($"\nFound {regionIds.Count} region(s) in the drawing.");

                }
                else
                {
                    ed.WriteMessage("\nNo region found in the drawing.");
                }
            }
        }

        public static Line[] ToLine(Region region)
        {
            Extents3d extents = region.GeometricExtents;

            double minX = extents.MinPoint.X;
            double minY = extents.MinPoint.Y;
            double minZ = extents.MinPoint.Z;

            double maxX = extents.MaxPoint.X;
            double maxY = extents.MaxPoint.Y;
            double maxZ = extents.MaxPoint.Z;

            Point3d point_0 = new Point3d(minX, minY, minZ);
            Point3d point_1 = new Point3d(minX, maxY, minZ);
            Point3d point_2 = new Point3d(maxX, maxY, maxZ);
            Point3d point_3 = new Point3d(maxX, minY, maxZ);

            Line[] lines = [
                new Line(point_0, point_1),
                new Line(point_1, point_2),
                new Line(point_2, point_3),
                new Line(point_3, point_0),
            ];

            return lines;
        }

        public static Polyline ToPolyline(Region region)
        {
            Polyline pline = new Polyline();
            Extents3d extents = region.GeometricExtents;

            double minX = extents.MinPoint.X;
            double minY = extents.MinPoint.Y;

            double maxX = extents.MaxPoint.X;
            double maxY = extents.MaxPoint.Y;

            pline.AddVertexAt(0, new Point2d(minX, minY), 0, 0, 0);
            pline.AddVertexAt(1, new Point2d(minX, maxY), 0, 0, 0);
            pline.AddVertexAt(2, new Point2d(maxX, maxY), 0, 0, 0);
            pline.AddVertexAt(3, new Point2d(maxX, minY), 0, 0, 0);
            pline.Closed = true;

            return pline;
        }

        public static bool Intersects(Entity entity, Region region)
        {
            Point3dCollection points = new Point3dCollection();
            region.IntersectWith(entity, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
            return points.Count > 0;
        }

        public static bool IsPointInsideRegionExtent(Point3d point, Region region)
        {
            Parameter par = new();
            Extents3d extents = region.GeometricExtents;
            double enduranceLimit = par.EnduranceLimit;

            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            double minX = extents.MinPoint.X - enduranceLimit;
            double minY = extents.MinPoint.Y - enduranceLimit;
            double minZ = extents.MinPoint.Z - enduranceLimit;

            double maxX = extents.MaxPoint.X + enduranceLimit;
            double maxY = extents.MaxPoint.Y + enduranceLimit;
            double maxZ = extents.MaxPoint.Z + enduranceLimit;

            return x >= minX && x <= maxX &&
                   y >= minY && y <= maxY &&
                   z >= minZ && z <= maxZ;
        }

        public static int InCircle(List<ObjectId> circleObjectIds, ObjectIdCollection surfaceObjectIdCollection, Region region, Transaction acTra)
        {
            int count = 0;
            foreach (ObjectId circleObjectId in circleObjectIds)
            {
                using Circle? circle = acTra.GetObject(circleObjectId, OpenMode.ForRead) as Circle;
                if (circle != null)
                {
                    if (ConRegion.IsPointInsideRegionExtent(circle.Center, region))
                    {
                        count++;
                        surfaceObjectIdCollection.Add(circleObjectId);
                    }
                }
            }
            return count;
        }

        public static int InCircleColor(List<ObjectId> circleObjectIds, Region region, Transaction acTra)
        {
            Parameter.Color color = new();
            int count = 0;
            foreach (ObjectId circleObjectId in circleObjectIds)
            {
                using Circle? circle = acTra.GetObject(circleObjectId, OpenMode.ForWrite) as Circle;
                if (circle != null)
                {
                    if (ConRegion.IsPointInsideRegionExtent(circle.Center, region))
                    {
                        count++;
                        circle.Color = Color.FromRgb(color.Info[0], color.Info[1], color.Info[2]);
                    }
                }
            }
            return count;
        }

        public static int InLine(List<ObjectId> lineObjectIds, ObjectIdCollection surfaceObjectIdCollection, Region region, Transaction acTra)
        {
            int count = 0;
            foreach (ObjectId lineObjectId in lineObjectIds)
            {
                using Line? line = acTra.GetObject(lineObjectId, OpenMode.ForRead) as Line;
                if (line != null)
                {
                    if (ConRegion.IsPointInsideRegionExtent(line.StartPoint, region))
                    {
                        if (ConRegion.IsPointInsideRegionExtent(line.EndPoint, region))
                        {
                            count++;
                            surfaceObjectIdCollection.Add(lineObjectId);
                        }
                    }
                }
            }
            return count;
        }
        public static int InLineColor(List<ObjectId> lineObjectIds, Region region, Transaction acTra)
        {
            Parameter.Color color = new();

            int count = 0;
            foreach (ObjectId lineObjectId in lineObjectIds)
            {
                using Line? line = acTra.GetObject(lineObjectId, OpenMode.ForWrite) as Line;
                if (line != null)
                {
                    if (ConRegion.IsPointInsideRegionExtent(line.StartPoint, region))
                    {
                        if (ConRegion.IsPointInsideRegionExtent(line.EndPoint, region))
                        {
                            count++;
                            line.Color = Color.FromRgb(color.Info[0],color.Info[1], color.Info[2]);
                        }
                    }
                }
            }
            return count;
        }

        public static bool Contain(BlockTableRecord blockTableRecord, Transaction trans)
        {
            bool l = false;
            foreach (ObjectId entityId in blockTableRecord)
            {
                if (ItIsCorrectId(entityId, trans))
                {
                    l = true;
                }
            }
            return l;
        }

        public static List<ObjectId> Select(BlockTableRecord blockTableRecord, Transaction trans)
        {
            List<ObjectId> objIds = new();
            foreach (ObjectId entityId in blockTableRecord)
            {
                if (ItIsCorrectId(entityId, trans))
                {
                    objIds.Add(entityId);
                }
            }
            return objIds;
        }

        public static List<ObjectId> Select(SelectionSet selSet, Transaction trans)
        {
            List<ObjectId> objIds = new();

            foreach (SelectedObject selObj in selSet)
            {
                ObjectId objectId = selObj.ObjectId;
                if (ItIsCorrectId(objectId, trans))
                {
                    objIds.Add(objectId);
                }
            }
            return objIds;
        }

        public static bool ItIsCorrectId(ObjectId objectId, Transaction trans)
        {
            using Entity? entity = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
            if (entity != null)
            {
                if (entity is Region)
                {
                    return true;
                }
            }
            return false;
        }


        public static List<ObjectId> Select(BlockTableRecord blockTableRecord, string layerName, LayerTable lt, Transaction trans)
        {
            List<ObjectId> objIds = new List<ObjectId>();
            foreach (ObjectId entityId in blockTableRecord)
            {
                if (ItIsCorrectId(entityId, layerName, lt, trans))
                {
                    objIds.Add(entityId);
                }
            }
            return objIds;
        }

        public static List<ObjectId> Select(SelectionSet selSet, string layerName, LayerTable lt, Transaction trans)
        {
            List<ObjectId> objIds = new List<ObjectId>();
            foreach (SelectedObject selObj in selSet)
            {
                ObjectId objectId = selObj.ObjectId;
                if (ItIsCorrectId(objectId, layerName, lt, trans))
                {
                    objIds.Add(objectId);
                }
            }
            return objIds;
        }

        public static bool ItIsCorrectId(ObjectId objectId, string layerName, LayerTable lt, Transaction trans)
        {
            if (lt.Has(layerName))
            {
                ObjectId layerId = lt[layerName];
                Entity? entity = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
                if (entity != null && entity.LayerId == layerId)
                {
                    if (entity is Region)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
