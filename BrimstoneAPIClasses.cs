using System;
using System.Collections.Generic;
using Quintessential;
using MonoMod.Utils;
using SDL2;
using System.Linq;

namespace Brimstone;

using Font = class_1;
using Song = class_186;
using Tip = class_215;
using Texture = class_256;
public static partial class BrimstoneAPI
{
	// Yoinked from RMC
	public abstract class ResourceModel
	{
		internal static Dictionary<string, Texture> TextureBank;
		internal abstract Dictionary<string, Texture> InitialTextureBank();
		public void EnsureTextureBankExists() => TextureBank ??= this.InitialTextureBank();
		internal static Texture FetchTexture(string filePath)
		{
			if (!TextureBank.ContainsKey(filePath))
			{
				TextureBank[filePath] = class_235.method_615(filePath);
			}
			return TextureBank[filePath];
		}
	}

	public class AdvancedContentModel
	{
		public CreditsModel Credits;
		public List<int> SigmarStoryUnlocks;
		public List<int> LeftHandedChapters;
		public Dictionary<string, string> JournalRemappings;
		public List<CharacterModel> Characters;
		public List<CutsceneModel> Cutscenes;
		public List<DocumentModel> Documents;
		public List<PuzzleModel> Puzzles;
		public string name;

		public void ModifyCampaignItem(CampaignItem campaignItem)
		{
			if (!campaignItem.field_2325.method_1085()) return;
			string puzzleID = campaignItem.field_2325.method_1087().field_2766;
			if (this.Cutscenes is not null)
			{
				foreach (var cutsceneM in this.Cutscenes.Where(x => x.ID == puzzleID))
				{
					cutsceneM.ModifyCampaignItem(campaignItem);
					return;
				}
			} else
			{
				Quintessential.Logger.Log("Brimestone: " + this.name + " has no cutscenes.");
			}
			if (this.Documents is not null)
			{
                foreach (var documentM in this.Documents.Where(x => x.ID == puzzleID))
                {
                    documentM.ModifyCampaignItem(campaignItem);
                    return;
                }
            }
            else
            {
                Quintessential.Logger.Log("Brimestone: " + this.name + " has no documents.");
            }
            if (this.Puzzles is not null)
			{
                foreach (var puzzleM in this.Puzzles.Where(x => x.ID == puzzleID))
                {
                    puzzleM.ModifyCampaignItem(campaignItem);
                    return;
                }
            }
            else
            {
                Quintessential.Logger.Log("Brimestone: " + this.name + " has no puzzles.");
            }
        }
	}

	public class CreditsModel
	{
		public string PositionOffset;
		public List<List<string>> Texts;
		public Texture Background;
	}

	public const enum_129 typePuzzle = (enum_129)0;
	public const enum_129 typeCutscene = (enum_129)1;
	public const enum_129 typeDocument = (enum_129)2;
	public const enum_129 typeSolitaire = (enum_129)3;

	public class CharacterModel : ResourceModel
	{
		public string ID, Name, SmallPortrait, LargePortrait;
		public int Color;
		public bool IsOnLeft;
		internal override Dictionary<string, Texture> InitialTextureBank()
		{
			string path = "textures/portraits/";
			var portrait = class_238.field_1989.field_93;
			return new()
		{
			{"",                                null},
			{path + "anataeus_alt_large",       portrait.field_670},
			{path + "anataeus_large",           portrait.field_671},
			{path + "anataeus_shabby_large",    portrait.field_672},
			{path + "anataeus_shabby_small",    portrait.field_673},
			{path + "anataeus_small",           portrait.field_674},
			{path + "anataeus_student_small",   portrait.field_675},
			{path + "armand_large",             portrait.field_676},
			{path + "clara_large",              portrait.field_677},
			{path + "clara_small",              portrait.field_678},
			{path + "clara_tiara_small",        portrait.field_679},
			{path + "concordia_large",          portrait.field_680},
			{path + "concordia_shabby_large",   portrait.field_681},
			{path + "concordia_shabby_small",   portrait.field_682},
			{path + "concordia_small",          portrait.field_683},
			{path + "gelt_armor_small",         portrait.field_684},
			{path + "gelt_large",               portrait.field_685},
			{path + "gelt_small",               portrait.field_686},
			{path + "henley_small",             portrait.field_687},
			{path + "nils_cloak_large",         portrait.field_688},
			{path + "nils_cloak_small",         portrait.field_689},
			{path + "nils_large",               portrait.field_690},
			{path + "nils_small",               portrait.field_691},
			{path + "taros_large",              portrait.field_692},
			{path + "verrin_large",             portrait.field_693},
			{path + "verrin_shabby_large",      portrait.field_694},
		};
		}

