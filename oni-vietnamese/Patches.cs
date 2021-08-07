using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using TMPro;
using UnityEngine;

namespace oni_vietnamese {
    using CB = Action<MotdServerClient.MotdResponse, string>;
    using Resp = MotdServerClient.MotdResponse;

    public class Patches : KMod.UserMod2 {
        private const string fn = "GRAYSTROKE";
        private static readonly string ns = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        private static TMP_FontAsset font;

        public override void OnLoad(Harmony harmony) {
            harmony.PatchAll();
            PUtil.InitLibrary();

            using (var stream = GetResourceStream($"font")) {
                font = AssetBundle.LoadFromStream(stream).LoadAsset<TMP_FontAsset>(fn);

                TMP_Settings.fallbackFontAssets.Add(font);
            }

            AssetBundle.UnloadAllAssetBundles(false);

            // Hotfix Exposure Tiers
            STRINGS.DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.TIER1 = "Phơi nhiễm nhẹ";
            STRINGS.DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.TIER2 = "Phơi nhiễm";
            STRINGS.DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.TIER3 = "Phơi nhiễm nặng";
            typeof(STRINGS.DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS)
                .GetField("EXPOSURE_TIERS")
                .SetValue(null, new LocString[] { "Phơi nhiễm nhẹ", "Phơi nhiễm", "Phơi nhiễm nặng" });
        }

        private static Stream GetResourceStream(string name) {
            return Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"{ns}.Assets.{name}");
        }

        private static void ReassignFont(IEnumerable<TextMeshProUGUI> sequence) {
            sequence.DoIf(
                tmpg => tmpg != null && tmpg.font != null && tmpg.font.name != fn,
                tmpg => tmpg.font = font
            );
        }

        private static void ReassignString(ref string target, string targetString, string newString) {
            if (target.Contains(targetString)) target = target.Replace(targetString, newString);
        }

        [HarmonyPatch(typeof(Localization))]
        [HarmonyPatch(nameof(Localization.Initialize))]
        public static class Localization_Initialize_Patch {
            public static bool Prefix() {
                var lines = new List<string>();

                using (var stream = GetResourceStream("strings.po"))
                using (var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    while (!streamReader.EndOfStream) lines.Add(streamReader.ReadLine());

                Localization.OverloadStrings(Localization.ExtractTranslatedStrings(lines.ToArray(), false));

                Localization.SwapToLocalizedFont(fn);

                return false;
            }
        }

        [HarmonyPatch(typeof(LanguageOptionsScreen))]
        [HarmonyPatch("RebuildPreinstalledButtons")]
        public static class LanguageOptionsScreen_RebuildPreinstalledButtons_Patch {
            public static bool Prefix(LanguageOptionsScreen __instance, ref List<GameObject> ___buttons) {
                var sprite = PUIUtils.LoadSprite($"{ns}.Assets.preview.png");
                var gameObject = Util.KInstantiateUI(
                    __instance.languageButtonPrefab,
                    __instance.preinstalledLanguagesContainer,
                    false
                );
                var component = gameObject.GetComponent<HierarchyReferences>();
                var reference = component.GetReference<LocText>("Title");

                reference.text = "ONI Tiếng Việt";

                component.GetReference<UnityEngine.UI.Image>("Image").sprite = sprite;

                ___buttons.Add(gameObject);

                return false;
            }
        }

        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("OnPrefabInit")]
        public static class Game_OnPrefabInit_Patch {
            public static void Prefix() {
                ReassignFont(Resources.FindObjectsOfTypeAll<LocText>());
            }
        }

        [HarmonyPatch(typeof(NameDisplayScreen))]
        [HarmonyPatch(nameof(NameDisplayScreen.AddNewEntry))]
        public static class NameDisplayScreen_AddNewEntry_Patch {
            public static void Postfix(NameDisplayScreen __instance, GameObject representedObject) {
                var targetEntry = __instance.entries.Find(entry => entry.world_go == representedObject);
                if (targetEntry != null && targetEntry.display_go != null) {
                    var txt = targetEntry.display_go.GetComponentInChildren<LocText>();
                    if (txt != null && txt.font.name != fn) txt.font = font;
                }
            }
        }

        [HarmonyPatch(typeof(MotdServerClient))]
        [HarmonyPatch(nameof(MotdServerClient.GetMotd))]
        public static class MotdServerClient_GetMotd_Patch {
            private static Type Resp = typeof(Action<MotdServerClient.MotdResponse, string>);

            private static MethodInfo GetLocalMotd = typeof(MotdServerClient).GetMethod(
                "GetLocalMotd",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            private static PropertyInfo MotdLocalPath = typeof(MotdServerClient).GetProperty(
                "MotdLocalPath",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            public static bool Prefix(MotdServerClient __instance, CB cb) {
                var path = MotdLocalPath.GetValue(__instance, null);
                var localMotd = GetLocalMotd.Invoke(__instance, new object[] { path }) as Resp;

                localMotd.image_header_text = "Bản cập nhật mới";
                localMotd.news_header_text = "Tham gia thảo luận";
                localMotd.news_body_text = "Đăng ký email thông báo của chúng tôi\n"
                                            + "để cập nhật những thông tin mới nhất\n"
                                            + "hoặc vào diễn đàn để trực tiếp tham gia thảo luận！";
                localMotd.patch_notes_summary = "<b>Bản cập nhật tháng 7 năm 2021</b>\n\n"
                                                + "• Chuyển đổi từ menu chính của<i>《Spaced Out!》</i>về lại bản gốc mà không cần tải lại game\n"
                                                + "• Tất cả các bản cập nhật và sửa lỗi cho DLC <i>《Spaced Out!》</i>\n"
                                                + "• Cập nhật nhiệm vụ thuộc địa cho <i>《Spaced Out!》</i>\n"
                                                + "• Thêm các công trình mới: mặt nạ dưỡng khí, van đồng hồ...\n"
                                                + "• Một bản cập nhật rất lớn, nhiều module thay đổi\n\n"
                                                + "Vui lòng đọc thêm hướng dẫn để biết thêm chi tiết！";

                cb(localMotd, null);

                return false;
            }
        }

        [HarmonyPatch(typeof(PatchNotesScreen))]
        [HarmonyPatch("OnSpawn")]
        public static class PatchNotesScreen_OnSpawn_Patch {
            public static void Postfix(PatchNotesScreen __instance) {
                __instance
                    .GetComponentsInChildren<TextMeshProUGUI>()
                    .DoIf(
                        txt => txt != null && txt.name == "Title",
                        txt => txt.text = "Phát hành"
                    );
            }
        }
    }
}

