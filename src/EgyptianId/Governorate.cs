namespace EgyptianId;

/// <summary>Governorate of birth registration (digits 8 and 9 of the national ID).</summary>
public enum Governorate : byte
{
    /// <summary>Cairo (code 01).</summary>
    Cairo = 1,
    /// <summary>Alexandria (code 02).</summary>
    Alexandria = 2,
    /// <summary>Port Said (code 03).</summary>
    PortSaid = 3,
    /// <summary>Suez (code 04).</summary>
    Suez = 4,
    /// <summary>Damietta (code 11).</summary>
    Damietta = 11,
    /// <summary>Dakahlia (code 12).</summary>
    Dakahlia = 12,
    /// <summary>Sharqia (code 13).</summary>
    Sharqia = 13,
    /// <summary>Qalyubia (code 14).</summary>
    Qalyubia = 14,
    /// <summary>Kafr El Sheikh (code 15).</summary>
    KafrElSheikh = 15,
    /// <summary>Gharbia (code 16).</summary>
    Gharbia = 16,
    /// <summary>Monufia (code 17).</summary>
    Monufia = 17,
    /// <summary>Beheira (code 18).</summary>
    Beheira = 18,
    /// <summary>Ismailia (code 19).</summary>
    Ismailia = 19,
    /// <summary>Giza (code 21).</summary>
    Giza = 21,
    /// <summary>Beni Suef (code 22).</summary>
    BeniSuef = 22,
    /// <summary>Faiyum (code 23).</summary>
    Faiyum = 23,
    /// <summary>Minya (code 24).</summary>
    Minya = 24,
    /// <summary>Asyut (code 25).</summary>
    Asyut = 25,
    /// <summary>Sohag (code 26).</summary>
    Sohag = 26,
    /// <summary>Qena (code 27).</summary>
    Qena = 27,
    /// <summary>Aswan (code 28).</summary>
    Aswan = 28,
    /// <summary>Luxor (code 29).</summary>
    Luxor = 29,
    /// <summary>Red Sea (code 31).</summary>
    RedSea = 31,
    /// <summary>New Valley (code 32).</summary>
    NewValley = 32,
    /// <summary>Matruh (code 33).</summary>
    Matruh = 33,
    /// <summary>North Sinai (code 34).</summary>
    NorthSinai = 34,
    /// <summary>South Sinai (code 35).</summary>
    SouthSinai = 35,
    /// <summary>outside Egypt (birthplace abroad) (code 88).</summary>
    OutsideRepublic = 88
}

/// <summary>Display helpers for <see cref="Governorate"/>.</summary>
public static class GovernorateExtensions
{
    /// <summary>Returns the English display name, for example "Kafr El Sheikh".</summary>
    public static string EnglishName(this Governorate governorate) => governorate switch
    {
        Governorate.OutsideRepublic => "Outside Egypt",
        _ => System.Text.RegularExpressions.Regex.Replace(governorate.ToString(), "(?<=[a-z])(?=[A-Z])", " ")
    };

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