		public class_230 FromModel()
		{
			this.EnsureTextureBankExists();
			return new class_230(
				class_134.method_253(this.Name, string.Empty),
				FetchTexture(this.LargePortrait ?? ""),
				FetchTexture(this.SmallPortrait ?? ""),
				BrimstoneAPI.HexColor(this.Color),
				this.IsOnLeft
			);
		}
	}

	public class CutsceneModel : ResourceModel
	{
		public string ID, Location, Background, Music;
		internal override Dictionary<string, Texture> InitialTextureBank()
		{
			string path1 = "textures/cinematic/backgrounds/";
			string path2 = "textures/puzzle_select/";
			var cinematic = class_238.field_1989.field_84.field_535;
			var puzzleSelect = class_238.field_1989.field_96;
			return new()
		{
			{path1 + "greathall_a", cinematic.field_536},
			{path1 + "greathall_b", cinematic.field_537},
			{path1 + "greathall_c", cinematic.field_538},
			{path1 + "tailor_a",    cinematic.field_539},
			{path1 + "tailor_b",    cinematic.field_540},
			{path1 + "tailor_c",    cinematic.field_541},
			{path1 + "workshop",    cinematic.field_542},
			{path2 + "background_0",    puzzleSelect.field_826},
			{path2 + "background_1",    puzzleSelect.field_827},
			{path2 + "background_2",    puzzleSelect.field_828},
			{path2 + "background_3",    puzzleSelect.field_829},
			{path2 + "background_4",    puzzleSelect.field_830},
			{path2 + "background_5",    puzzleSelect.field_831},
			{path2 + "background_6",    puzzleSelect.field_832},
		};
		}
		public Tuple<string, Texture> FromModel() => Tuple.Create(this.Location, FetchTexture(this.Background));
		public void ModifyCampaignItem(CampaignItem campaignItem)
		{
			campaignItem.field_2324 = BrimstoneAPI.typeCutscene;
			campaignItem.field_2328 = BrimstoneAPI.GetSongAndFanfare(this.Music).Item1;
		}
	}

	public static Color HexColor(int hex) => Color.FromHex(hex);
	public static Color White = Color.White;

	public class DocumentModel : ResourceModel
	{
		public string ID, Texture;
		public List<DrawItemModel> DrawItems;

		internal override Dictionary<string, Texture> InitialTextureBank()
		{
			string path = "textures/documents/";
			var docs = class_238.field_1989.field_85;
			return new()
		{
			{"",                        docs.field_570},
			{path + "letter_0",         docs.field_563},
			{path + "letter_0_bar",     docs.field_564},
			{path + "letter_1",         docs.field_565},
			{path + "letter_2",         docs.field_566},
			{path + "letter_3",         docs.field_567},
			{path + "letter_4",         docs.field_568},
			{path + "letter_4_overlay", docs.field_569},
			{path + "letter_5",         docs.field_570},
			{path + "letter_6",         docs.field_571},
			{path + "letter_6_overlay", docs.field_572},
			{path + "letter_7",         docs.field_573},
			{path + "letter_9",         docs.field_574},
			{path + "letter_response",  docs.field_575},
			{path + "pip",              docs.field_576},
		};
		}

