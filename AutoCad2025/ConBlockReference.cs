using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(AutoCad2025.ConBlockReference))]
namespace AutoCad2025
{
    public class ConBlockReference
    {
        public static void BlocksToDwgs(List<ObjectId> blockRefIds, string exportFolder, Database acDbCur, Transaction acTraCor)
        {

            ObjectIdCollection acBloTaeRecOIds = new ObjectIdCollection();

            foreach (ObjectId blockId in blockRefIds)
            {
                BlockReference blockRef = acTraCor.GetObject(blockId, OpenMode.ForRead) as BlockReference;
                BlockTableRecord acBlockTblRec = acTraCor.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                if (!acBloTaeRecOIds.Contains(acBlockTblRec.ObjectId))
                {
                    acBloTaeRecOIds.Add(acBlockTblRec.ObjectId);

                    ObjectIdCollection acObjIdColl = ConBlockTableRecord.GetObjectsId(acBlockTblRec);

                    string exportFileName = System.IO.Path.Combine(exportFolder, $"{blockRef.Name}.dwg");

                    ConDocument.SaveToNowDrawingAndClose(exportFileName, acDbCur, acObjIdColl);
                }
            }
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
                if (entity is BlockReference)
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
                    if (entity is BlockReference)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
