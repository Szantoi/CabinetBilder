using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(AutoCad2025.ConBlockTableRecord))]
namespace AutoCad2025
{
    public class ConBlockTableRecord
    {
        [CommandMethod("AddToBlock")]
        public void AddToBlock()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityOptions blockOptions = new PromptEntityOptions("\nSelect a block: ");
            blockOptions.SetRejectMessage("\nSelected object is not a block.");
            blockOptions.AddAllowedClass(typeof(BlockReference), false);
            PromptEntityResult blockResult = ed.GetEntity(blockOptions);

            if (blockResult.Status != PromptStatus.OK)
            {
                return;
            }

            PromptEntityOptions entityOptions = new PromptEntityOptions("\nSelect an entity: ");
            PromptEntityResult entityResult = ed.GetEntity(entityOptions);
            if (entityResult.Status != PromptStatus.OK)
            {
                return;
            }

            using DocumentLock acLockDoc = doc.LockDocument();
            using Transaction trans = db.TransactionManager.StartTransaction();
            BlockReference blockRef = trans.GetObject(blockResult.ObjectId, OpenMode.ForRead) as BlockReference;

            if (blockRef == null)
            {
                ed.WriteMessage("\nSelected object is not a block.");
                return;
            }

            Entity ent = trans.GetObject(entityResult.ObjectId, OpenMode.ForRead) as Entity;

            if (ent == null)
            {
                ed.WriteMessage("\nSelected object is not an entity.");
                return;
            }

            BlockTableRecord block1Record = trans.GetObject(blockRef.BlockTableRecord, OpenMode.ForWrite) as BlockTableRecord;

            Entity newEnt = ent.Clone() as Entity;

            Matrix3d blockInverseTransform = blockRef.BlockTransform.Inverse();

            newEnt.TransformBy(blockInverseTransform);

            block1Record.AppendEntity(newEnt);
            trans.AddNewlyCreatedDBObject(newEnt, true);

            trans.Commit();
            doc.SendStringToExecute("_REGEN ", true, false, false);
        }

        [CommandMethod("AddSelectionToBlock")]
        public void AddSelectionToBlock()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityOptions blockOptions = new PromptEntityOptions("\nSelect a block: ");
            blockOptions.SetRejectMessage("\nSelected object is not a block.");
            blockOptions.AddAllowedClass(typeof(BlockReference), false);
            PromptEntityResult blockResult = ed.GetEntity(blockOptions);
            if (blockResult.Status != PromptStatus.OK)
            {
                return;
            }

            using DocumentLock acLockDoc = doc.LockDocument();
            using Transaction trans = db.TransactionManager.StartTransaction();

            BlockReference blockRef = trans.GetObject(blockResult.ObjectId, OpenMode.ForWrite) as BlockReference;
            if (blockRef == null)
            {
                ed.WriteMessage("\nSelected object is not a block.");
                return;
            }

            PromptSelectionResult selectionResult = ed.GetSelection();

            if (selectionResult.Status != PromptStatus.OK)
            {
                return;
            }

            SelectionSet selectionSet = selectionResult.Value;

            BlockTableRecord blockTblRec = trans.GetObject(blockRef.BlockTableRecord, OpenMode.ForWrite) as BlockTableRecord;

            Matrix3d blockInverseTransform = blockRef.BlockTransform.Inverse();
            foreach (SelectedObject selectedObject in selectionSet)
            {
                Entity ent = trans.GetObject(selectedObject.ObjectId, OpenMode.ForRead) as Entity;
                if (ent != null)
                {
                    Entity newEnt = ent.Clone() as Entity;
                    newEnt.TransformBy(blockInverseTransform);
                    blockTblRec.AppendEntity(newEnt);
                    trans.AddNewlyCreatedDBObject(newEnt, true);
                }
                Entity entErase = trans.GetObject(selectedObject.ObjectId, OpenMode.ForWrite) as Entity;
                entErase.Erase();
            }

            trans.Commit();
        }


        [CommandMethod("CreatingABlock")]
        public void CreatingABlock()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }
                    }
                }
                acTrans.Commit();
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
                if (entity is BlockTableRecord)
                {
                    return true;
                }
            }
            return false;
        }

        public static ObjectIdCollection GetObjectsId(BlockTableRecord BlockTblRec)
        {
            ObjectIdCollection objectIds = new ObjectIdCollection();
            foreach (ObjectId entityId in BlockTblRec)
            {
                objectIds.Add(entityId);
            }
            return objectIds;
        }
    }
}
