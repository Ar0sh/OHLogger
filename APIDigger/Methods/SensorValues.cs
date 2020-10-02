namespace APIDigger.Methods
{
    public class SensorValues
    {
        private readonly string link;
        private readonly string state;
        private readonly string pattern;
        private readonly bool? readOnly;
        private readonly string options;

        public SensorValues(string link, string state, string pattern, bool? readOnly, string options)
        {
            this.link = link;
            this.state = state;
            this.pattern = pattern;
            this.readOnly = readOnly;
            this.options = options;
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
    }
}