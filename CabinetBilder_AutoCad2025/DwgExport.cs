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

[assembly: CommandClass(typeof(CabinetBilder_AutoCad2025.DwgExport))]
namespace CabinetBilder_AutoCad2025
{
    internal class DwgExport
    {
        [CommandMethod("ExportBlocks")]
        public void ExportBlocks2()
        {
            Document acDocMgrCur = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDbCur = acDocMgrCur.Database;
            Editor ed = acDocMgrCur.Editor;

            using DocumentLock acLockDocCur = acDocMgrCur.LockDocument();
            using Transaction acTraCor = acDbCur.TransactionManager.StartTransaction();

            BlockTable acBlkTblCor = acTraCor.GetObject(acDbCur.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord acBlkTblRecCur = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

            if (acBlkTblRecCur != null)
            {
                string exportFolder = PopUp.OpenFolderDialog();

                List<ObjectId> blockRefIds = ConBlockReference.Select(acBlkTblRecCur, acTraCor);
                ed.WriteMessage($"\n{blockRefIds.Count}");

                ConBlockReference.BlocksToDwgs(blockRefIds, exportFolder, acDbCur, acTraCor);
            }
            acTraCor.Commit();
        }

        [CommandMethod("ExportBlocksWoodComponent")]
        public void ExportBlocksWoodComponent()
        {
            Document acDocMgrCur = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDbCur = acDocMgrCur.Database;
            Editor ed = acDocMgrCur.Editor;

            using DocumentLock acLockDocCur = acDocMgrCur.LockDocument();
            using Transaction acTraCor = acDbCur.TransactionManager.StartTransaction();

            BlockTable acBlkTblCor = acTraCor.GetObject(acDbCur.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord acBlkTblRecCur = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

            if (acBlkTblRecCur != null)
            {
                string exportFolder = PopUp.OpenFolderDialog();

                LayerTable lt = acTraCor.GetObject(acDbCur.LayerTableId, OpenMode.ForRead) as LayerTable;
                string layerName = "WoodComponent";

                List<ObjectId> blockRefIds = ConBlockReference.Select(acBlkTblRecCur, layerName, lt, acTraCor);
                ed.WriteMessage($"\n{blockRefIds.Count}");

                ConBlockReference.BlocksToDwgs(blockRefIds, exportFolder, acDbCur, acTraCor);
            }
            acTraCor.Commit();
        }

    }
}