		public void ModifyCampaignItem(CampaignItem campaignItem)
		{
			EnsureTextureBankExists();
			campaignItem.field_2324 = BrimstoneAPI.typeDocument;
			this.AddDocumentFromModel();
		}

		public void AddDocumentFromModel()
		{
			this.EnsureTextureBankExists();

			List<Document.DrawItem> drawItems = new();
			if (this.DrawItems != null)
			{
				foreach (var drawItem in this.DrawItems)
				{
					drawItems.Add(drawItem.FromModel());
				}
			}
			new Document(this.ID, FetchTexture(this.Texture ?? ""), drawItems);
		}



		public class DrawItemModel
		{
			public string Position, Texture, Rotation, Scale, Alpha, Font, Color, Align, LineSpacing, ColumnWidth;
			public bool Handwritten;

			public Document.DrawItem FromModel()
			{
				bool isImageItem = !string.IsNullOrEmpty(this.Texture);

				// image AND text properties
				Color color = isImageItem ? BrimstoneAPI.White : DocumentScreen.field_2410;
				if (!string.IsNullOrEmpty(this.Color)) color = BrimstoneAPI.HexColor(int.Parse(this.Color));
				Vector2 position = BrimstoneAPI.Vector2FromString(this.Position);

				if (isImageItem)
				{
					return new Document.DrawItem(
						position,
						FetchTexture(this.Texture),
						color,
						BrimstoneAPI.FloatFromString(this.Scale, 1f),
						BrimstoneAPI.FloatFromString(this.Rotation),
						BrimstoneAPI.FloatFromString(this.Alpha, 1f)
					);
				}
				else // isTextItem
				{
					return new Document.DrawItem(
						position,
						Document.DrawItem.GetFont(this.Font),
						color,
						Document.DrawItem.GetAlignment(this.Align),
						BrimstoneAPI.FloatFromString(this.LineSpacing, 1f),
						BrimstoneAPI.FloatFromString(this.ColumnWidth, float.MaxValue),
						this.Handwritten
					);
				}
			}
		}
	}

	public class Document
	{
		string ID;
#pragma warning disable IDE0052 // Remove unread private members
        readonly Texture baseTexture = null;
#pragma warning restore IDE0052 // Remove unread private members
        Action<Language, string[]> drawFunction;

		//==================================================//
		// constructors
		public Document(string documentID, Texture documentBaseTexture, Action<Language, string[]> documentDrawFunction)
		{
			this.ID = documentID;
			this.baseTexture = documentBaseTexture;
			this.drawFunction = documentDrawFunction;
			documentDatabase[this.ID] = this;
		}
		public Document(string documentID, Texture documentBaseTexture, Action<Language, Vector2, string[]> draw)
		: this(documentID, documentBaseTexture, MakeDocumentDrawFunction(documentBaseTexture, draw))
		{ }
		public Document(string documentID, Texture documentBaseTexture, List<DrawItem> drawItems)
		: this(documentID, documentBaseTexture, MakeDocumentDrawFunction(drawItems))
		{ }

		private static Action<Language, string[]> MakeDocumentDrawFunction(Texture baseTexture, Action<Language, Vector2, string[]> draw)
		{
			return (lang, textArray) => {
				Vector2 origin = class_115.field_1433 / 2 - baseTexture.field_2056.ToVector2() / 2;
				class_135.method_272(baseTexture, origin.Rounded()); //draw document
				draw(lang, origin, textArray);
			};
		}
		private static Action<Language, Vector2, string[]> MakeDocumentDrawFunction(List<DrawItem> drawItems)
		{
			return (lang, origin, textArray) => {
				int maxIndex = textArray.Length;
				int index = 0;
				for (int i = 0; i < drawItems.Count; i++)
				{
					string text = index < maxIndex ? textArray[index] : null;
					index += drawItems[i].Draw(origin, text) ? 1 : 0;
				}
			};
		}
		//==================================================//
		//global stuff
		private static Dictionary<string, Document> documentDatabase = new();

		public static void Load()
		{
			On.DocumentScreen.method_50 += DocumentScreen_Method_50;
		}

