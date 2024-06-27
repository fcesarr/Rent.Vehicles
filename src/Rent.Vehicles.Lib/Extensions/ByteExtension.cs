using System.Security.Cryptography;
using System.Text;

namespace Rent.Vehicles.Lib.Extensions;

public static class ByteExtension
{
    public static string ByteToSha256String(this byte[] bytes)
    {
        bytes = SHA256.HashData(bytes);

        return ToString(bytes);
    }

    public static string ByteToMD5String(this byte[] bytes)
    {
        bytes = MD5.HashData(bytes);

        return ToString(bytes);
    }

    private static string ToString(this byte[] bytes)
    {
        StringBuilder stringBuilder = new();
        foreach (var @byte in bytes)
        {
            stringBuilder.Append(@byte.ToString("x2"));
        }

        return stringBuilder.ToString();
    }
}
