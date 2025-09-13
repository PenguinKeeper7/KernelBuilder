using KernelBuilder.Algorithms;

namespace KernelBuilder
{
    public class SHA224 : IAlgorithm
    {
        // Basic info
        private int _byteLength = 28;
        public int byteLength { get => _byteLength; set => _byteLength = value; }
        public int length => byteLength * (int)outputFormat;
        public Common.Endianness endianness => Common.Endianness.BigEndian;
        public Common.OutputFormat outputFormat => Common.OutputFormat.Hex;
        public string context { get; set; }

        // Module info
        public string opts_types => "OPTS_TYPE_PT_GENERATE_BE | OPTS_TYPE_ST_ADD80 | OPTS_TYPE_ST_ADDBITS15";
        public string initialValues => "SHA224M_";

        // Function names
        public string contextType => "sha224_ctx_t";
        public string initFunction => "sha224_init";
        public string updateFunction => "sha224_update";
        public string updateGlobalFunction => "sha224_update_global";
        public string updateSwapFunction => "sha224_update_swap";
        public string updateVectorSwapFunction => "sha224_update_vector_swap";
        public string updateGlobalSwapFunction => "sha224_update_global_swap";
        public string update64Function => "sha224_update_64";
        public string update64VectorFunction => "sha224_update_vector_64";
        public string finalFunction => "sha224_final";
    }
}
