using Huzurevi.Domain.Entities;

namespace Huzurevi.Application.Common.Interfaces;

public interface ITokenUretici
{
    (string Token, DateTime GecerlilikBitis) TokenUret(Kullanici kullanici, int oturumDakika, int? kurulusId, string? rol);
}
