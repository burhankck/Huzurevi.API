namespace Huzurevi.Application.Common.Interfaces;

public interface ISifreHasher
{
    string Hashle(string sifre);
    bool Dogrula(string sifre, string hash);
}
