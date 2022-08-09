using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace RoBIM
{
    public class InsulationLocation
    {
        public XYZ LocationPoint
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
        
        public ProductionReference productionReference { get; set; }
    }
}
