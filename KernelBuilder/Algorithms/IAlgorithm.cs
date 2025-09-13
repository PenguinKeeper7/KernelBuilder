namespace KernelBuilder.Algorithms
{
    public interface IAlgorithm
    {
        // Basic info
        public int byteLength { get; set; }
        public int length => byteLength * (int)outputFormat;
        Common.Endianness endianness { get; }
        Common.OutputFormat outputFormat { get; }
        public string context { get; set; }

        // Module info
        public string opts_types { get; }
        public string initialValues { get;  } // ex. SHA1M_

        // Function names
        // TODO: maybe get rid of these using intialValues
        public string contextType { get; }
        public string initFunction { get; }
        public string updateFunction { get; }
        public string updateGlobalFunction { get; }
        public string updateSwapFunction { get; }
        public string updateVectorSwapFunction { get;  }
        public string updateGlobalSwapFunction { get; }
        public string update64Function { get; }
        public string update64VectorFunction { get; }
        public string finalFunction { get; }
    }
}
