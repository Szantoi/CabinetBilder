using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;

[assembly: CommandClass(typeof(AutoCad2025.ConSolid3D))]

namespace AutoCad2025
{
    public class ConSolid3D
    {
        [CommandMethod("ExplodeSelection")]
        public static void ExplodeSelection()
        {
            Document? doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database? db = doc.Database;
            Editor? ed = doc.Editor;

            Transaction? trans = db.TransactionManager.StartTransaction();
            try
            {
                BlockTable? bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord? btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Get user selection (3D solid)
                PromptSelectionResult? psr = ed.GetSelection();
                if (psr.Status == PromptStatus.OK)
                {
                    SelectionSet? ss = psr.Value;
                    ExplodeSelectionSet(ss, trans, btr);
                }
                trans.Commit();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error: " + ex.Message);
                trans.Abort();
            }

        }


        public static (double, double, double) GetSolidDimensions(Solid3d? solid)
        {
            if (solid != null)
            {
                Extents3d extents = solid.GeometricExtents;
                double length = extents.MaxPoint.X - extents.MinPoint.X;
                double width = extents.MaxPoint.Y - extents.MinPoint.Y;
                double height = extents.MaxPoint.Z - extents.MinPoint.Z;
                return (length, width, height);
            }
            return (0.0, 0.0, 0.0);
        }

        public static void ExplodeSelectionSet(SelectionSet ss, Transaction trans, BlockTableRecord btr)
        {
            foreach (SelectedObject selObj in ss)
            {
                if (selObj != null)
                {
                    Explode(selObj, trans, btr);
                }
            }
        }

        public static void Explode(SelectedObject selObj, Transaction trans, BlockTableRecord btr)
        {
            ObjectId objectId = selObj.ObjectId;
            Explode(objectId, trans, btr);
        }

        public static void Explode(ObjectId objectId, Transaction trans, BlockTableRecord btr)
        {
            Entity? entity = trans.GetObject(objectId, OpenMode.ForWrite) as Entity;
            if (entity != null)
            {
                DBObjectCollection? explodedObjects = new DBObjectCollection();
                entity.Explode(explodedObjects);
                entity.Erase();
                foreach (DBObject obj in explodedObjects)
                {
                    Entity? explodedEntity = obj as Entity;
                    btr.AppendEntity(explodedEntity);
                    trans.AddNewlyCreatedDBObject(explodedEntity, true);
                }
            }
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
                if (entity is Solid3d)
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
                    if (entity is Solid3d)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
