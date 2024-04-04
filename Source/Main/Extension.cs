using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
	
	internal static class Extension
	{
		
		internal static void AddTag(ref TagFilter t, string s)
		{
			bool flag = t == null;
			if (flag)
			{
				t = new TagFilter();
			}
			t.tags.Add(s);
		}

		
		internal static void ChangeXPosition(this Rect rect, ref bool bdoFlag, int xoffset)
		{
			bool flag = !bdoFlag;
			if (!flag)
			{
				bdoFlag = false;
				rect.position = new Vector2(rect.position.x + (float)xoffset, rect.position.y);
			}
		}

		
		internal static void AddFromList<T1, T2>(this SortedDictionary<T1, T2> dic, List<T1> otherList, T2 defaultVal = default(T2))
		{
			bool flag = dic != null && !otherList.NullOrEmpty<T1>();
			if (flag)
			{
				foreach (T1 key in otherList)
				{
					bool flag2 = !dic.ContainsKey(key);
					if (flag2)
					{
						dic.Add(key, defaultVal);
					}
				}
			}
		}

		
		internal static void AddFromList<T1, T2>(this Dictionary<T1, T2> dic, List<T1> otherList, T2 defaultVal = default(T2))
		{
			bool flag = dic != null && !otherList.NullOrEmpty<T1>();
			if (flag)
			{
				foreach (T1 key in otherList)
				{
					bool flag2 = !dic.ContainsKey(key);
					if (flag2)
					{
						dic.Add(key, defaultVal);
					}
				}
			}
		}

		
		internal static void AddLabeled<T>(this Dictionary<string, T> dic, string key, T value)
		{
			bool flag = dic == null;
			if (flag)
			{
				dic = new Dictionary<string, T>();
			}
			bool flag2 = !dic.ContainsKey(key);
			if (flag2)
			{
				dic.Add(key, value);
			}
			else
			{
				dic.Add(key + dic.Count.ToString(), value);
			}
		}

		
		internal static void AddLabeled<T>(this SortedDictionary<string, T> dic, string key, T value)
		{
			bool flag = dic == null;
			if (flag)
			{
				dic = new SortedDictionary<string, T>();
			}
			bool flag2 = !dic.ContainsKey(key);
			if (flag2)
			{
				dic.Add(key, value);
			}
			else
			{
				dic.Add(key + dic.Count.ToString(), value);
			}
		}

		
		internal static void AddSkipDuplicate<T1, T2>(this Dictionary<T1, T2> dic, T1 key, T2 value)
		{
			bool flag = dic == null;
			if (flag)
			{
				dic = new Dictionary<T1, T2>();
			}
			bool flag2 = !dic.ContainsKey(key);
			if (flag2)
			{
				dic.Add(key, value);
			}
		}

		
		internal static void AddSkipDuplicate<T1, T2>(this SortedDictionary<T1, T2> dic, T1 key, T2 value)
		{
			bool flag = dic == null;
			if (flag)
			{
				dic = new SortedDictionary<T1, T2>();
			}
			bool flag2 = !dic.ContainsKey(key);
			if (flag2)
			{
				dic.Add(key, value);
			}
		}

		
		internal static string AsString<T1, T2>(this Dictionary<T1, T2> dic, string pairator = "", string separator = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = dic != null && dic.Count > 0;
			if (flag)
			{
				int num = dic.Count;
				using (Dictionary<T1, T2>.Enumerator enumerator = dic.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						num--;
						StringBuilder stringBuilder2 = stringBuilder;
						KeyValuePair<T1, T2> keyValuePair = enumerator.Current;
						T1 key = keyValuePair.Key;
						stringBuilder2.Append(key.ToString());
						stringBuilder.Append(pairator);
						StringBuilder stringBuilder3 = stringBuilder;
						keyValuePair = enumerator.Current;
						T2 value = keyValuePair.Value;
						stringBuilder3.Append(value.ToString());
						bool flag2 = num > 0;
						if (flag2)
						{
							stringBuilder.Append(separator);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		
		internal static string AsString<T1, T2>(this SortedDictionary<T1, T2> dic, string pairator = "", string separator = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = dic != null && dic.Count > 0;
			if (flag)
			{
				int num = dic.Count;
				using (SortedDictionary<T1, T2>.Enumerator enumerator = dic.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						num--;
						StringBuilder stringBuilder2 = stringBuilder;
						KeyValuePair<T1, T2> keyValuePair = enumerator.Current;
						T1 key = keyValuePair.Key;
						stringBuilder2.Append(key.ToString());
						stringBuilder.Append(pairator);
						StringBuilder stringBuilder3 = stringBuilder;
						keyValuePair = enumerator.Current;
						T2 value = keyValuePair.Value;
						stringBuilder3.Append(value.ToString());
						bool flag2 = num > 0;
						if (flag2)
						{
							stringBuilder.Append(separator);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		
		internal static T2 GetValue<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
		{
			bool flag = dic != null && dic.ContainsKey(key);
			T2 result;
			if (flag)
			{
				result = dic[key];
			}
			else
			{
				result = default(T2);
			}
			return result;
		}

		
		internal static T2 GetValue<T1, T2>(this SortedDictionary<T1, T2> dic, T1 key)
		{
			bool flag = dic != null && dic.ContainsKey(key);
			T2 result;
			if (flag)
			{
				result = dic[key];
			}
			else
			{
				result = default(T2);
			}
			return result;
		}

		
		internal static T1 KeyByValue<T1, T2>(this Dictionary<T1, T2> dic, T2 value)
		{
			bool flag = dic != null;
			if (flag)
			{
				foreach (T1 t in dic.Keys)
				{
					bool flag2;
					if (dic[t] != null)
					{
						T2 t2 = dic[t];
						flag2 = (t2.ToString() == value.ToString());
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						return t;
					}
				}
			}
			return default(T1);
		}

		
		internal static string KeyByValue<T>(this SortedDictionary<string, T> dic, T value)
		{
			bool flag = dic != null;
			if (flag)
			{
				foreach (string text in dic.Keys)
				{
					T t = dic[text];
					bool flag2 = t.Equals(value);
					if (flag2)
					{
						return text;
					}
				}
			}
			return "";
		}

		
		internal static void Merge<T1, T2>(this Dictionary<T1, T2> dic1, Dictionary<T1, T2> dic2)
		{
			bool flag = dic1 == null;
			if (flag)
			{
				dic1 = new Dictionary<T1, T2>();
			}
			foreach (T1 key in dic2.Keys)
			{
				bool flag2 = !dic1.ContainsKey(key);
				if (flag2)
				{
					dic1.Add(key, dic2[key]);
				}
			}
		}

		
		internal static void AddElem<T>(ref List<T> l, T element)
		{
			bool flag = l == null;
			if (flag)
			{
				l = new List<T>();
			}
			l.Add(element);
		}

		
		internal static void AddElemUnique<T>(ref List<T> l, T element)
		{
			bool flag = l == null;
			if (flag)
			{
				l = new List<T>();
			}
			bool flag2 = !l.Contains(element);
			if (flag2)
			{
				l.Add(element);
			}
		}

		
		internal static void AddFromList<T>(this List<T> l, List<T> otherList)
		{
			bool flag = l != null && !otherList.NullOrEmpty<T>();
			if (flag)
			{
				foreach (T item in otherList)
				{
					bool flag2 = !l.Contains(item);
					if (flag2)
					{
						l.Add(item);
					}
				}
			}
		}

		
		internal static bool IsSame<T>(this List<T> l, List<T> otherList)
		{
			foreach (T item in otherList)
			{
				bool flag = !l.Contains(item);
				if (flag)
				{
					return false;
				}
			}
			foreach (T item2 in l)
			{
				bool flag2 = !l.Contains(item2);
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		
		internal static string ListToString<T>(this List<T> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = list != null;
			string result;
			if (flag)
			{
				bool flag2 = typeof(T) == typeof(string);
				if (flag2)
				{
					foreach (string str in list.OfType<string>())
					{
						stringBuilder.Append(str + "|");
					}
				}
				else
				{
					bool flag3 = typeof(T) == typeof(int);
					if (flag3)
					{
						foreach (int num in list.OfType<int>())
						{
							stringBuilder.Append(num.ToString() + "|");
						}
					}
					else
					{
						bool flag4 = typeof(T) == typeof(float);
						if (flag4)
						{
							foreach (float num2 in list.OfType<float>())
							{
								stringBuilder.Append(num2.ToString() + "|");
							}
						}
						else
						{
							bool flag5 = typeof(T) == typeof(StatModifier);
							if (flag5)
							{
								foreach (StatModifier statModifier in list.OfType<StatModifier>())
								{
									bool flag6 = statModifier != null && statModifier.stat != null && statModifier.stat.defName != null;
									if (flag6)
									{
										stringBuilder.Append(statModifier.stat.defName + "!" + statModifier.value.ToString() + "|");
									}
								}
							}
							else
							{
								bool flag7 = typeof(T) == typeof(Aptitude);
								if (flag7)
								{
									foreach (Aptitude aptitude in list.OfType<Aptitude>())
									{
										bool flag8 = aptitude != null && aptitude.skill != null && aptitude.skill.defName != null;
										if (flag8)
										{
											stringBuilder.Append(aptitude.skill.defName + "!" + aptitude.level.ToString() + "|");
										}
									}
								}
								else
								{
									bool flag9 = typeof(T) == typeof(ThingDefCountClass);
									if (flag9)
									{
										foreach (ThingDefCountClass thingDefCountClass in list.OfType<ThingDefCountClass>())
										{
											bool flag10 = thingDefCountClass != null && thingDefCountClass.thingDef != null && thingDefCountClass.thingDef.defName != null;
											if (flag10)
											{
												stringBuilder.Append(thingDefCountClass.thingDef.defName + "!" + thingDefCountClass.count.ToString() + "|");
											}
										}
									}
									else
									{
										bool flag11 = typeof(T) == typeof(GeneticTraitData);
										if (flag11)
										{
											foreach (GeneticTraitData geneticTraitData in list.OfType<GeneticTraitData>())
											{
												bool flag12 = geneticTraitData != null && geneticTraitData.def != null && geneticTraitData.def.defName != null;
												if (flag12)
												{
													stringBuilder.Append(geneticTraitData.def.defName + "!" + geneticTraitData.degree.ToString() + "|");
												}
											}
										}
										else
										{
											bool flag13 = typeof(T) == typeof(DamageFactor);
											if (flag13)
											{
												foreach (DamageFactor damageFactor in list.OfType<DamageFactor>())
												{
													bool flag14 = damageFactor != null && damageFactor.damageDef != null && damageFactor.damageDef.defName != null;
													if (flag14)
													{
														stringBuilder.Append(damageFactor.damageDef.defName + "!" + damageFactor.factor.ToString() + "|");
													}
												}
											}
											else
											{
												bool flag15 = typeof(T) == typeof(PawnCapacityModifier);
												if (flag15)
												{
													foreach (PawnCapacityModifier pawnCapacityModifier in list.OfType<PawnCapacityModifier>())
													{
														bool flag16 = pawnCapacityModifier != null && pawnCapacityModifier.capacity != null && pawnCapacityModifier.capacity.defName != null;
														if (flag16)
														{
															stringBuilder.Append(string.Concat(new string[]
															{
																pawnCapacityModifier.capacity.defName,
																"!",
																pawnCapacityModifier.offset.ToString(),
																"#",
																pawnCapacityModifier.postFactor.ToString(),
																"|"
															}));
														}
													}
												}
												else
												{
													bool flag17 = typeof(T) == typeof(BodyPartRecord);
													if (flag17)
													{
														foreach (BodyPartRecord bodyPartRecord in list.OfType<BodyPartRecord>())
														{
															bool flag18 = bodyPartRecord != null && bodyPartRecord.def.defName != null;
															if (flag18)
															{
																stringBuilder.Append(bodyPartRecord.def.defName + "|");
															}
														}
													}
													else
													{
														bool flag19 = typeof(T) == typeof(TraitDef);
														if (flag19)
														{
															foreach (TraitDef traitDef in list.OfType<TraitDef>())
															{
																bool flag20 = traitDef != null && traitDef.defName != null;
																if (flag20)
																{
																	stringBuilder.Append(traitDef.defName + " (" + traitDef.degreeDatas.ListToString<TraitDegreeData>() + ")|");
																}
															}
														}
														else
														{
															bool flag21 = typeof(T) == typeof(TraitDegreeData);
															if (flag21)
															{
																foreach (TraitDegreeData traitDegreeData in list.OfType<TraitDegreeData>())
																{
																	bool flag22 = traitDegreeData != null && traitDegreeData.label != null;
																	if (flag22)
																	{
																		stringBuilder.Append(traitDegreeData.label + "|");
																	}
																}
															}
															else
															{
																bool flag23 = typeof(T) == typeof(Apparel);
																if (flag23)
																{
																	foreach (Apparel apparel in list.OfType<Apparel>())
																	{
																		bool flag24 = apparel != null && apparel.Label != null;
																		if (flag24)
																		{
																			stringBuilder.Append(apparel.Label + ", ");
																		}
																	}
																}
																else
																{
																	bool flag25 = typeof(T) == typeof(Pawn);
																	if (flag25)
																	{
																		foreach (Pawn pawn in list.OfType<Pawn>())
																		{
																			bool flag26 = pawn != null && pawn.Label != null;
																			if (flag26)
																			{
																				stringBuilder.Append(pawn.Label + "|");
																			}
																		}
																	}
																	else
																	{
																		bool flag27 = typeof(T) == typeof(Texture2D);
																		if (flag27)
																		{
																			foreach (Texture2D texture2D in list.OfType<Texture2D>())
																			{
																				bool flag28 = texture2D != null && texture2D.name != null;
																				if (flag28)
																				{
																					stringBuilder.Append(texture2D.name + "\n");
																				}
																			}
																		}
																		else
																		{
																			bool flag29 = typeof(T) == typeof(Def) || typeof(T).BaseType == typeof(Def) || (typeof(T).BaseType != null && typeof(T).BaseType.BaseType == typeof(Def));
																			if (flag29)
																			{
																				foreach (Def def in list.OfType<Def>())
																				{
																					bool flag30 = def != null && def.defName != null;
																					if (flag30)
																					{
																						stringBuilder.Append(def.defName + "|");
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				string text = stringBuilder.ToString();
				bool flag31 = !string.IsNullOrEmpty(text);
				if (flag31)
				{
					text = text.Remove(text.Length - 1, 1);
				}
				result = text;
			}
			else
			{
				result = "";
			}
			return result;
		}

		
		internal static int NextOrPrevIndex<T>(this List<T> l, int index, bool next, bool random)
		{
			bool flag = l.NullOrEmpty<T>();
			int result;
			if (flag)
			{
				result = 0;
			}
			else if (random)
			{
				result = l.IndexOf(l.RandomElement<T>());
			}
			else
			{
				if (next)
				{
					bool flag2 = index + 1 < l.Count;
					if (flag2)
					{
						index++;
					}
					else
					{
						index = 0;
					}
				}
				else
				{
					bool flag3 = index - 1 >= 0;
					if (flag3)
					{
						index--;
					}
					else
					{
						index = l.Count - 1;
					}
				}
				result = index;
			}
			return result;
		}

		
		internal static int NextOrPrevIndex<T>(this HashSet<T> l, int index, bool next, bool random)
		{
			bool flag = l.NullOrEmpty<T>();
			int result;
			if (flag)
			{
				result = 0;
			}
			else if (random)
			{
				result = l.IndexOf(l.RandomElement<T>());
			}
			else
			{
				if (next)
				{
					bool flag2 = index + 1 < l.Count;
					if (flag2)
					{
						index++;
					}
					else
					{
						index = 0;
					}
				}
				else
				{
					bool flag3 = index - 1 >= 0;
					if (flag3)
					{
						index--;
					}
					else
					{
						index = l.Count - 1;
					}
				}
				result = index;
			}
			return result;
		}

		
		internal static int IndexOf<T>(this HashSet<T> l, T val)
		{
			return (l.NullOrEmpty<T>() || val == null) ? 0 : l.FirstIndexOf((T y) => val.Equals(y));
		}

		
		internal static T At<T>(this HashSet<T> l, int index)
		{
			return l.NullOrEmpty<T>() ? default(T) : l.ElementAt(index);
		}

		
		internal static T At<T>(this List<T> l, int index)
		{
			return l.NullOrEmpty<T>() ? default(T) : l.ElementAt(index);
		}

		
		internal static bool NullOrEmpty<T>(this HashSet<T> l)
		{
			return l == null || l.Count == 0;
		}

		
		internal static void RemoveDuplicates<T>(this HashSet<T> l) where T : Def
		{
			HashSet<T> hashSet = new HashSet<T>();
			foreach (T item in l)
			{
				bool flag = !hashSet.Contains(item);
				if (flag)
				{
					hashSet.Add(item);
				}
			}
			l = hashSet;
		}

		
		internal static string[] SplitNo(this string s, string x)
		{
			return s.Split(new string[]
			{
				x
			}, StringSplitOptions.None);
		}

		
		internal static string[] SplitNoEmpty(this string s, string x)
		{
			return s.Split(new string[]
			{
				x
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		
		internal static bool AsBool(this string s)
		{
			return s == "1" || s == "True";
		}

		
		internal static bool AsBoolWithDefault(this string s, bool defVal)
		{
			return s.NullOrEmpty() ? defVal : (s == "1" || s == "True");
		}

		
		internal static float AsFloat(this string input)
		{
			float result = 0f;
			float.TryParse(input, out result);
			return result;
		}

		
		internal static float? AsFloatZero(this string input)
		{
			float value = 0f;
			bool flag = float.TryParse(input, out value);
			float? result;
			if (flag)
			{
				result = new float?(value);
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		internal static int AsInt32(this string input)
		{
			int result = 0;
			int.TryParse(input, out result);
			return result;
		}

		
		internal static long AsLong(this string input)
		{
			long result = 0L;
			long.TryParse(input, out result);
			return result;
		}

		
		internal static Vector3? AsVector3Zero(this string input)
		{
			string[] array = input.SplitNo("#");
			bool flag = array.Length == 3 && !array[0].NullOrEmpty() && !array[1].NullOrEmpty() && !array[2].NullOrEmpty();
			Vector3? result;
			if (flag)
			{
				float x = array[0].AsFloat();
				float y = array[1].AsFloat();
				float z = array[2].AsFloat();
				result = new Vector3?(new Vector3(x, y, z));
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		internal static string AsString(this Vector3? v)
		{
			string result;
			if (v != null)
			{
				string[] array = new string[5];
				int num = 0;
				Vector3 value = v.Value;
				array[num] = value.x.ToString();
				array[1] = "#";
				int num2 = 2;
				value = v.Value;
				array[num2] = value.y.ToString();
				array[3] = "#";
				int num3 = 4;
				value = v.Value;
				array[num3] = value.z.ToString();
				result = string.Concat(array);
			}
			else
			{
				result = "";
			}
			return result;
		}

		
		internal static void AsDefValue(this string s, out string defName, out string value)
		{
			string[] array = s.SplitNo("!");
			defName = array.GetStringValue(0);
			value = array.GetStringValue(1);
		}

		
		internal static KeyCode AsKeyCode(this string key)
		{
			KeyCode result;
			try
			{
				result = (KeyCode)Enum.Parse(typeof(KeyCode), key);
			}
			catch
			{
				result = KeyCode.None;
			}
			return result;
		}

		
		internal static string ParamDictionaryAsString<T>(SortedDictionary<T, string> dicParams)
		{
			string text = "";
			foreach (T key in dicParams.Keys)
			{
				text = text + dicParams[key] + ",";
			}
			text = text.SubstringRemoveLast();
			return text;
		}

		
		internal static bool LoadSeparatedStringIntoDictionary<T>(string custom, ref SortedDictionary<T, string> dicParams, Func<int, T> Tgetter, int maxParamCount)
		{
			dicParams = new SortedDictionary<T, string>();
			bool flag = !string.IsNullOrEmpty(custom);
			if (flag)
			{
				string[] array = custom.Trim().SplitNo(",");
				foreach (string value in array)
				{
					bool flag2 = dicParams.Count < maxParamCount;
					if (flag2)
					{
						dicParams.Add(Tgetter(dicParams.Count), value);
					}
				}
			}
			return dicParams.Count != 0;
		}

		
		internal static List<string> StringToList(this string slist)
		{
			List<string> list = new List<string>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string item in array)
				{
					list.Add(item);
				}
			}
			return list;
		}

		
		internal static List<float> StringToFList(this string slist)
		{
			List<float> list = new List<float>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string input in array)
				{
					list.Add(input.AsFloat());
				}
			}
			return list;
		}

		
		internal static List<StatModifier> StringToListStatModifier(string slist)
		{
			List<StatModifier> list = new List<StatModifier>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string defName;
					string input;
					s.AsDefValue(out defName, out input);
					StatModifier statModifier = new StatModifier();
					statModifier.stat = DefTool.StatDef(defName);
					bool flag2 = statModifier.stat != null;
					if (flag2)
					{
						statModifier.value = input.AsFloat();
						list.Add(statModifier);
					}
				}
			}
			return list;
		}

		
		internal static List<Aptitude> StringToListAptitudes(string slist)
		{
			List<Aptitude> list = new List<Aptitude>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string defName;
					string input;
					s.AsDefValue(out defName, out input);
					Aptitude aptitude = new Aptitude();
					aptitude.skill = DefTool.SkillDef(defName);
					bool flag2 = aptitude.skill != null;
					if (flag2)
					{
						aptitude.level = input.AsInt32();
						list.Add(aptitude);
					}
				}
			}
			return list;
		}

		
		internal static List<ThingDefCountClass> StringToListThingDefCountClass(string slist)
		{
			List<ThingDefCountClass> list = new List<ThingDefCountClass>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string defName;
					string input;
					s.AsDefValue(out defName, out input);
					ThingDefCountClass thingDefCountClass = new ThingDefCountClass();
					thingDefCountClass.thingDef = DefTool.GetDef<ThingDef>(defName);
					bool flag2 = thingDefCountClass.thingDef != null;
					if (flag2)
					{
						thingDefCountClass.count = input.AsInt32();
						list.Add(thingDefCountClass);
					}
				}
			}
			return list;
		}

		
		internal static List<PassionMod> StringToListPassionMods(string slist)
		{
			List<PassionMod> list = new List<PassionMod>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string defName;
					string value;
					s.AsDefValue(out defName, out value);
					PassionMod passionMod = new PassionMod();
					passionMod.skill = DefTool.GetDef<SkillDef>(defName);
					bool flag2 = passionMod.skill != null;
					if (flag2)
					{
						PassionMod.PassionModType modType;
						bool flag3 = Enum.TryParse<PassionMod.PassionModType>(value, out modType);
						if (flag3)
						{
							passionMod.modType = modType;
							list.Add(passionMod);
						}
					}
				}
			}
			return list;
		}

		
		internal static TagFilter StringToTagFilter(this string slist)
		{
			bool flag = !slist.NullOrEmpty();
			TagFilter result;
			if (flag)
			{
				result = new TagFilter
				{
					tags = slist.StringToList()
				};
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		internal static string AsListString(this PassionMod p)
		{
			string result = "";
			bool flag = p != null && p.skill != null && p.skill.defName != null;
			if (flag)
			{
				string defName = p.skill.defName;
				string str = "!";
				int modType = (int)p.modType;
				result = defName + str + modType.ToString() + "|";
			}
			return result;
		}

		
		internal static string AsListString(this TagFilter t)
		{
			bool flag = t != null && !t.tags.NullOrEmpty<string>();
			string result;
			if (flag)
			{
				result = t.tags.ListToString<string>();
			}
			else
			{
				result = "";
			}
			return result;
		}

		
		internal static List<T> StringToListNonDef<T>(this string slist)
		{
			bool flag = typeof(T) == typeof(StatModifier);
			List<T> result;
			if (flag)
			{
				result = (Extension.StringToListStatModifier(slist) as List<T>);
			}
			else
			{
				bool flag2 = typeof(T) == typeof(Aptitude);
				if (flag2)
				{
					result = (Extension.StringToListAptitudes(slist) as List<T>);
				}
				else
				{
					bool flag3 = typeof(T) == typeof(PawnCapacityModifier);
					if (flag3)
					{
						result = (Extension.StringToListCapacities(slist) as List<T>);
					}
					else
					{
						bool flag4 = typeof(T) == typeof(GeneticTraitData);
						if (flag4)
						{
							result = (Extension.StringToListGeneticTraitData(slist) as List<T>);
						}
						else
						{
							bool flag5 = typeof(T) == typeof(DamageFactor);
							if (flag5)
							{
								result = (Extension.StringToListDamageFactors(slist) as List<T>);
							}
							else
							{
								bool flag6 = typeof(T) == typeof(PassionMod);
								if (flag6)
								{
									result = (Extension.StringToListPassionMods(slist) as List<T>);
								}
								else
								{
									bool flag7 = typeof(T) == typeof(ThingDefCountClass);
									if (flag7)
									{
										result = (Extension.StringToListThingDefCountClass(slist) as List<T>);
									}
									else
									{
										result = null;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		internal static List<T> StringToList<T>(this string slist) where T : Def
		{
			List<T> list = new List<T>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string defName in array)
				{
					T def = DefTool.GetDef<T>(defName);
					bool flag2 = def != null;
					if (flag2)
					{
						list.Add(def);
					}
				}
			}
			return list;
		}

		
		internal static List<DamageFactor> StringToListDamageFactors(string slist)
		{
			List<DamageFactor> list = new List<DamageFactor>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string defName;
					string input;
					s.AsDefValue(out defName, out input);
					DamageFactor damageFactor = new DamageFactor();
					damageFactor.damageDef = DefTool.GetDef<DamageDef>(defName);
					bool flag2 = damageFactor.damageDef != null;
					if (flag2)
					{
						damageFactor.factor = input.AsFloat();
						list.Add(damageFactor);
					}
				}
			}
			return list;
		}

		
		internal static List<GeneticTraitData> StringToListGeneticTraitData(string slist)
		{
			List<GeneticTraitData> list = new List<GeneticTraitData>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string defName;
					string input;
					s.AsDefValue(out defName, out input);
					GeneticTraitData geneticTraitData = new GeneticTraitData();
					geneticTraitData.def = DefTool.GetDef<TraitDef>(defName);
					bool flag2 = geneticTraitData.def != null;
					if (flag2)
					{
						geneticTraitData.degree = input.AsInt32();
						list.Add(geneticTraitData);
					}
				}
			}
			return list;
		}

		
		internal static List<PawnCapacityModifier> StringToListCapacities(string slist)
		{
			List<PawnCapacityModifier> list = new List<PawnCapacityModifier>();
			bool flag = !string.IsNullOrEmpty(slist);
			if (flag)
			{
				string[] array = slist.SplitNoEmpty("|");
				foreach (string s in array)
				{
					string[] array3 = s.SplitNo("!");
					string stringValue = array3.GetStringValue(0);
					string stringValue2 = array3.GetStringValue(1);
					string[] array4 = stringValue2.SplitNo("#");
					string stringValue3 = array4.GetStringValue(0);
					string stringValue4 = array4.GetStringValue(1);
					PawnCapacityModifier pawnCapacityModifier = new PawnCapacityModifier();
					pawnCapacityModifier.capacity = DefTool.PawnCapacityDef(stringValue);
					bool flag2 = pawnCapacityModifier.capacity != null;
					if (flag2)
					{
						pawnCapacityModifier.offset = stringValue3.AsFloat();
						pawnCapacityModifier.postFactor = stringValue4.AsFloat();
						list.Add(pawnCapacityModifier);
					}
				}
			}
			return list;
		}

		
		internal static string GetStringValue(this string[] array, int index)
		{
			return (array.NullOrEmpty<string>() || array.Length <= index) ? "" : array[index];
		}

		
		internal static string SubstringBackwardFrom(this string text, string startFrom, bool withoutIt = true)
		{
			bool flag = text != null;
			if (flag)
			{
				int num = text.LastIndexOf(startFrom);
				bool flag2 = num >= 0;
				if (flag2)
				{
					if (withoutIt)
					{
						return text.Substring(num + startFrom.Length);
					}
					return text.Substring(num);
				}
			}
			return text;
		}

		
		internal static string SubstringBackwardTo(this string text, string endOn, bool withoutIt = true)
		{
			bool flag = text != null;
			if (flag)
			{
				int num = text.LastIndexOf(endOn);
				bool flag2 = num >= 0;
				if (flag2)
				{
					if (withoutIt)
					{
						return text.Substring(0, num);
					}
					return text.Substring(num);
				}
			}
			return text;
		}

		
		internal static string SubstringFrom(this string text, string from, int occuranceCount)
		{
			string text2 = text;
			for (int i = 0; i < occuranceCount; i++)
			{
				text2 = text2.SubstringFrom(from, true);
			}
			return text2;
		}

		
		internal static string SubstringFrom(this string text, string startFrom, bool withoutIt = true)
		{
			bool flag = text != null;
			if (flag)
			{
				int num = text.IndexOf(startFrom);
				bool flag2 = num >= 0;
				if (flag2)
				{
					if (withoutIt)
					{
						return text.Substring(num + startFrom.Length);
					}
					return text.Substring(num);
				}
			}
			return text;
		}

		
		internal static string SubstringTo(this string text, string to, int occuranceCount)
		{
			string text2 = text;
			string text3 = "";
			for (int i = 0; i < occuranceCount; i++)
			{
				text3 += text2.SubstringTo(to, false);
				text2 = text2.SubstringFrom(to, true);
			}
			bool flag = text3.Length > 0;
			string result;
			if (flag)
			{
				result = text3.Substring(0, text3.Length - 1);
			}
			else
			{
				result = text3;
			}
			return result;
		}

		
		internal static string SubstringTo(this string text, string endOn, bool withoutIt = true)
		{
			bool flag = text != null;
			if (flag)
			{
				int num = text.IndexOf(endOn);
				bool flag2 = num >= 0;
				if (flag2)
				{
					if (withoutIt)
					{
						return text.Substring(0, num);
					}
					return text.Substring(0, num + endOn.Length);
				}
			}
			return text;
		}

		
		internal static string SubstringRemoveLast(this string text)
		{
			return text.NullOrEmpty() ? text : text.Substring(0, text.Length - 1);
		}

		
		internal static string AsStringUNICODE(this byte[] input)
		{
			return input.AsString(Encoding.UTF8);
		}

		
		internal static string AsString(this byte[] input, Encoding enc)
		{
			bool flag = input.NullOrEmpty<byte>();
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				result = enc.GetString(input);
			}
			return result;
		}

		
		internal static byte[] AsBytes(this string text, Encoding enc)
		{
			bool flag = text == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = enc == null;
				if (flag2)
				{
					result = Encoding.Default.GetBytes(text);
				}
				else
				{
					result = enc.GetBytes(text);
				}
			}
			return result;
		}

		
		internal static string AsBase64(this string text, Encoding enc)
		{
			byte[] inArray = text.AsBytes(enc);
			return Convert.ToBase64String(inArray);
		}

		
		internal static byte[] Base64ToBytes(this string base64)
		{
			byte[] result = new byte[0];
			bool flag = !string.IsNullOrEmpty(base64);
			if (flag)
			{
				try
				{
					result = Convert.FromBase64String(base64);
				}
				catch
				{
				}
			}
			return result;
		}

		
		internal static string Base64ToString(this string base64, Encoding enc)
		{
			bool flag = base64 == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = enc == null;
				if (flag2)
				{
					result = Encoding.Default.GetString(base64.Base64ToBytes());
				}
				else
				{
					result = enc.GetString(base64.Base64ToBytes());
				}
			}
			return result;
		}

		
		internal static bool HasMID(string packageId)
		{
			ModMetaData modWithIdentifier = ModLister.GetModWithIdentifier(packageId, false);
			return modWithIdentifier != null && modWithIdentifier.Active;
		}
	}
}
