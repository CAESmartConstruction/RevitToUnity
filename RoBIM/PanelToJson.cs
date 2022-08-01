﻿using System;
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
            elementsJson.ElementList = new List<OneElement>();

            foreach (Reference reference in reference_collector)
            {
                Element targetElement = doc.GetElement(reference);
                int categoryId = targetElement.Category.Id.IntegerValue;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                if (categoryId == (int)BuiltInCategory.OST_StructuralFraming)
                {
                    if(targetElement.Name == "#6_Screw")
                    {
                        OneElement oneElement = UtilityJson.getJsonFromScrew(targetElement);
                        elementsJson.ElementList.Add(oneElement);
                    }
                    else
                    {
                        OneElement oneElement = UtilityJson.getJsonFromStructuralFraming(doc,targetElement);
                        elementsJson.ElementList.Add(oneElement);
                    }

                }
                else if (categoryId == (int)BuiltInCategory.OST_GenericModel)
                {
                    OneElement oneElement = UtilityJson.getJsonFromInsulationArray(doc,targetElement);
                    elementsJson.ElementList.Add(oneElement);
                }
            }

            //String directory = String.Format(@"C:\Users\nick0\RoBIM-1\Result_File\panel_{0}.txt", DateTime.Now.ToLongDateString());
            trans.Commit();
            string directory = Directory.GetCurrentDirectory();
            if (directory != null)
            {
                directory = Directory.GetParent(directory).ToString();
                directory = Directory.GetParent(directory).ToString();
                directory = Directory.GetParent(directory).ToString();
            }

            String TimeStamp = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", "_");
            String filename = String.Format(@"panel_{0}.txt", TimeStamp);
            //directory 相對路徑
            directory = @directory + @"\1\"+ filename;
           
           
            
            MessageBox.Show("file_place:"+ directory);
            string json = JsonConvert.SerializeObject(elementsJson, Formatting.Indented);
            File.WriteAllText(@directory, json);

            return Result.Succeeded;
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
    public interface  OneElement{
        
    
    }

    public class SteelComponet:OneElement
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
        public StructuralLocation structuralLocation
        {
            get;
            set;
        }
        public IList<XYZ> SectionOnStart
        {
            get;
            set;
        }
        public double CrossSectionRotation
        {
            get;
            set;
        }
        public  InstanceTransform instanceTransform
        {
            get;
            set;
        }


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
    }

    public class InsulationLocation
    {
        public XYZ StartPoint
        {
            get;
            set;
        }
       
    }
    public class InsulationSize
    {
        public double Height
        {
            get;
            set;
        }
        public double Width
        {
            get;
            set;
        }
        public double Thick
        {
            get;
            set;
        }

    }
    public class InsulationNumbers
    {
        public int HNumber
        {
            get;
            set;
        }
        public int VNumber
        {
            get;
            set;
        }
      

    }
    public class InsulationRemaining
    {
        public double HRemaing
        {
            get;
            set;
        }
        public double VRemaing
        {
            get;
            set;
        }


    }

    public class Insulation : OneElement
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
        public InsulationLocation insulationLocation
        {
            get;
            set;
        }
        public InsulationNumbers insulationNumbers
        {
            get;
            set;
        }
        public InsulationSize insulationSize
        {
            get;
            set;
        }
        public string Material
        {
            get;
            set;
        }
        public InsulationRemaining insulationRemaining
        {
            get;
            set;
        }


    }
    public class Elements
    {
        public List<OneElement> ElementList
        {
            get;
            set;
        }
    }
    
   

}
