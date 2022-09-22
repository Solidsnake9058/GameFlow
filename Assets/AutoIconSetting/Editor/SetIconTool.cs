using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

[InitializeOnLoad]
public class SetIconTool : EditorWindow
{

    private static readonly string m_AssetPath = "Assets/AutoIconSetting/IconPath.asset";
    private static readonly string m_DefualtIOSPath = "Assets/Icon/iOS/AppIcon.appiconset";
    private static readonly string m_DefualtAndroidPath = "Assets/Icon/Android";

    private static IconPath m_Asset;

#if UNITY_2018_1_OR_NEWER
    private static readonly string[] m_IconPathListIOS =
    {
        "Icon-App-60x60@3x.png",
        "Icon-App-60x60@2x.png",
        "Icon-App-83.5x83.5@2x.png",
        "Icon-App-76x76@2x.png",
        "Icon-App-76x76@1x.png",
        "Icon-App-40x40@3x.png",
        "Icon-App-40x40@2x.png",
        "Icon-App-40x40@2x.png",
        "Icon-App-40x40@1x.png",
        "Icon-App-29x29@3x.png",
        "Icon-App-29x29@2x.png",
        "Icon-App-29x29@1x.png",
        "Icon-App-29x29@2x.png",
        "Icon-App-29x29@1x.png",
        "Icon-App-20x20@3x.png",
        "Icon-App-20x20@2x.png",
        "Icon-App-20x20@2x.png",
        "Icon-App-20x20@1x.png",
        "ItunesArtwork@2x.png"
    };
#else
    private static readonly string[] m_IconPathListIOS =
    {
        "Icon-App-60x60@3x.png",
        "Icon-App-83.5x83.5@2x.png",
        "Icon-App-76x76@2x.png",
        "",
        "Icon-App-60x60@2x.png",
        "",
        "Icon-App-76x76@1x.png",
        "",
        "",
        "Icon-App-40x40@3x.png",
        "Icon-App-40x40@2x.png",
        "Icon-App-40x40@1x.png",
        "Icon-App-29x29@3x.png",
        "Icon-App-29x29@2x.png",
        "Icon-App-29x29@1x.png",
        "Icon-App-20x20@3x.png",
        "Icon-App-20x20@2x.png",
        "Icon-App-20x20@1x.png",
        "ItunesArtwork@2x.png"
    };
#endif

    private static readonly string[] m_IconPathListAndroid =
    {
        "mipmap-xxxhdpi",
        "mipmap-xxhdpi",
        "mipmap-xhdpi",
        "mipmap-hdpi",
        "mipmap-mdpi",
        "mipmap-ldpi"
    };

    private static readonly string[] m_IconFileListAndroidLegecy =
    {
       "ic_launcher.png",
    };

    private static readonly string[] m_IconFileListAndroidRound =
    {
       "ic_launcher_round.png",
    };

    private static readonly string[] m_IconFileListAndroidAdaptive =
    {
       "ic_launcher_background.png",
       "ic_launcher_foreground.png"
    };

    private static readonly PlatformIconKind[] m_AndroidIconKinds =
    {
        UnityEditor.Android.AndroidPlatformIconKind.Legacy,
        UnityEditor.Android.AndroidPlatformIconKind.Round,
        UnityEditor.Android.AndroidPlatformIconKind.Adaptive
    };

    [MenuItem("Auto Set Icon/Set Icon Path", false, 0)]
    public static void Open()
    {
        if (GetPathAsset())
        {
            m_Asset = CreateInstance<IconPath>();
            m_Asset.m_iOSIconPath = m_DefualtIOSPath;
            m_Asset.m_AndroidIconPath = m_DefualtAndroidPath;
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(m_AssetPath);
            AssetDatabase.CreateAsset(m_Asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        var window = GetWindow<SetIconTool>("SetIconTool");
        window.minSize = window.maxSize = new Vector2(400, 200);
    }

    [MenuItem("Auto Set Icon/iOS", false, 100)]
    public static void SetIconIOS()
    {
        if (GetPathAsset())
        {
            Open();
            //Debug.LogWarning("Set icon path first, please!");
            return;
        }
        iOSIcon(m_Asset.m_iOSIconPath);
    }

    [MenuItem("Auto Set Icon/Android", false, 100)]
    public static void SetIcoAndroid()
    {
        if (GetPathAsset())
        {
            Open();
            //Debug.LogWarning("Set icon path first, please!");
            return;
        }

        for (int i = 0; i < m_AndroidIconKinds.Length; i++)
        {
            Androidcon(m_Asset.m_AndroidIconPath, m_AndroidIconKinds[i]);
        }
    }

    private static bool GetPathAsset()
    {
        m_Asset = AssetDatabase.LoadMainAssetAtPath(m_AssetPath) as IconPath;
        return m_Asset == null;
    }

    private static void iOSIcon(string dir, IconKind kind = IconKind.Any)
    {
        var sizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.iOS, kind);
        Texture2D[] iOSicons = new Texture2D[sizes.Length];

        for (int i = 0; i < iOSicons.Length; ++i)
        {
            Texture2D icon = AssetDatabase.LoadMainAssetAtPath(dir + "/" + m_IconPathListIOS[i]) as Texture2D;
            if (icon != null)
            {
                iOSicons[i] = icon;
            }
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, iOSicons, kind);
    }

    private static void Androidcon(string dir, PlatformIconKind kind)
    {
        var platform = NamedBuildTarget.Android;
        var icons = PlayerSettings.GetPlatformIcons(platform, kind);
        string[] fileList = m_IconFileListAndroidLegecy;
        if (kind.Equals(UnityEditor.Android.AndroidPlatformIconKind.Adaptive))
        {
            fileList = m_IconFileListAndroidAdaptive;
        }
        else if (kind.Equals(UnityEditor.Android.AndroidPlatformIconKind.Round))
        {
            fileList = m_IconFileListAndroidRound;
        }

        for (int i = 0; i < m_IconPathListAndroid.Length; i++)
        {
            for (int j = 0; j < fileList.Length; j++)
            {
                Texture2D icon = AssetDatabase.LoadMainAssetAtPath(dir + "/" + m_IconPathListAndroid[i] + "/" + fileList[j]) as Texture2D;
                if (icon != null)
                {
                    icons[i].SetTexture(icon, j);
                }
            }
        }
        PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android, kind, icons);
    }


    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            m_Asset.m_iOSIconPath = EditorGUILayout.TextField("iOS Icon Path", m_Asset.m_iOSIconPath);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            m_Asset.m_AndroidIconPath = EditorGUILayout.TextField("Android Icon Path", m_Asset.m_AndroidIconPath);
        }
        EditorGUILayout.EndHorizontal();
    }


}