using System.Diagnostics;

namespace SharedLib.Tokens.Core.Jwa;

[DebuggerDisplay("{Enc}")]
public class EncryptionAlgorithmContent(string enc)
{
    public const string Aes128CbcHmacSha256 = "A128CBC-HS256";
    public const string Aes192CbcHmacSha384 = "A192CBC-HS384";
    public const string Aes256CbcHmacSha512 = "A256CBC-HS512";
    public const string Aes128Gcm = "A128GCM";
    public const string Aes192Gcm = "A192GCM";
    public const string Aes256Gcm = "A256GCM";
    public string Enc { get; } = enc;

    public static implicit operator string(EncryptionAlgorithmContent value) => value.Enc;
    public static implicit operator EncryptionAlgorithmContent(string value) => new(value);
}