		private static void DocumentScreen_Method_50(On.DocumentScreen.orig_method_50 orig, DocumentScreen documentScreen_self, float timeDelta)
		{
			var documentScreen_dyn = new DynamicData(documentScreen_self);
			var class264 = documentScreen_dyn.Get<class_264>("field_2409");
			string documentID = class264.field_2090;
			if (!documentDatabase.ContainsKey(documentID))
			{
				orig(documentScreen_self, timeDelta);
				return;
			};
			//========================================================//
			Document document = documentDatabase[documentID];
			Language lang = class_134.field_1504;
			string[] textArray = documentScreen_dyn.Get<string[]>("field_2408");
			document.drawFunction(lang, textArray);

			// player input / scene transition
			if (Input.IsLeftClickPressed() || Input.IsSdlKeyPressed(SDL.enum_160.SDLK_ESCAPE))
			{
				GameLogic.field_2434.field_2451.method_574(class264);
				// if epilogue, find associated credits
				CreditsModel credits = null;
				string creditsModName = String.Empty;
				if (documentID.EndsWith("epilogue"))
				{
                    foreach (string modName in BrimstoneAPI.AdvancedContentDictionary.Keys)
                    {
                        if (documentID == modName + "-epilogue")
                        {
                            creditsModName = modName;
							credits = BrimstoneAPI.AdvancedContentDictionary[modName].Credits;
                            break;
                        }
                    }
                }
				if (credits is not null)
				{
					// trigger credits
					GameLogic.field_2434.method_945(new CreditsScreen(credits, creditsModName), (Maybe<class_124>)Transitions.field_4109, (Maybe<class_124>)Transitions.field_4108);
				}
				else
				{
					// unsure what this does
					class_238.field_1991.field_1875.method_28(1f); // ui_paper_back
					GameLogic.field_2434.method_949();
				}
			}
		}


		//==================================================//
		// DrawItem helper class
		public sealed class DrawItem
		{
			static Color TextColor => DocumentScreen.field_2410;
			Vector2 position = new(0f, 0f);
			Color color = TextColor;

			Texture texture = null;
			float rotation = 0f;
			float scale = 1f;
			float alpha = 1f;

			Font font = GetFont(null);
			enum_0 alignment = (enum_0)0;
			float lineSpacing = 1f;
			float columnWidth = float.MaxValue;
			bool handwritten = false;

			/////////////////////////////////////////////////////////////////////////////////////////////////
			// constructors
			public DrawItem() { }

			public DrawItem(Vector2 position, Font font, Color color, enum_0 alignment = 0, float lineSpacing = 1f, float columnWidth = float.MaxValue, bool handwritten = false)
			{
				this.position = position;
				this.font = font;
				this.color = color;
				this.alignment = alignment;
				this.lineSpacing = lineSpacing;
				this.columnWidth = columnWidth;
				this.handwritten = handwritten;
			}
			public DrawItem(Vector2 position, Font font, enum_0 alignment = 0, float lineSpacing = 1f, float columnWidth = float.MaxValue, bool handwritten = false)
			: this(position, font, TextColor, alignment, lineSpacing, columnWidth, handwritten)
			{ }

			public DrawItem(Vector2 position, Texture texture, Color color, float scale = 1f, float rotation = 0f, float alpha = 1f)
			{
				this.position = position;
				this.texture = texture;
				this.color = color;
				this.rotation = rotation;
				this.scale = scale;
				this.alpha = alpha;
			}
			public DrawItem(Vector2 position, Texture texture, float scale = 1f, float rotation = 0f, float alpha = 1f)
			: this(position, texture, Color.White, scale, rotation, alpha)
			{ }

			/////////////////////////////////////////////////////////////////////////////////////////////////
			// public helpers
			public static enum_0 GetAlignment(string align)
			{
				align ??= "left";
				switch (align.ToLower())
				{
					default: return (enum_0)0;
					case "center": return (enum_0)1;
					case "right": return (enum_0)2;
				}
			}

