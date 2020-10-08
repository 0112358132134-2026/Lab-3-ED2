namespace API_Huffman.Models
{
    public class HuffCompressions
    {
        public string originalName { get; set; }
        public string compressedFilePath { get; set; }
        public double compressionRatio { get; set; }
        public double compressionFactor { get; set; }
        public double reductionPorcentage { get; set; }
    }
}
