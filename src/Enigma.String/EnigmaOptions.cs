namespace Enigma.String
{
    public class EnigmaOptions
    {
        public EnigmaOptions()
        {

        }

        public EnigmaOptions(string symmetricKey)
        {
            SymmetricKey = symmetricKey;
        }

        public string SymmetricKey { get; set; } = null!;
    }
}