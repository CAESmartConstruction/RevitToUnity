using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;


namespace RoBIM
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class PanelToJson : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
            ICollection<Reference> reference_collector;
            UIDocument uidoc;
            uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Transaction trans = new Transaction(doc);
            trans.Start("test");
            reference_collector = uidoc.Selection.PickObjects(ObjectType.Element);

            Elements elementsJson = new Elements();
            elementsJson.StructuralFramingList = new List<OneElement>();
            elementsJson.InsulationList = new List<OneElement>();
            elementsJson.ScrewList = new List<OneElement>();
            List<OneElement> screwList=new List<OneElement>();
            foreach (Reference reference in reference_collector)
            {
                Element targetElement = doc.GetElement(reference);
                int categoryId = targetElement.Category.Id.IntegerValue;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                if (categoryId == (int)BuiltInCategory.OST_StructuralFraming)
                {
                    if(targetElement.Name == "#6_Screw")
                    {
                        if (targetElement.get_Parameter(BuiltInParameter.IS_VISIBLE_PARAM).AsInteger() == 1)
                        {
                            OneElement oneElement = UtilityJson.getJsonFromScrew(targetElement);
                            elementsJson.ElementList.Add(oneElement);
                        }
                        
                    }
                    else
                    {
                        if (targetElement.get_Parameter(BuiltInParameter.IS_VISIBLE_PARAM).AsInteger() == 1)
                        {
                            OneElement oneElement = UtilityJson.getJsonFromStructuralFraming(doc, targetElement);
                            elementsJson.StructuralFramingList.Add(oneElement);
                        }
                            
                    }

                }
                else if (categoryId == (int)BuiltInCategory.OST_GenericModel)
                {
                    List<OneElement> oneElementList = UtilityJson.getJsonFromInsulationArray(doc,targetElement);
                    elementsJson.InsulationList.AddRange(oneElementList);
                }
            }

            trans.Commit();
            string directory = Directory.GetCurrentDirectory();//會是模型的路徑
            if (directory != null)
            {
                directory = Directory.GetParent(directory).ToString();
                directory = Directory.GetParent(directory).ToString();
                directory = Directory.GetParent(directory).ToString();
            }
            //directory 相對路徑
            directory = String.Format(@"C:\Users\nick0\RoBIM-1\Result_File\panel_{0}.txt", DateTime.Now.ToLongDateString());
            String TimeStamp = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "_");
            String filename = String.Format(@"panel_{0}.txt", TimeStamp);
            
            //directory = String.Format(@"C:\Users\ian89\source\repos\RoBIMtoJson");
            //directory = @directory + @"\Result_File\"+ filename;

       
            
            MessageBox.Show("file_place:"+ directory);
            string json = JsonConvert.SerializeObject(elementsJson, Formatting.Indented);
            File.WriteAllText(@directory, json);

            return Result.Succeeded;
        }
        public static bool steelComponentStructuralLocationEqual(SteelComponent steelComponentA, SteelComponent steelComponentB)
        {
            if (steelComponentA.structuralLocation.EndPoint.IsAlmostEqualTo(steelComponentB.structuralLocation.EndPoint) &&
                steelComponentA.structuralLocation.StartPoint.IsAlmostEqualTo(steelComponentB.structuralLocation.StartPoint))
                return true;
            else
                return false;
        }

    }
    public class InstanceTransform
    {
        public XYZ BasisX
        {
            get;
            set;
        }
        public XYZ BasisY
        {
            get;
            set;
        }
        public XYZ BasisZ
        {
            get;
            set;
        }
        public XYZ Origin
        {
            get;
            set;
        }
    }
    
    public class StructuralLocation
    {
        public XYZ StartPoint
        {
            get;
            set;
        }
        public XYZ EndPoint
        {
            get;
            set;
        }
    }
    public class ScrewLocation
    {
        public XYZ ScrewPoint
        {
            get;
            set;
        }
    }
    public interface OneElement {

        ProductionReference productionReference { get; set; }
        String ElementType { get; set; }

    }

    
    public enum ProductionMethod
    {
        Gripper,
        VacuumGripper,
        Screw,
        None,
        
    }
    public class ProductionReference
    {
        public XYZ Position { get; set; }
        public XYZ Direction  { get; set; }
        public String ProductionMethod { get; set; }
    }
    public class ScrewComponent: OneElement
    {
        public String ElementType
        {
            get;
            set;
        }
        public String ElementName
        {
            get;
            set;
        }
        public ScrewLocation screwLocation
        {
            get;
            set;
        }
        public XYZ screwDirection
        {
            get;
            set;
        }
        public double screwLength_in_mm
        {
            get;
            set;
        }
        public ProductionReference productionReference { get; set; }
    }

    
    
    
   

}
