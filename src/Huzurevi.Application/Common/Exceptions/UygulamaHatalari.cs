namespace Huzurevi.Application.Common.Exceptions;

public class KayitBulunamadiHatasi(string mesaj) : Exception(mesaj);

public class CakismaHatasi(string mesaj) : Exception(mesaj);

public class GecersizIstekHatasi(string mesaj) : Exception(mesaj);

public class YetkisizHatasi(string mesaj) : Exception(mesaj);

public class HesapKilitliHatasi(string mesaj) : Exception(mesaj);

public class BakimModuHatasi(string mesaj) : Exception(mesaj);
