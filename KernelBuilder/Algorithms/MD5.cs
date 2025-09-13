using KernelBuilder.Algorithms;

namespace KernelBuilder
{
    public class MD5 : IAlgorithm
    {
        // Basic info
        private int _byteLength = 16;
        public int byteLength { get => _byteLength; set => _byteLength = value; } 
        public int length => byteLength * (int)outputFormat;
        public Common.Endianness endianness => Common.Endianness.LittleEndian;
        public Common.OutputFormat outputFormat => Common.OutputFormat.Hex;
        public string context { get; set; }

        // Module info
        public string opts_types => "OPTS_TYPE_PT_GENERATE_LE | OPTS_TYPE_ST_ADD80 | OPTS_TYPE_ST_ADDBITS14";
        public string initialValues => "MD5M_";

        // Function names
        public string contextType => "md5_ctx_t";
        public string initFunction => "md5_init";
        public string updateFunction => "md5_update";
        public string updateGlobalFunction => "md5_update_global";
        public string updateSwapFunction => "md5_update_swap";
        public string updateVectorSwapFunction => "md5_update_vector_swap";
        public string updateGlobalSwapFunction => "md5_update_global_swap";
        public string update64Function => "md5_update_64";
        public string update64VectorFunction => "md5_update_vector_64";
        public string finalFunction => "md5_final";
    }
}