			public static Font GetFont(string font)
			{
				string defaultFont = "cormorant 15";
				font ??= defaultFont;
				Dictionary<string, Font> FontBank = new()
			{
				{"crimson 21", class_238.field_1990.field_2146},
				{"crimson 16.5", class_238.field_1990.field_2145},
				{"crimson 15", class_238.field_1990.field_2144},
				{"crimson 13", class_238.field_1990.field_2143},
				{"crimson 12", class_238.field_1990.field_2142},
				{"crimson 10.5", class_238.field_1990.field_2141},
				{"crimson 9.75", class_238.field_1990.field_2140},

				{"cinzel 21", class_238.field_1990.field_2147},
				{"cormorant 22.5", class_238.field_1990.field_2148},
				{"cormorant 18", class_238.field_1990.field_2149},
				{"cormorant 15", class_238.field_1990.field_2150},
				{"cormorant 12.75", class_238.field_1990.field_2151},
				{"cormorant 11", class_238.field_1990.field_2152},

				{"reenie 17.25", class_238.field_1990.field_2153},
				{"naver 17.25", class_238.field_1990.field_2154},
			};
				return FontBank.ContainsKey(font) ? FontBank[font] : FontBank[defaultFont];
			}

			public static Font GetHandwrittenFont() => GetFont(class_134.field_1504 == Language.Korean ? "naver 17.25" : "reenie 17.25");

			public bool Draw(Vector2 origin, string text)
			{
				// returns true if it drew as text
				if (this.texture != null)
				{
					Vector2 textureDimensions = this.texture.field_2056.ToVector2();
					Matrix4 Translation = Matrix4.method_1070((origin + this.position).ToVector3(0.0f));
					Matrix4 Rotation = Matrix4.method_1073(this.rotation);
					Matrix4 Scaling = Matrix4.method_1074((textureDimensions * this.scale).ToVector3(0.0f));
					Matrix4 Transformation = Translation * Rotation * Scaling; // order is important
					class_135.method_262(this.texture, this.color.WithAlpha(this.alpha), Transformation);
				}
				else if (!string.IsNullOrEmpty(text))
				{
					if (handwritten) font = GetHandwrittenFont();
					DrawText(text, origin + position, font, color, alignment, lineSpacing, columnWidth, float.MaxValue, int.MaxValue, true);
					return true;
				}
				return false;
			}

			public static Bounds2 DrawText(
			string text,
			Vector2 position,
			Font font,
			Color color,
			enum_0 alignment = (enum_0)0,
			float lineSpacing = 1f,
			float columnWidth = float.MaxValue,
			float truncateWidth = float.MaxValue,
			int maxCharactersDrawn = int.MaxValue,
			bool returnBoundingBox = false)
			{
				return class_135.method_290(text, position, font, color, alignment, lineSpacing,
					0.6f/*default for documents, not sure what it does*/,
					columnWidth,
					truncateWidth,
					0/*default for documents, not sure what it does*/,
					new Color()/*default for documents, not sure what it does*/,
					null/*default texture for documents, changing it seems to affect the color somehow, not sure what it actually does*/,
					Math.Max(-1, maxCharactersDrawn - 1),
					returnBoundingBox,
					true/*false will hide the text - however, this can be done by setting maxCharactersDrawn == 0*/
				);
			}
		}
	}
	public class PuzzleModel : ResourceModel
	{
		public string ID, Music;
		public TipModel Tip = null;
		public CabinetModel Cabinet;
		public bool NoStoryPanel = false;
		public Dictionary<int, string> JournalPreview;

