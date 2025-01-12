namespace SharedLib.Tokens.Core.Jwa;

public class EncryptionAlgorithmKey(string alg)
{
    public const string Aes128KW = "A128KW";
    public const string Aes256KW = "A256KW";
    public const string RsaPKCS1 = "RSA1_5";
    public const string RsaOAEP = "RSA-OAEP";

    public string Alg { get; } = alg;


    public static implicit operator string(EncryptionAlgorithmKey value) => value.Alg;
    public static implicit operator EncryptionAlgorithmKey(string value) => new(value);
}
