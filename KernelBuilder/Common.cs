using KernelBuilder.Algorithms;

namespace KernelBuilder
{
    public static class Common
    {
        public static IAlgorithm parseAlgorithmName(string algorithm)
        {
            if (algorithm == "MD5")
                return new MD5();
            else if (algorithm == "SHA1")
                return new SHA1();
            else if (algorithm == "SHA224")
                return new SHA224();
            else if (algorithm == "SHA256")
                return new SHA256();

            throw new NotImplementedException("The hash function you specified is not supported!");
        }

        public enum Endianness
        {
            LittleEndian,
            BigEndian
        }
        public enum OutputFormat // TODO: support binary
        {
            Binary = 1,
            Hex = 2
        }
        public enum Conversion
        {
            LE2LE,
            LE2BE,
            BE2LE,
            BE2BE
        }
        public enum KernelType
        {
            SingleHash,
            MultiHash
        }
        public enum AttackVector
        {
            a0,
            a1,
            a3 
        }
    }

    public class CodeList : List<string>
    {
        public int spacing = 0;

        public new void Add(string element)
        {
            foreach (string item in element.Split("\n"))
            {
                base.Add($"{string.Concat(Enumerable.Repeat(' ', spacing * 2))}{item.Trim()}"); // call the original Add
            }
        }

        public void Add(string element, int spacing)
        {
            foreach (string item in element.Split("\n"))
            {
                base.Add($"{string.Concat(Enumerable.Repeat(' ', spacing * 2))}{item.Trim()}"); // call the original Add
            }

            this.spacing = spacing;
        }
    }


}
