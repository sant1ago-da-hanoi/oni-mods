using System;
using System.Collections;
using HarmonyLib;

namespace ElementConfig {
	internal class Main {
		private static EleLTransInfo[] EleLTransList = new EleLTransInfo[] {
			new EleLTransInfo((SimHashes)(-927923200), 111.65f, (SimHashes)371787440, (SimHashes)(-729385479), 0.1f)
		};

		private static EleHTransInfo[] EleHTransList = new EleHTransInfo[] {
			new EleHTransInfo((SimHashes)1832607973, 353.15f, (SimHashes)1836671383, (SimHashes)869554203, 0.05f),
			new EleHTransInfo((SimHashes)(-1412059381), 673f, (SimHashes)(-486269331), (SimHashes)(-902240476), 0.05f),
			new EleHTransInfo((SimHashes)(-486269331), 812f, (SimHashes)(-927923200), (SimHashes)(-902240476), 0.05f),
			new EleHTransInfo((SimHashes)(-899253461), 1820f, (SimHashes)502659099, (SimHashes)(-902240476), 0.3f),
			new EleHTransInfo((SimHashes)361868060, 1337f, (SimHashes)(-1083496621), (SimHashes)(-537625624), 0.4f),
			new EleHTransInfo((SimHashes)(-1108652427), 717.8f, (SimHashes)(-2120504832), 0, 0f),
			new EleHTransInfo((SimHashes)(-1713958528), 2895f, (SimHashes)(-509585641), (SimHashes)(-729385479), 0.15f)
		};

		private static EleDensInfo[] EleDensList = new EleDensInfo[] {
			new EleDensInfo((SimHashes)(-314016756), 26.346f),
			new EleDensInfo((SimHashes)(-1324664829), 70.905f),
			new EleDensInfo((SimHashes)721531317, 32.159f),
			new EleDensInfo((SimHashes)(-1046145888), 2.01588f),
			new EleDensInfo((SimHashes)(-1528777920), 31.9988f),
			new EleDensInfo((SimHashes)1887387588, 123.895f),
			new EleDensInfo((SimHashes)(-432557516), 116f),
			new EleDensInfo((SimHashes)(-1946026749), 58.44f),
			new EleDensInfo((SimHashes)(-927923200), 21.331f),
			new EleDensInfo((SimHashes)(-899515856), 18.0153f),
			new EleDensInfo((SimHashes)(-2120504832), 256.52f),
			new EleDensInfo((SimHashes)(-751997156), 71f),
			new EleDensInfo((SimHashes)(-1934139602), 125f),
			new EleDensInfo((SimHashes)(-645698215), 493f),
			new EleDensInfo((SimHashes)660593444, 580f),
			new EleDensInfo((SimHashes)1157157570, 650f),
			new EleDensInfo((SimHashes)371787440, 717f),
			new EleDensInfo((SimHashes)(-87974045), 789f),
			new EleDensInfo((SimHashes)1832607973, 1030f),
			new EleDensInfo((SimHashes)1911997537, 1043f),
			new EleDensInfo((SimHashes)(-1526513293), 1101f),
			new EleDensInfo((SimHashes)(-1908044868), 1141f),
			new EleDensInfo((SimHashes)(-324547888), 1287f),
			new EleDensInfo((SimHashes)(-1312713527), 1400f),
			new EleDensInfo((SimHashes)(-422045879), 1550f),
			new EleDensInfo((SimHashes)(-878033482), 1563f),
			new EleDensInfo((SimHashes) 505665536, 1600f),
			new EleDensInfo((SimHashes) 1323821489, 1740f),
			new EleDensInfo((SimHashes)(-1108652427), 1819f),
			new EleDensInfo((SimHashes)(-1075911705), 2300f),
			new EleDensInfo((SimHashes) 2108244480, 2375f),
			new EleDensInfo((SimHashes) 502659099, 6980f),
			new EleDensInfo((SimHashes) 1499134368, 7600f),
			new EleDensInfo((SimHashes) 2128494380, 7992f),
			new EleDensInfo((SimHashes)(-1558864561), 10660f),
			new EleDensInfo((SimHashes) 1732126099, 13690f),
			new EleDensInfo((SimHashes) 1937473860, 17300f),
			new EleDensInfo((SimHashes)(-1083496621), 17310f),
			new EleDensInfo((SimHashes)(-509585641), 17600f)
		};

