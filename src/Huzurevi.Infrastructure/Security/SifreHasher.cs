using Huzurevi.Application.Common.Interfaces;

namespace Huzurevi.Infrastructure.Security;

public class SifreHasher : ISifreHasher
{
    public string Hashle(string sifre) => BCrypt.Net.BCrypt.HashPassword(sifre);

    public bool Dogrula(string sifre, string hash) => BCrypt.Net.BCrypt.Verify(sifre, hash);
}
