using AutoCad2025;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ViewModel.PointCloudManager;
using CabinetBilder_UI;
using System.Collections.Generic;
using System.Security.Policy;

[assembly: CommandClass(typeof(CabinetBilder_AutoCad2025.Cad))]
namespace CabinetBilder_AutoCad2025
{
    internal class Cad
    {
        [CommandMethod("Cad_Add_To_Block")]
        public void Cad_Add_To_Block()
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

        [CommandMethod("CAD_Add_Selection_To_Block")]
        public void CAD_Add_Selection_To_Block()
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

        [CommandMethod("CAD_Export_Blocks")]
        public static void CAD_Export_Blocks()
        {
            string exportFolder = PopUp.OpenFolderDialog();

            Document acDocMgrCur = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDbCur = acDocMgrCur.Database;
            Editor ed = acDocMgrCur.Editor;

            using DocumentLock acLockDocCur = acDocMgrCur.LockDocument();
            using Transaction acTraCor = acDbCur.TransactionManager.StartTransaction();
            if (acTraCor != null)
            {
                BlockTable? acBlkTblCor = acTraCor.GetObject(acDbCur.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (acBlkTblCor != null)
                {
                    BlockTableRecord? acBlkTblRecCur = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                    if (acBlkTblRecCur != null)
                    {
                        List<ObjectId> blockRefIds = ConBlockReference.Select(acBlkTblRecCur, acTraCor);
                        ed.WriteMessage($"\n{blockRefIds.Count}");

                        ConBlockReference.BlocksToDwgs(blockRefIds, exportFolder, acDbCur, acTraCor);
                    }
                }
                acTraCor.Commit();
            }
        }

        [CommandMethod("CAD_Export_Blocks_Wood_Component")]
        public static void CAD_Export_Blocks_Wood_Component()
        {
            string exportFolder = PopUp.OpenFolderDialog();
            string layerName = "WoodComponent";

            Document acDocMgrCur = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (acDocMgrCur != null)
            {
                Database acDbCur = acDocMgrCur.Database;
                if (acDbCur != null)
                {
                    Editor ed = acDocMgrCur.Editor;

                    using DocumentLock acLockDocCur = acDocMgrCur.LockDocument();
                    using Transaction acTraCor = acDbCur.TransactionManager.StartTransaction();
                    if (acTraCor != null)
                    {
                        BlockTable? acBlkTblCor = acTraCor.GetObject(acDbCur.BlockTableId, OpenMode.ForRead) as BlockTable;
                        if (acBlkTblCor != null)
                        {
                            BlockTableRecord? acBlkTblRecCur = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                            if (acBlkTblRecCur != null)
                            {
                                LayerTable? lt = acTraCor.GetObject(acDbCur.LayerTableId, OpenMode.ForRead) as LayerTable;
                                if (lt != null)
                                {
                                    List<ObjectId> blockRefIds = ConBlockReference.Select(acBlkTblRecCur, layerName, lt, acTraCor);
                                    ed.WriteMessage($"\n{blockRefIds.Count}");

                                    ConBlockReference.BlocksToDwgs(blockRefIds, exportFolder, acDbCur, acTraCor);
                                }
                            }
                        }
                        acTraCor.Commit();
                    }
                }
            }
        }
    }
}

