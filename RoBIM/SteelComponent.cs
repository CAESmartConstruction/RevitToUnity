using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace RoBIM
{
    public class SteelComponent : OneElement
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
        public InstanceTransform instanceTransform
        {
            get;
            set;
        }
        public ProductionReference productionReference { get; set; }

     }
    }
