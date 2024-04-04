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
    public class ConLine
    {
        [CommandMethod("SelectLine")]
        public void SelectLine()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            TypedValue[] lineFilter = new TypedValue[] { new TypedValue((int)DxfCode.Start, "LINE") };
            SelectionFilter lineSelFilter = new SelectionFilter(lineFilter);

            PromptSelectionResult selResult = ed.GetSelection(lineSelFilter);
            if (selResult.Status == PromptStatus.OK)
            {
                SelectionSet selSet = selResult.Value;
                foreach (SelectedObject selObj in selSet)
                {
                    ObjectId objectId = selObj.ObjectId;
                    using (Transaction trans = doc.Database.TransactionManager.StartTransaction())
                    {
                        Entity entity = trans.GetObject(objectId, OpenMode.ForRead) as Entity;
                        if (ItIsCorrectId(objectId, trans))
                        {
                            Line line = (Line)entity;
                            ed.WriteMessage($"\nSelected line with Start point at ({Math.Round(line.StartPoint.X, 2)},{Math.Round(line.StartPoint.Y, 2)},{Math.Round(line.StartPoint.Z, 2)}) and Length {Math.Round(line.Length, 2)}");
                        }
                        trans.Commit();
                    }
                }
            }
            else
            {
                ed.WriteMessage("\nNo lines selected.");
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
                    if (entity is Line)
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
                        if (entity is Line)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

    }
}
