namespace Huzurevi.Application.Features.Anasayfa;

public record AnasayfaIstatistikDto(
    int ToplamSakin,
    int ToplamOda,
    int ToplamYatak,
    int DoluYatak,
    int BosYatak,
    int DolulukOrani,
    int IceridekiZiyaretci,
    List<SonSakinDto> SonSakinler);

public record SonSakinDto(int Id, string Ad, string Soyad, string TcKimlikNo, string Durum);
