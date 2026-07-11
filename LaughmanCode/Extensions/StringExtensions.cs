namespace Laughman.LaughmanCode.Extensions;

public static class StringExtensions
{
    // Godot 资源路径必须用正斜杠；Path.Join 在 Windows 会产生反斜杠，需统一替换。
    private static string ResPath(params string[] parts)
    {
        return string.Join("/", parts);
    }

    public static string ImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "card_portraits", path);
    }
    public static string BigCardImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "powers", "big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "relics", "big", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "charui", path);
    }
}