		internal override Dictionary<string, Texture> InitialTextureBank()
		{
			string prodPath = "textures/pipelines/";
			var prods = class_238.field_1989.field_92;
			return new()
		{
			{prodPath + "aether_overlay_bottom",    prods.field_390},
			{prodPath + "aether_overlay_middle",    prods.field_391},
			{prodPath + "aether_overlay_top",       prods.field_392},
			{prodPath + "amaro_overlay_bottom",     prods.field_393},
			{prodPath + "amaro_overlay_top",        prods.field_394},
			{prodPath + "edge_overlay_left",        prods.field_395},
			{prodPath + "edge_overlay_right",       prods.field_396},
			{prodPath + "solvent_overlay",          prods.field_397},
		};
		}
		public void ModifyCampaignItem(CampaignItem campaignItem)
		{
			this.EnsureTextureBankExists();;
			Tuple<Song, Sound> musicPair = BrimstoneAPI.GetSongAndFanfare(this.Music);
			campaignItem.field_2328 = musicPair.Item1;
			campaignItem.field_2329 = musicPair.Item2;

			Puzzle puzzle = campaignItem.field_2325.method_1087();
			this.ModifyCampaignItem(puzzle);
		}
		public void ModifyCampaignItem(Puzzle puzzle)
		{
			if (this.Tip != null) puzzle.field_2769 = this.Tip.FromModel();
		}

		public Dictionary<int, Vector2> GetJournalPreview()
		{
			Dictionary<int, Vector2> ret = new();
			if (this.JournalPreview != null)
			{
				foreach (var kvp in this.JournalPreview)
				{
					ret.Add(kvp.Key, BrimstoneAPI.Vector2FromString(kvp.Value));
				}
			}
			return ret;
		}

		//////////////////////////////////////////////////
		public class TipModel
		{
#pragma warning disable CS0649
			public string ID, Title, Description, Texture, Solution, SolutionOffset;
			public Tip FromModel()
			{
				Maybe<Texture> maybeImage = !string.IsNullOrEmpty(this.Texture) ? FetchTexture(this.Texture) : (Maybe<Texture>)struct_18.field_1431;

				return new Tip()
				{
					field_1899 = this.ID,
					field_1900 = class_134.method_253(this.Title ?? "<Untitled Tip>", string.Empty),
					field_1901 = class_134.method_253(this.Description ?? "<Description Missing>", string.Empty),
					field_1902 = this.Solution ?? "speedbonder",
					field_1903 = maybeImage,
					field_1904 = BrimstoneAPI.Vector2FromString(this.SolutionOffset),
				};
			}
		}

		public class CabinetModel
		{
			public List<OverlayModel> Overlays;

			public List<Tuple<Texture, Vector2>> FetchOverlays()
			{
				List<Tuple<Texture, Vector2>> ret = new();
				foreach (var overlay in Overlays)
				{
					ret.Add(overlay.FromModel());
				}
				return ret;
			}

			public class OverlayModel
			{
#pragma warning disable CS0649
				public string Texture, Position;
				public Tuple<Texture, Vector2> FromModel()
				{
					return Tuple.Create(FetchTexture(this.Texture), BrimstoneAPI.Vector2FromString(this.Position));
				}
#pragma warning restore CS0649
			}
		}
	}

    public sealed class CreditsScreen : IScreen
    {
		public CreditsScreen(CreditsModel cm, string mN) {
			this.creditsModal = cm;
			this.modName = mN;
		}

        private readonly CreditsModel creditsModal;
		private readonly string modName;

        private float timer;

        static class_124 TransitionInstant = new()
        {
            field_1458 = 0f,
            field_1459 = Transitions.field_4108.field_1459,
            field_1460 = Transitions.field_4108.field_1460
        };
        private static bool transitioningBackToMenu;
        public static Vector2 ScreenResolution => class_115.field_1433;
		private void SetCreditsSeen() => GameLogic.field_2434.field_2451.field_1929.method_858(modName + "-CreditsSeen", true.method_453());
        private bool GetCreditsSeen() => GameLogic.field_2434.field_2451.field_1929.method_862(new delegate_384<bool>(bool.TryParse), modName + "-CreditsSeen").method_1090(false);
        private void ExitCredits()
        {
            SetCreditsSeen();
            // get rid of all documents that might be waiting for us after we exit the credits - not sure if it's needed
            GameLogic.field_2434.method_951<DocumentScreen>();
            // transition out
            GameLogic.field_2434.method_947((Maybe<class_124>)TransitionInstant, (Maybe<class_124>)TransitionInstant);
            // only trigger this codeblock once
            transitioningBackToMenu = true;
        }

