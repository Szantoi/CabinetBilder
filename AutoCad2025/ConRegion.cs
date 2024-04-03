using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;

[assembly: CommandClass(typeof(AutoCad2025.ConRegion))]
namespace AutoCad2025
{
    public class ConRegion
    {
       
        [CommandMethod("CheckCircleInRegion")]
        public void CheckCircleInRegion()
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
        public void NumberRegionsTheModel()
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
                    Entity entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;
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

        public static bool Intersects(Entity entity, Region region)
        {
            Point3dCollection points = new Point3dCollection();
            region.IntersectWith(entity, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
            return points.Count > 0;
        }

        public static bool IsPointInsideRegionExtent(Point3d point, Region region)
        {
            Extents3d extents = region.GeometricExtents;

            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            double minX = extents.MinPoint.X;
            double minY = extents.MinPoint.Y;
            double minZ = extents.MinPoint.Z;
            double maxX = extents.MaxPoint.X;
            double maxY = extents.MaxPoint.Y;
            double maxZ = extents.MaxPoint.Z;

            return x >= minX && x <= maxX &&
                   y >= minY && y <= maxY &&
                   z >= minZ && z <= maxZ;
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
            List<ObjectId> objIds = new List<ObjectId>();
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
            List<ObjectId> objIds = new List<ObjectId>();

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
            Entity? entity = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
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
