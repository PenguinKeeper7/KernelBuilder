using CommandLine;
using System;
using System.Collections.Generic;
using static KernelBuilder.Common;

namespace KernelBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string algorithm = "";
            string ID = "";
            bool overwrite = false;
            bool hashcat = false;

            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(opts =>
            {
                algorithm = opts.Algorithm;
                ID = opts.ID;
                overwrite = opts.Overwrite;
                hashcat = opts.Hashcat;
            }).WithNotParsed(errors =>
            {
                // CommandLineParser already prints its own --help text on error, so do nothing
                Environment.Exit(1);
            });

            if(int.TryParse(ID, out _) == false || ID.Length > 5 || ID.Length == 0)
            {
                Console.WriteLine($"The ID \"{ID}\" must be a 1-5 digit number. ex: 98000");
                Environment.Exit(0);
            }

            ID = ID.PadLeft(5, '0');

            List<string> instructions = Interpreter.GenerateInstructions(algorithm);

            CreateFolderStructure(ID, overwrite, hashcat);

            GenerateKernels(instructions, ID, hashcat);
            GenerateModule(algorithm, instructions, ID, hashcat);

            if (hashcat)
            {
                Console.WriteLine($"Plugin has been stored in the Hashcat folders");
                Console.WriteLine("");
                Console.WriteLine($"Compile Hashcat using: https://github.com/hashcat/hashcat/blob/master/BUILD.md");
            }
            else
            {
                Console.WriteLine($"Plugin has been stored in plugins/{ID}");
                Console.WriteLine("");
                Console.WriteLine($"1) Copy and paste the 2 folders inside plugins/{ID} into Hashcat");
                Console.WriteLine($"2) Compile Hashcat using: https://github.com/hashcat/hashcat/blob/master/BUILD.md");
            }
        }

        public static void CreateFolderStructure(string ID, bool overwrite, bool hashcat)
        {
            if (Directory.Exists($"plugins/{ID}") && overwrite == false && hashcat == false)
            {
                Console.Error.WriteLine($"{ID} has already been used!");
                Environment.Exit(0);
            }

            string prefix = "";

            if (hashcat == false)
            {
                prefix = $"plugins/{ID}/";
                Directory.CreateDirectory($"{prefix}");
            }

            Directory.CreateDirectory($"{prefix}src/modules");
            Directory.CreateDirectory($"{prefix}OpenCL");
        }

        public static void GenerateKernels(List<string> instructions, string ID, bool hashcat)
        {
            string prefix = "";

            if (hashcat == false)
                prefix = $"plugins/{ID}/";

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

                File.WriteAllLines($"{prefix}OpenCL/m{ID}_{attackVector}-pure.cl", kernelCode.ToArray());
            }
        }

        public static void GenerateModule(string algorithm, List<string> instructions, string ID, bool hashcat)
        {
            string prefix = "";

            if (hashcat == false)
                prefix = $"plugins/{ID}/";

            CodeList moduleCode = new CodeList();

            moduleCode.AddRange(ModuleCodeGenerator.GenerateModule(algorithm, instructions, ID));

            File.WriteAllLines($"{prefix}src/modules/module_{ID}.c", moduleCode.ToArray());
        }
    }
}
