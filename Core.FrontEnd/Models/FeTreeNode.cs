using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeTreeNode
    {
        public FeTreeNode()
        {
           
        }
        public string text;
        public string id;
        public string parent;
        public string icon = "fa fa-folder fa-large";
        public string type = "default";
        public State state = new State();
        public LinkAttribute a_attr = new LinkAttribute();
        public LiAttribute li_attr = new LiAttribute();
        public bool isSinglePage;
        public bool showInFrontEnd;

        public short ctype;

        public List<FeTreeNode> children { get { return new List<FeTreeNode>();} }

        public class State
        {
            public bool opened;
            public bool disabled;
            public bool selected;
            public bool @checked ;
        }

        public class LinkAttribute
        {
            public string href = "#";
        }

        public class LiAttribute
        {

        }
    }
}