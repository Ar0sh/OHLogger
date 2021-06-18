using System.Windows.Media;

namespace OHDataLogger.Methods
{
    public class SensorValues
    {
        private readonly string link;
        private string state;
        private readonly string pattern;
        private readonly bool editable;
        private readonly string name;
        private readonly string type;
        private readonly string label;

        public SensorValues(string link, string state, string pattern, bool editable, string type, string name, string label)
        {
            this.link = link;
            this.state = state;
            this.pattern = pattern;
            this.editable = editable;
            this.type = type;
            this.name = name;
            this.label = label;
        }

        /** Returns the value used to delimit timestamps. */
        public string GetLink()
        {
            return link;
        }

        public string GetState()
        {
            return state;
        }

        public void SetState(string updState)
        {
            state = updState;
        }

        public string GetPattern()
        {
            return pattern;
        }
        public bool GetEditable()
        {
            return editable;
        }

        public string GetName()
        {
            return name;
        }

        public string GetItemType()
        {
            return type;
        }

        public string GetLabel()
        {
            return label;
        }
    }
}