		[HarmonyPatch(typeof(ElementLoader), "FinaliseElementsTable")]
		public static class ElementLoader_Patch {
			public static void Prefix(ref Hashtable substanceList) {
				foreach (EleLTransInfo eleLTransInfo in EleLTransList) {
					Element element = ElementLoader.FindElementByHash(eleLTransInfo.id);
					bool flag = element != null;
					if (flag) {
						element.lowTemp = eleLTransInfo.lowTemp;
						element.lowTempTransitionTarget = eleLTransInfo.lowTransTarget;
						element.lowTempTransition = ElementLoader.FindElementByHash(eleLTransInfo.lowTransTarget);
						element.lowTempTransitionOreID = eleLTransInfo.lowTransOreId;
						element.lowTempTransitionOreMassConversion = eleLTransInfo.lowTransOreRatio;
					}
				}
				foreach (EleHTransInfo eleHTransInfo in EleHTransList) {
					Element element2 = ElementLoader.FindElementByHash(eleHTransInfo.id);
					bool flag2 = element2 != null;
					if (flag2) {
						element2.highTemp = eleHTransInfo.highTemp;
						element2.highTempTransitionTarget = eleHTransInfo.highTransTarget;
						element2.highTempTransition = ElementLoader.FindElementByHash(eleHTransInfo.highTransTarget);
						element2.highTempTransitionOreID = eleHTransInfo.highTransOreId;
						element2.highTempTransitionOreMassConversion = eleHTransInfo.highTransOreRatio;
					}
				}
				foreach (EleDensInfo eleDensInfo in EleDensList) {
					Element element3 = ElementLoader.FindElementByHash(eleDensInfo.id);
					bool flag3 = element3 != null;
					if (flag3) {
						bool isLiquid = element3.IsLiquid;
						if (isLiquid) {
							element3.maxMass = eleDensInfo.density;
						} else {
							bool isGas = element3.IsGas;
							if (isGas)
							{
								element3.molarMass = eleDensInfo.density;
							}
						}
					}
				}
				foreach (Element element4 in ElementLoader.elements) {
					bool flag4 = element4 != null;
					if (flag4) {
						bool isLiquid2 = element4.IsLiquid;
						if (isLiquid2) {
							element4.molarMass = element4.maxMass;
							element4.maxCompression = 1f / element4.viscosity + 1f;
						}
					}
				}
			}
		}

		private struct EleLTransInfo {
			public EleLTransInfo(SimHashes ID, float TEMP, SimHashes TARGT, SimHashes OREID, float ORERATIO) {
				this.id = ID;
				this.lowTemp = TEMP;
				this.lowTransTarget = TARGT;
				this.lowTransOreId = OREID;
				this.lowTransOreRatio = ORERATIO;
			}

			public SimHashes id;

			public float lowTemp;

			public SimHashes lowTransTarget;

			public SimHashes lowTransOreId;

			public float lowTransOreRatio;
		}

		private struct EleHTransInfo {
			public EleHTransInfo(SimHashes ID, float TEMP, SimHashes TARGT, SimHashes OREID, float ORERATIO) {
				this.id = ID;
				this.highTemp = TEMP;
				this.highTransTarget = TARGT;
				this.highTransOreId = OREID;
				this.highTransOreRatio = ORERATIO;
			}

			public SimHashes id;

			public float highTemp;

			public SimHashes highTransTarget;

			public SimHashes highTransOreId;

			public float highTransOreRatio;
		}

		private struct EleDensInfo {
			public EleDensInfo(SimHashes ID, float DENSITY) {
				this.id = ID;
				this.density = DENSITY;
			}

			public SimHashes id;

			public float density;
		}
	}
}
