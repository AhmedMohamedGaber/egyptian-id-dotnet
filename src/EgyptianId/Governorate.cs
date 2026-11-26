namespace EgyptianId;

/// <summary>Governorate of birth registration (digits 8 and 9 of the national ID).</summary>
public enum Governorate : byte
{
    Cairo = 1,
    Alexandria = 2,
    PortSaid = 3,
    Suez = 4,
    Damietta = 11,
    Dakahlia = 12,
    Sharqia = 13,
    Qalyubia = 14,
    KafrElSheikh = 15,
    Gharbia = 16,
    Monufia = 17,
    Beheira = 18,
    Ismailia = 19,
    Giza = 21,
    BeniSuef = 22,
    Faiyum = 23,
    Minya = 24,
    Asyut = 25,
    Sohag = 26,
    Qena = 27,
    Aswan = 28,
    Luxor = 29,
    RedSea = 31,
    NewValley = 32,
    Matruh = 33,
    NorthSinai = 34,
    SouthSinai = 35,
    OutsideRepublic = 88
}

/// <summary>Display helpers for <see cref="Governorate"/>.</summary>
public static class GovernorateExtensions
{
    /// <summary>Returns the Arabic name of the governorate.</summary>
    public static string ArabicName(this Governorate governorate) => governorate switch
    {
        Governorate.Cairo => "القاهرة",
        Governorate.Alexandria => "الإسكندرية",
        Governorate.PortSaid => "بورسعيد",
        Governorate.Suez => "السويس",
        Governorate.Damietta => "دمياط",
        Governorate.Dakahlia => "الدقهلية",
        Governorate.Sharqia => "الشرقية",
        Governorate.Qalyubia => "القليوبية",
        Governorate.KafrElSheikh => "كفر الشيخ",
        Governorate.Gharbia => "الغربية",
        Governorate.Monufia => "المنوفية",
        Governorate.Beheira => "البحيرة",
        Governorate.Ismailia => "الإسماعيلية",
        Governorate.Giza => "الجيزة",
        Governorate.BeniSuef => "بني سويف",
        Governorate.Faiyum => "الفيوم",
        Governorate.Minya => "المنيا",
        Governorate.Asyut => "أسيوط",
        Governorate.Sohag => "سوهاج",
        Governorate.Qena => "قنا",
        Governorate.Aswan => "أسوان",
        Governorate.Luxor => "الأقصر",
        Governorate.RedSea => "البحر الأحمر",
        Governorate.NewValley => "الوادي الجديد",
        Governorate.Matruh => "مطروح",
        Governorate.NorthSinai => "شمال سيناء",
        Governorate.SouthSinai => "جنوب سيناء",
        Governorate.OutsideRepublic => "خارج الجمهورية",
        _ => governorate.ToString()
    };
}
