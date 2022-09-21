using System.Runtime.Serialization;
using System.Xml;

namespace SW2URDF.URDF
{
    //The base URDF element, a robot
    [DataContract(IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/SW2URDF")]
    public class XacroMacro : URDFElement
    {
        public Link BaseLink { get; private set; }

        [DataMember]
        private readonly URDFAttribute NameAttribute;

        public string Name
        {
            get => (string)NameAttribute.Value;
            set => NameAttribute.Value = value;
        }

        public XacroMacro() : base("macro", true, "xacro")
        {
            BaseLink = new Link(null);
            NameAttribute = new URDFAttribute("name", true, "");
            URDFAttribute ParamAttribute = new URDFAttribute("params", true, "prefix");

            ChildElements.Add(BaseLink);
            Attributes.Add(NameAttribute);
            Attributes.Add(ParamAttribute);
        }

        public void SetBaseLink(Link link)
        {
            BaseLink = link;
            ChildElements.Clear();
            ChildElements.Add(link);
        }
        
        public string[] GetJointNames(bool includeFixed)
        {
            return BaseLink.GetJointNames(includeFixed);
        }
    }
    
    public class Robot : URDFElement
    {
        [DataMember]
        public Link BaseLink { get; private set; }
        public XacroMacro Macro { get; private set; }

        [DataMember]
        private readonly URDFAttribute NameAttribute;

        private readonly string Ns = "http://www.ros.org/wiki/xacro";

        public string Name
        {
            get => (string)NameAttribute.Value;
            set => NameAttribute.Value = value;
        }

        public Robot() : base("robot", true)
        {
            NameAttribute = new URDFAttribute("name", true, "");
            URDFAttribute NameSpaceAttribute = new URDFAttribute("xmlns", true, Ns, "xacro");

            Macro = new XacroMacro();

            ChildElements.Add(Macro);
            Attributes.Add(NameAttribute);
            Attributes.Add(NameSpaceAttribute);
        }

        public override void WriteURDF(XmlWriter writer)
        {
            writer.WriteStartDocument();
            string buildVersion = Versioning.Version.GetBuildVersion();
            string commitVersion = Versioning.Version.GetCommitVersion();

            writer.WriteComment(" This URDF was automatically created by SolidWorks to URDF Exporter! " +
                "Originally created by Stephen Brawner (brawner@gmail.com) \r\n" +
                string.Format("     Commit Version: {0}  Build Version: {1}\r\n", commitVersion, buildVersion) +
                "     For more information, please see http://wiki.ros.org/sw_urdf_exporter ");

            base.WriteURDF(writer);

            writer.WriteEndDocument();
            writer.Close();
        }

        public void SetBaseLink(Link link)
        {
            // TODO: Not the best place, need a better solution
            Macro.Name = Name;
            Macro.SetBaseLink(link);
        }

        internal string[] GetJointNames(bool includeFixed)
        {
            return Macro.GetJointNames(includeFixed);
        }
    }
}