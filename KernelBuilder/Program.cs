using static KernelBuilder.Common;

namespace KernelBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2 || args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("Usage: KernelBuilder <algorithm> <ID>");
                Environment.Exit(0);
            }

            string algorithm = args[0];
            string ID = args[1];

            if(int.TryParse(ID, out _) == false || ID.Length > 5 || ID.Length == 0)
            {
                Console.WriteLine($"The ID \"{ID}\" must be a 1-5 digit number. ex: 98000");
                Environment.Exit(0);
            }

            ID = ID.PadLeft(5, '0');

            List<string> instructions = Interpreter.GenerateInstructions(algorithm);

            CreateFolderStructure(ID);

            GenerateKernels(instructions, ID);
            GenerateModule(algorithm, instructions, ID);

            Console.WriteLine($"Plugin has been stored in plugins/{ID}");
            Console.WriteLine("");
            Console.WriteLine($"1) Copy and paste the 2 folders inside plugins/{ID} into Hashcat");
            Console.WriteLine($"2) Compile Hashcat using: https://github.com/hashcat/hashcat/blob/master/BUILD.md");
        }

        public static void CreateFolderStructure(string ID)
        {
            // TODO: Add an overwrite argument to stop this bailing out
            if (Directory.Exists($"plugins/{ID}"))
            {
                Console.Error.WriteLine($"{ID} has already been used!");
                Environment.Exit(0);
            }
            
            Directory.CreateDirectory($"plugins/{ID}");
            Directory.CreateDirectory($"plugins/{ID}/src/modules");
            Directory.CreateDirectory($"plugins/{ID}/OpenCL");
        }

        public static void GenerateKernels(List<string> instructions, string ID)
        {
            AttackVector[] attackVectors = new AttackVector[] { AttackVector.a0, AttackVector.a1, AttackVector.a3 };
            foreach (AttackVector attackVector in attackVectors )
            {
                CodeList kernelCode = new CodeList();

                kernelCode.AddRange(KernelCodeGenerator.GenerateImports(attackVector));

                kernelCode.AddRange(KernelCodeGenerator.GenerateHeader(instructions, KernelType.MultiHash, attackVector, ID));
                kernelCode.AddRange(KernelCodeGenerator.GenerateCompute(instructions, attackVector));
                kernelCode.AddRange(KernelCodeGenerator.GenerateFooter(instructions.Last(), attackVector, KernelType.MultiHash));

                kernelCode.AddRange(KernelCodeGenerator.GenerateHeader(instructions, KernelType.SingleHash, attackVector, ID));
                kernelCode.AddRange(KernelCodeGenerator.GenerateCompute(instructions, attackVector));
                kernelCode.AddRange(KernelCodeGenerator.GenerateFooter(instructions.Last(), attackVector, KernelType.SingleHash));

                File.WriteAllLines($"plugins/{ID}/OpenCL/m{ID}_{attackVector}-pure.cl", kernelCode.ToArray());
            }
        }

        public static void GenerateModule(string algorithm, List<string> instructions, string ID)
        {
            CodeList moduleCode = new CodeList();

            moduleCode.AddRange(ModuleCodeGenerator.GenerateModule(algorithm, instructions, ID));

            File.WriteAllLines($"plugins/{ID}/src/modules/module_{ID}.c", moduleCode.ToArray());
        }
    }
}
