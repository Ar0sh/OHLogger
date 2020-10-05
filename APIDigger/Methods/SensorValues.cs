using System.Windows.Media;

namespace APIDigger.Methods
{
    public class SensorValues
    {
        private readonly string link;
        private string state;
        private readonly string pattern;
        private readonly bool? readOnly;
        private readonly string options;
        private readonly string name;
        private Brushes color;

        public SensorValues(string link, string state, string pattern, bool? readOnly, string options, string name, Brushes color)
        {
            this.link = link;
            this.state = state;
            this.pattern = pattern;
            this.readOnly = readOnly;
            this.options = options;
            this.name = name;
            this.color = color;
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
            this.state = updState;
        }

        public string GetPattern()
        {
            return pattern;
        }
        public bool? GetReadonly()
        {
            return readOnly;
        }

        public string GetOptions()
        {
            return options;
        }

        public string GetName()
        {
            return name;
        }

        public Brushes GetColor()
        {
            return color;
        }

        public void SetColor(Brushes updColor)
        {
            color = updColor;
        }
    }
}