        static Bounds2 DrawCreditText(string str, Vector2 pos, bool bigFont, float alpha)
        {
            Font crimson_21 = class_238.field_1990.field_2146;
            Font crimson_16_5 = class_238.field_1990.field_2145;
            Font font = bigFont ? crimson_21 : crimson_16_5;
            return class_135.method_290(str, pos, font, class_181.field_1718.WithAlpha(alpha), (enum_0)1, 1f, 0.6f, float.MaxValue, float.MaxValue, 0, new Color(), null, int.MaxValue, false, true);
        }

        public void method_47(bool param_4523)
        {
            transitioningBackToMenu = false;
            var creditsSong = class_238.field_1992.field_973;
            creditsSong.field_1741 = 1.0;
            GameLogic.field_2434.field_2443.method_673(creditsSong);
        }

        public void method_48() { }

        public bool method_1037() => true;

        public void method_50(float timeDelta)
        {
            timer += timeDelta;

            Texture background = creditsModal.Background;
            float scalar = ScreenResolution.Y / background.field_2056.Y;
            Vector2 normedSize = background.field_2056.ToVector2() * scalar;
            class_135.method_279(Color.Black, Vector2.Zero, ScreenResolution);
            class_135.method_263(background, Color.White, ScreenResolution / 2 - normedSize / 2, normedSize);

            scalar = ScreenResolution.Y / 2160f;
            Vector2 textPosition = ScreenResolution / 2 + BrimstoneAPI.Vector2FromString(creditsModal.PositionOffset) * scalar;

            class_310 class310 = new class_310();
            class310.IncrementTimer(2f);
            var credits = creditsModal.Texts;
            foreach (var entry in credits)
            {
                AddCreditFrame(class310, textPosition, entry);
            }
            class310.actions.ForEach(x => x());

            if (transitioningBackToMenu) return;

            if ((GetCreditsSeen() && Input.IsSdlKeyPressed(SDL.enum_160.SDLK_ESCAPE)) || timer >= class310.time)
            {
                ExitCredits();
            }
        }

        public sealed class class_310
        {
            public float time;
            public List<Action> actions = new();
            public void IncrementTimer(float amount) => this.time += amount;
        }
        private void AddCreditFrame(class_310 class310, Vector2 position, List<string> credit)
        {
            float fadeTime = 0.5f;
            float gapTime = 0.5f;
            float drawfullTime = 5f;

            float num = fadeTime + drawfullTime + fadeTime + gapTime;
            float time = class310.time;
            if (timer >= time && timer < time + num)
            {
                float drawTime = 1f;
                if (timer < time + fadeTime)
                    drawTime = class_162.method_416(timer, time, time + fadeTime, 0f, 1f);
                else if (timer > time + fadeTime + drawfullTime)
                    drawTime = class_162.method_416(timer, time + fadeTime + drawfullTime, time + fadeTime + drawfullTime + fadeTime, 1f, 0f);
                class310.actions.Add(() => new CreditFrame(position, credit).Draw(drawTime * 0.9f));
            }
            class310.IncrementTimer(num);
        }
        public sealed class CreditFrame
        {
            public Vector2 origin;
            public List<string> texts;
            Vector2 nextLineOffset = new(0f, -32f);
            Vector2 initialOffset = new(0f, 13f);

            public CreditFrame(Vector2 origin, List<string> texts)
            {
                this.origin = origin;
                this.texts = texts;
            }

            internal void Draw(float alpha)
            {
                Vector2 pos = origin + initialOffset * (texts.Count - 1);
                for (int i = 0; i < texts.Count; i++)
                {
                    DrawCreditText(texts[i], pos, i == 0, alpha);
                    pos += nextLineOffset;
                }
            }
        }
    }
}