using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;


[assembly: CommandClass(typeof(AutoCad2025.ConXDATA))]
namespace AutoCad2025
{
    public class ConXDATA
    {
        public static void AddCustomProperty(ObjectId entityId, string propertyName, string propertyValue)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity entity = tr.GetObject(entityId, OpenMode.ForWrite) as Entity;
                if (entity != null)
                {
                    // Set custom properties using XDATA
                    ResultBuffer xdata = new ResultBuffer(
                        new TypedValue((int)DxfCode.ExtendedDataRegAppName, propertyName),
                        new TypedValue((int)DxfCode.ExtendedDataAsciiString, propertyValue)
                    );
                    entity.XData = xdata;

                    tr.Commit();
                }
            }
        }

        public static void ReadCustomProperties(ObjectId entityId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity entity = tr.GetObject(entityId, OpenMode.ForRead) as Entity;
                if (entity != null)
                {
                    ResultBuffer xdata = entity.XData;
                    if (xdata != null)
                    {
                        foreach (TypedValue tv in xdata)
                        {
                            if (tv.TypeCode == (short)DxfCode.ExtendedDataRegAppName)
                            {
                                string appName = tv.Value.ToString();
                                if (appName == "MyCustomApp") // Replace with your custom app name
                                {
                                    // Extract custom property values
                                    // Example: Read diameter, material, and thickness
                                    // You can add more conditions based on your specific properties
                                    // tv.Value will contain the property value
                                    // Handle the values accordingly
                                    // ...
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
