using KernelBuilder.Algorithms;

namespace KernelBuilder
{
    public class SHA256 : IAlgorithm
    {
        // Basic info
        private int _byteLength = 32;
        public int byteLength { get => _byteLength; set => _byteLength = value; }
        public int length => byteLength * (int)outputFormat;
        public Common.Endianness endianness => Common.Endianness.BigEndian;
        public Common.OutputFormat outputFormat => Common.OutputFormat.Hex;
        public string context { get; set; }

        // Module info
        public string opts_types => "OPTS_TYPE_PT_GENERATE_BE | OPTS_TYPE_ST_ADD80 | OPTS_TYPE_ST_ADDBITS15";
        public string initialValues => "SHA256M_";

        // Function names
        public string contextType => "sha256_ctx_t";
        public string initFunction => "sha256_init";
        public string updateFunction => "sha256_update";
        public string updateGlobalFunction => "sha256_update_global";
        public string updateSwapFunction => "sha256_update_swap";
        public string updateVectorSwapFunction => "sha256_update_vector_swap";
        public string updateGlobalSwapFunction => "sha256_update_global_swap";
        public string update64Function => "sha256_update_64";
        public string update64VectorFunction => "sha256_update_vector_64";
        public string finalFunction => "sha256_final";
    }
}
