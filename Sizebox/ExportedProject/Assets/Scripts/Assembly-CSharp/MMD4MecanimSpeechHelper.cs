using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MMD4MecanimSpeechHelper : MMD4MecanimMorphHelper
{
	private struct MorphData
	{
		public char morphChar;

		public string morphName;

		public float morphLength;
	}

	private class PlayingData
	{
		public AudioClip audioClip;

		public float playingLength;

		public float playingTime;

		public int morphPos = -1;

		public float morphTime;

		public List<MorphData> morphDataList;
	}

	public AudioClip speechAudioClip;

	public string speechMorphText;

	public float elementLength;

	public float consonantLength;

	private static readonly float defaultElementLength = 0.2f;

	private static readonly float defaultConsonantLength = 0.1f;

	private static List<string> hiraMorphNameList = new List<string> { "あ", "い", "う", "え", "お" };

	private static Dictionary<string, string> englishPhraseDictionary = new Dictionary<string, string>
	{
		{ "ch", "i" },
		{ "ck", "u" },
		{ "cs", "uu" },
		{ "dy", "i-" },
		{ "ff", "u" },
		{ "ght", "o" },
		{ "kni", "ai" },
		{ "llo", "o-" },
		{ "phy", "i" },
		{ "ss", "/u" },
		{ "ty", "i-" },
		{ "th", "u" },
		{ "ca", "a" },
		{ "ci", "i" },
		{ "cu", "u" },
		{ "ce", "e" },
		{ "co", "o" },
		{ "qa", "a" },
		{ "qi", "i" },
		{ "qu", "u" },
		{ "qe", "e" },
		{ "qo", "o" },
		{ "ka", "a" },
		{ "ki", "i" },
		{ "ku", "u" },
		{ "ke", "e" },
		{ "ko", "o" },
		{ "ga", "a" },
		{ "gi", "ai" },
		{ "gu", "u" },
		{ "ge", "e" },
		{ "go", "o-" },
		{ "sa", "^a" },
		{ "si", "i/" },
		{ "su", "u" },
		{ "se", "^u" },
		{ "so", "^o" },
		{ "sha", "^a" },
		{ "shi", "i" },
		{ "shu", "u" },
		{ "she", "^i-" },
		{ "sho", "^o" },
		{ "xa", "^a" },
		{ "xi", "^i" },
		{ "xu", "^u" },
		{ "xe", "^e" },
		{ "xo", "^o" },
		{ "za", "^a" },
		{ "zi", "^i" },
		{ "zu", "^u" },
		{ "ze", "^e" },
		{ "zo", "^o" },
		{ "ja", "^a" },
		{ "ji", "i" },
		{ "ju", "u" },
		{ "je", "^e" },
		{ "jo", "^o" },
		{ "ta", "a" },
		{ "ti", "i" },
		{ "tu", "u" },
		{ "te", "^e" },
		{ "to", "u-" },
		{ "fa", "^a" },
		{ "fi", "^i" },
		{ "fu", "u" },
		{ "fe", "^e" },
		{ "fo", "^o" },
		{ "da", "a" },
		{ "di", "i" },
		{ "du", "u" },
		{ "de", "e" },
		{ "do", "u-" },
		{ "na", "^a" },
		{ "ni", "ai" },
		{ "nu", "u" },
		{ "ne", "u" },
		{ "no", "^o" },
		{ "nya", "^a" },
		{ "nyi", "i" },
		{ "nyu", "u" },
		{ "nye", "e" },
		{ "nyo", "^o" },
		{ "ha", "a" },
		{ "hi", "ai" },
		{ "hu", "u" },
		{ "he", "e" },
		{ "ho", "o" },
		{ "pa", "`a" },
		{ "pi", "`i" },
		{ "pu", "`u" },
		{ "pe", "`e" },
		{ "po", "`o" },
		{ "ba", "`a" },
		{ "bi", "`i" },
		{ "bu", "`u" },
		{ "be", "i" },
		{ "bo", "o" },
		{ "ma", "`a" },
		{ "mi", "`i" },
		{ "mu", "`u" },
		{ "me", "`e" },
		{ "mo", "`a" },
		{ "ya", "a" },
		{ "yi", "i" },
		{ "yu", "u" },
		{ "ye", "e" },
		{ "yo", "o" },
		{ "ra", "o" },
		{ "ri", "i" },
		{ "ru", "a" },
		{ "re", "i" },
		{ "ro", "o-" },
		{ "la", "a" },
		{ "li", "i" },
		{ "lu", "u" },
		{ "le", "i" },
		{ "lo", "a" },
		{ "wa", "^a" },
		{ "wi", "^i-" },
		{ "wu", "u-" },
		{ "we", "^i-" },
		{ "wo", "^o-" },
		{ "va", "`a" },
		{ "vi", "`i" },
		{ "vu", "`u" },
		{ "ve", "`u" },
		{ "vo", "`a" }
	};

	private static Dictionary<char, string> englishPronunDictionary = new Dictionary<char, string>
	{
		{ 'a', "a" },
		{ 'i', "i" },
		{ 'u', "u" },
		{ 'e', "e" },
		{ 'o', "o" },
		{ 'n', "n" },
		{ 'b', "u" },
		{ 'c', "u" },
		{ 'd', "o" },
		{ 'f', "u" },
		{ 'g', "u" },
		{ 'h', "u" },
		{ 'j', "i" },
		{ 'k', "u" },
		{ 'l', "u" },
		{ 'm', "u" },
		{ 'p', "u" },
		{ 'q', "u" },
		{ 'r', "a" },
		{ 's', "u" },
		{ 't', "o" },
		{ 'v', "u" },
		{ 'w', "u" },
		{ 'x', "uu" },
		{ 'y', "i" },
		{ 'z', "u" }
	};

	private static Dictionary<char, string> japanesePostPhraseDictionary = new Dictionary<char, string>
	{
		{ 'ぁ', "a" },
		{ 'ぃ', "i" },
		{ 'ぅ', "u" },
		{ 'ぇ', "e" },
		{ 'ぉ', "o" },
		{ 'ァ', "a" },
		{ 'ィ', "i" },
		{ 'ゥ', "u" },
		{ 'ェ', "e" },
		{ 'ォ', "o" }
	};

	private static Dictionary<string, string> japanesePhraseDictionary = new Dictionary<string, string>
	{
		{ "きぁ", "^a" },
		{ "きぃ", "i" },
		{ "きぅ", "u" },
		{ "きぇ", "^e" },
		{ "きぉ", "^o" },
		{ "しぁ", "^a" },
		{ "しぃ", "i" },
		{ "しぅ", "u" },
		{ "しぇ", "^e" },
		{ "しぉ", "^o" },
		{ "にぁ", "^a" },
		{ "にぃ", "i" },
		{ "にぅ", "u" },
		{ "にぇ", "^e" },
		{ "にぉ", "^o" },
		{ "ひぁ", "^a" },
		{ "ひぃ", "i" },
		{ "ひぅ", "u" },
		{ "ひぇ", "^e" },
		{ "ひぉ", "^o" },
		{ "びぁ", "`a" },
		{ "びぃ", "`i" },
		{ "びぅ", "`u" },
		{ "びぇ", "`e" },
		{ "びぉ", "`o" },
		{ "ぴぁ", "`a" },
		{ "ぴぃ", "`i" },
		{ "ぴぅ", "`u" },
		{ "ぴぇ", "`e" },
		{ "ぴぉ", "`o" },
		{ "まぁ", "`a" },
		{ "まぃ", "`i" },
		{ "まぅ", "`u" },
		{ "まぇ", "`e" },
		{ "まぉ", "`o" },
		{ "りぁ", "^a" },
		{ "りぃ", "i" },
		{ "りぅ", "u" },
		{ "りぇ", "^e" },
		{ "りぉ", "^o" },
		{ "キァ", "^a" },
		{ "キィ", "i" },
		{ "キゥ", "u" },
		{ "キェ", "^e" },
		{ "キォ", "^o" },
		{ "シァ", "^a" },
		{ "シィ", "i" },
		{ "シゥ", "u" },
		{ "シェ", "^e" },
		{ "シォ", "^o" },
		{ "ニァ", "^a" },
		{ "ニィ", "i" },
		{ "ニゥ", "u" },
		{ "ニェ", "^e" },
		{ "ニォ", "^o" },
		{ "ヒァ", "`a" },
		{ "ヒィ", "`i" },
		{ "ヒゥ", "`u" },
		{ "ヒェ", "`e" },
		{ "ヒォ", "`o" },
		{ "ビァ", "`a" },
		{ "ビィ", "`i" },
		{ "ビゥ", "`u" },
		{ "ビェ", "`e" },
		{ "ビォ", "`o" },
		{ "ピァ", "`a" },
		{ "ピィ", "`i" },
		{ "ピゥ", "`u" },
		{ "ピェ", "`e" },
		{ "ピォ", "`o" },
		{ "ミァ", "`a" },
		{ "ミィ", "`i" },
		{ "ミゥ", "`u" },
		{ "ミェ", "`e" },
		{ "ミォ", "`o" },
		{ "リァ", "^a" },
		{ "リィ", "i" },
		{ "リゥ", "u" },
		{ "リェ", "^e" },
		{ "リォ", "^o" },
		{ "ヴァ", "`a" },
		{ "ヴィ", "`i" },
		{ "ヴゥ", "`u" },
		{ "ヴェ", "`e" },
		{ "ヴォ", "`o" }
	};

	private static Dictionary<char, string> punctuatDictionary = new Dictionary<char, string>
	{
		{ '.', " " },
		{ ',', " " },
		{ '!', " " },
		{ '?', " " },
		{ '．', " " },
		{ '，', " " },
		{ '！', " " },
		{ '？', " " },
		{ '、', " " },
		{ '。', " " }
	};

	private static Dictionary<char, string> japanesePronunDictionary = new Dictionary<char, string>
	{
		{ 'ー', "-" },
		{ '―', "-" },
		{ '〜', "-" },
		{ 'ぁ', "a" },
		{ 'ぃ', "i" },
		{ 'ぅ', "u" },
		{ 'ぇ', "e" },
		{ 'ぉ', "o" },
		{ 'あ', "a" },
		{ 'い', "i" },
		{ 'う', "u" },
		{ 'え', "e" },
		{ 'お', "o" },
		{ 'か', "a" },
		{ 'き', "i" },
		{ 'く', "u" },
		{ 'け', "e" },
		{ 'こ', "o" },
		{ 'が', "a" },
		{ 'ぎ', "i" },
		{ 'ぐ', "u" },
		{ 'げ', "e" },
		{ 'ご', "o" },
		{ 'さ', "^a" },
		{ 'し', "i" },
		{ 'す', "u" },
		{ 'せ', "^e" },
		{ 'そ', "^o" },
		{ 'ざ', "^a" },
		{ 'じ', "i" },
		{ 'ず', "u" },
		{ 'ぜ', "^e" },
		{ 'ぞ', "^o" },
		{ 'た', "^a" },
		{ 'ち', "i" },
		{ 'つ', "u" },
		{ 'て', "^e" },
		{ 'と', "^o" },
		{ 'だ', "^a" },
		{ 'ぢ', "i" },
		{ 'づ', "u" },
		{ 'で', "^e" },
		{ 'ど', "^o" },
		{ 'っ', "/" },
		{ 'な', "^a" },
		{ 'に', "i" },
		{ 'ぬ', "u" },
		{ 'ね', "^e" },
		{ 'の', "^o" },
		{ 'は', "a" },
		{ 'ひ', "i" },
		{ 'ふ', "u" },
		{ 'へ', "e" },
		{ 'ほ', "o" },
		{ 'ば', "`a" },
		{ 'び', "`i" },
		{ 'ぶ', "`u" },
		{ 'べ', "`e" },
		{ 'ぼ', "`o" },
		{ 'ぱ', "`a" },
		{ 'ぴ', "`i" },
		{ 'ぷ', "`u" },
		{ 'ぺ', "`e" },
		{ 'ぽ', "`o" },
		{ 'ま', "`a" },
		{ 'み', "`i" },
		{ 'む', "`u" },
		{ 'め', "`e" },
		{ 'も', "`o" },
		{ 'ゃ', "a" },
		{ 'ゅ', "u" },
		{ 'ょ', "o" },
		{ 'や', "a" },
		{ 'ゆ', "u" },
		{ 'よ', "o" },
		{ 'ら', "^a" },
		{ 'り', "i" },
		{ 'る', "u" },
		{ 'れ', "^e" },
		{ 'ろ', "^o" },
		{ 'ゎ', "a" },
		{ 'ゐ', "i" },
		{ 'を', "o" },
		{ 'わ', "^a" },
		{ 'ゑ', "e" },
		{ 'ん', "n" },
		{ 'ァ', "a" },
		{ 'ィ', "i" },
		{ 'ゥ', "u" },
		{ 'ェ', "e" },
		{ 'ォ', "o" },
		{ 'ア', "a" },
		{ 'イ', "i" },
		{ 'ウ', "u" },
		{ 'エ', "e" },
		{ 'オ', "o" },
		{ 'カ', "a" },
		{ 'キ', "i" },
		{ 'ク', "u" },
		{ 'ケ', "e" },
		{ 'コ', "o" },
		{ 'ガ', "a" },
		{ 'ギ', "i" },
		{ 'グ', "u" },
		{ 'ゲ', "e" },
		{ 'ゴ', "o" },
		{ 'サ', "^a" },
		{ 'シ', "i" },
		{ 'ス', "u" },
		{ 'セ', "^e" },
		{ 'ソ', "^o" },
		{ 'ザ', "^a" },
		{ 'ジ', "i" },
		{ 'ズ', "u" },
		{ 'ゼ', "^e" },
		{ 'ゾ', "^o" },
		{ 'タ', "^a" },
		{ 'チ', "i" },
		{ 'ツ', "u" },
		{ 'テ', "^e" },
		{ 'ト', "^o" },
		{ 'ダ', "^a" },
		{ 'ヂ', "i" },
		{ 'ヅ', "u" },
		{ 'デ', "^e" },
		{ 'ド', "^o" },
		{ 'ッ', "/" },
		{ 'ナ', "^a" },
		{ 'ニ', "i" },
		{ 'ヌ', "u" },
		{ 'ネ', "^e" },
		{ 'ノ', "^o" },
		{ 'ハ', "a" },
		{ 'ヒ', "i" },
		{ 'フ', "u" },
		{ 'ヘ', "e" },
		{ 'ホ', "o" },
		{ 'バ', "`a" },
		{ 'ビ', "`i" },
		{ 'ブ', "`u" },
		{ 'ベ', "`e" },
		{ 'ボ', "`o" },
		{ 'パ', "`a" },
		{ 'ピ', "`i" },
		{ 'プ', "`u" },
		{ 'ペ', "`e" },
		{ 'ポ', "`o" },
		{ 'マ', "`a" },
		{ 'ミ', "`i" },
		{ 'ム', "`u" },
		{ 'メ', "`e" },
		{ 'モ', "`o" },
		{ 'ャ', "a" },
		{ 'ュ', "u" },
		{ 'ョ', "o" },
		{ 'ヤ', "a" },
		{ 'ユ', "u" },
		{ 'ヨ', "o" },
		{ 'ラ', "^a" },
		{ 'リ', "i" },
		{ 'ル', "u" },
		{ 'レ', "^e" },
		{ 'ロ', "^o" },
		{ 'ヮ', "a" },
		{ 'ヰ', "i" },
		{ 'ヲ', "o" },
		{ 'ワ', "^a" },
		{ 'ヱ', "e" },
		{ 'ン', "n" },
		{ 'ヴ', "`u" },
		{ 'ヵ', "a" },
		{ 'ヶ', "e" }
	};

	private AudioSource _audioSource;

	private List<PlayingData> _playingDataList = new List<PlayingData>();

	private int _playingPos;

	private bool _isPlayingAudioClip;

	private bool _isPlayingMorph;

	private int _validateMorphBits;

	public override bool isProcessing
	{
		get
		{
			if (speechAudioClip != null || !string.IsNullOrEmpty(speechMorphText))
			{
				return true;
			}
			if (_playingDataList != null && _playingPos < _playingDataList.Count)
			{
				return true;
			}
			return false;
		}
	}

	public override bool isAnimating
	{
		get
		{
			if (base.isAnimating)
			{
				return true;
			}
			if (speechAudioClip != null || !string.IsNullOrEmpty(speechMorphText))
			{
				return true;
			}
			if (_playingDataList != null && _playingPos < _playingDataList.Count)
			{
				return true;
			}
			return false;
		}
	}

	public float GetElementLength()
	{
		if (!(elementLength > 0f))
		{
			return defaultElementLength;
		}
		return elementLength;
	}

	public float GetConsonantLength()
	{
		if (!(consonantLength > 0f))
		{
			return defaultConsonantLength;
		}
		return consonantLength;
	}

	private static bool _IsEscape(char ch)
	{
		if (ch != '^' && ch != '`' && ch != '/' && ch != '-')
		{
			return ch == '~';
		}
		return true;
	}

	protected override void Start()
	{
		base.Start();
		if (_model != null)
		{
			_audioSource = _model.GetAudioSource();
		}
	}

	protected override void Update()
	{
		_UpdateSpeech(Time.deltaTime);
		base.Update();
	}

	public override void ForceUpdate()
	{
		_UpdateSpeech(0f);
		base.ForceUpdate();
	}

	public void ResetSpeech()
	{
		speechAudioClip = null;
		speechMorphText = "";
		if (_playingDataList.Count <= 0)
		{
			return;
		}
		if (_isPlayingAudioClip)
		{
			_isPlayingAudioClip = false;
			if (_audioSource != null)
			{
				_audioSource.Stop();
				_audioSource.clip = null;
			}
		}
		if (_isPlayingMorph)
		{
			_isPlayingMorph = false;
			morphName = "";
			morphWeight = 0f;
		}
		_playingDataList.Clear();
		_playingPos = 0;
		ForceUpdate();
	}

	private void _UpdateSpeech(float deltaTime)
	{
		_UpdatePlayingSpeech();
		if (_playingDataList == null || _playingDataList.Count == 0)
		{
			return;
		}
		if (_playingPos >= _playingDataList.Count)
		{
			if (_isPlayingAudioClip)
			{
				_isPlayingAudioClip = false;
				if (_audioSource != null)
				{
					_audioSource.Stop();
					_audioSource.clip = null;
				}
			}
			if (_isPlayingMorph)
			{
				_isPlayingMorph = false;
				morphName = "";
				morphWeight = 0f;
			}
			_playingDataList.Clear();
			_playingPos = 0;
			return;
		}
		PlayingData playingData = _playingDataList[_playingPos];
		while (playingData.playingTime >= playingData.playingLength)
		{
			if (_isPlayingAudioClip)
			{
				_isPlayingAudioClip = false;
				if (_audioSource != null)
				{
					_audioSource.Stop();
					_audioSource.clip = null;
				}
			}
			float playingTime = playingData.playingTime - playingData.playingLength;
			if (++_playingPos >= _playingDataList.Count)
			{
				if (_isPlayingMorph)
				{
					_isPlayingMorph = false;
					morphName = "";
					morphWeight = 0f;
				}
				return;
			}
			playingData = _playingDataList[_playingPos];
			playingData.playingTime = playingTime;
		}
		if (playingData.morphPos < 0)
		{
			playingData.morphPos = 0;
			if (playingData.audioClip != null)
			{
				_isPlayingAudioClip = true;
				if (_audioSource != null)
				{
					_audioSource.clip = playingData.audioClip;
					_audioSource.Play();
				}
			}
			if (playingData.morphDataList != null && playingData.morphPos < playingData.morphDataList.Count)
			{
				_isPlayingMorph = true;
				_UpdateMorph(playingData.morphDataList[playingData.morphPos].morphName);
			}
			else if (_isPlayingMorph)
			{
				_isPlayingMorph = false;
				morphName = "";
				morphWeight = 0f;
			}
		}
		bool flag = false;
		if (_isPlayingAudioClip && _audioSource != null)
		{
			if (_audioSource.isPlaying)
			{
				playingData.playingTime = _audioSource.time;
				flag = true;
			}
			else if (playingData.playingTime < playingData.playingLength)
			{
				playingData.playingTime = playingData.playingLength;
				flag = true;
			}
		}
		if (playingData.morphTime < playingData.playingTime && playingData.morphDataList != null)
		{
			float num = playingData.playingTime - playingData.morphTime;
			int morphPos = playingData.morphPos;
			while (playingData.morphPos < playingData.morphDataList.Count)
			{
				float morphLength = playingData.morphDataList[playingData.morphPos].morphLength;
				if (morphLength >= num)
				{
					break;
				}
				playingData.morphTime += morphLength;
				num -= morphLength;
				playingData.morphPos++;
			}
			if (morphPos != playingData.morphPos && playingData.morphPos < playingData.morphDataList.Count)
			{
				_UpdateMorph(playingData.morphDataList[playingData.morphPos].morphName);
			}
		}
		if (!flag)
		{
			playingData.playingTime += deltaTime;
		}
	}

	private void _UpdateMorph(string morphName)
	{
		if (_validateMorphBits == 0)
		{
			MMD4MecanimModel component = GetComponent<MMD4MecanimModel>();
			if (component != null && hiraMorphNameList != null)
			{
				for (int i = 0; i < hiraMorphNameList.Count; i++)
				{
					if (component.GetMorph(hiraMorphNameList[i]) != null)
					{
						_validateMorphBits |= 1 << i;
					}
				}
			}
			if (_validateMorphBits == 0)
			{
				_validateMorphBits = 31;
			}
		}
		float num = 0f;
		if (!string.IsNullOrEmpty(morphName))
		{
			num = 1f;
			if (hiraMorphNameList != null)
			{
				if (morphName == hiraMorphNameList[1] && (_validateMorphBits & 2) == 0)
				{
					morphName = hiraMorphNameList[0];
					num = 0.5f;
				}
				else if (morphName == hiraMorphNameList[2] && (_validateMorphBits & 4) == 0)
				{
					morphName = hiraMorphNameList[0];
					num = 0.3f;
				}
				else if (morphName == hiraMorphNameList[3] && (_validateMorphBits & 8) == 0)
				{
					morphName = hiraMorphNameList[0];
					num = 0.5f;
				}
				else if (morphName == hiraMorphNameList[4] && (_validateMorphBits & 0x10) == 0)
				{
					morphName = hiraMorphNameList[0];
					num = 0.8f;
				}
			}
		}
		base.morphName = morphName;
		morphWeight = num;
	}

	private void _UpdatePlayingSpeech()
	{
		if (!(speechAudioClip != null) && string.IsNullOrEmpty(speechMorphText))
		{
			return;
		}
		PlayingData playingData = _ParsePlayingData(speechAudioClip, speechMorphText);
		speechAudioClip = null;
		speechMorphText = "";
		_playingDataList.Clear();
		_playingPos = 0;
		if (_isPlayingAudioClip)
		{
			_isPlayingAudioClip = false;
			if (_audioSource != null)
			{
				_audioSource.Stop();
				_audioSource.clip = null;
			}
		}
		if (playingData != null)
		{
			_playingDataList.Add(playingData);
		}
	}

	private PlayingData _ParsePlayingData(AudioClip audioClip, string morphText)
	{
		PlayingData playingData = new PlayingData();
		playingData.audioClip = audioClip;
		playingData.playingLength = 0f;
		playingData.playingTime = 0f;
		playingData.morphPos = -1;
		playingData.morphTime = 0f;
		if (!string.IsNullOrEmpty(morphText))
		{
			playingData.morphDataList = _ParseMorphText(morphText);
		}
		else
		{
			playingData.morphDataList = _ParseMorphText(Path.GetFileNameWithoutExtension(audioClip.name));
		}
		if (playingData.morphDataList != null)
		{
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < playingData.morphDataList.Count; i++)
			{
				char morphChar = playingData.morphDataList[i].morphChar;
				if (playingData.morphDataList[i].morphLength == 0f)
				{
					if (morphChar != '^' && morphChar != '`')
					{
						num2++;
					}
				}
				else
				{
					num += playingData.morphDataList[i].morphLength;
				}
			}
			float num3 = GetElementLength();
			if (audioClip != null)
			{
				playingData.playingLength = audioClip.length;
				if (playingData.playingLength <= num)
				{
					playingData.playingLength = num;
					num3 = 0f;
				}
				else if (num2 > 0)
				{
					num3 = (playingData.playingLength - num) / (float)num2;
				}
			}
			else
			{
				playingData.playingLength = num + num3 * (float)num2;
			}
			if (num3 > 0f)
			{
				for (int j = 0; j < playingData.morphDataList.Count; j++)
				{
					char morphChar2 = playingData.morphDataList[j].morphChar;
					if (playingData.morphDataList[j].morphLength == 0f && morphChar2 != '^' && morphChar2 != '`')
					{
						MorphData value = playingData.morphDataList[j];
						value.morphLength = num3;
						playingData.morphDataList[j] = value;
					}
				}
				float num4 = GetConsonantLength();
				for (int k = 0; k < playingData.morphDataList.Count; k++)
				{
					char morphChar3 = playingData.morphDataList[k].morphChar;
					if (playingData.morphDataList[k].morphLength != 0f || (morphChar3 != '^' && morphChar3 != '`'))
					{
						continue;
					}
					int index = k;
					for (k++; k < playingData.morphDataList.Count; k++)
					{
						morphChar3 = playingData.morphDataList[k].morphChar;
						if (morphChar3 != '^' && morphChar3 != '`')
						{
							float morphLength = playingData.morphDataList[k].morphLength;
							float num5 = 0f;
							if (num4 * 2f <= morphLength)
							{
								MorphData value2 = playingData.morphDataList[k];
								value2.morphLength = morphLength - num4;
								playingData.morphDataList[k] = value2;
								num5 = num4;
							}
							else
							{
								MorphData value3 = playingData.morphDataList[k];
								value3.morphLength = morphLength * 0.5f;
								playingData.morphDataList[k] = value3;
								num5 = value3.morphLength;
							}
							MorphData value4 = playingData.morphDataList[index];
							value4.morphLength = num5;
							playingData.morphDataList[index] = value4;
							break;
						}
						if (playingData.morphDataList[k].morphLength != 0f)
						{
							break;
						}
					}
				}
			}
		}
		else if (audioClip != null)
		{
			playingData.playingLength = audioClip.length;
		}
		return playingData;
	}

	private List<MorphData> _ParseMorphText(string morphText)
	{
		if (string.IsNullOrEmpty(morphText))
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		List<MorphData> list = new List<MorphData>();
		bool flag = false;
		bool flag2 = false;
		int i = 0;
		while (i < morphText.Length)
		{
			string value = null;
			char c = morphText[i];
			if (c == '[')
			{
				int num = ++i;
				for (; i < morphText.Length; i++)
				{
					if (morphText[i] == ']')
					{
						int num2 = MMD4MecanimCommon.ToInt(morphText, num, i - num);
						if (flag2 && list.Count > 0)
						{
							MorphData value2 = list[list.Count - 1];
							value2.morphLength = (float)num2 * 0.001f;
							list[list.Count - 1] = value2;
						}
						else
						{
							MorphData item = default(MorphData);
							item.morphChar = ' ';
							item.morphName = "";
							item.morphLength = (float)num2 * 0.001f;
							list.Add(item);
						}
						i++;
						break;
					}
				}
				flag2 = false;
				continue;
			}
			if (_IsEscape(c))
			{
				MorphData item2 = default(MorphData);
				item2.morphChar = c;
				item2.morphName = "";
				switch (c)
				{
				case '^':
					item2.morphName = hiraMorphNameList[2];
					break;
				case '`':
					item2.morphName = "";
					break;
				case '-':
				case '/':
				case '~':
					if (list.Count > 0)
					{
						item2.morphName = list[list.Count - 1].morphName;
					}
					break;
				}
				flag2 = true;
				list.Add(item2);
				i++;
				continue;
			}
			if (MMD4MecanimCommon.IsAlphabet(c))
			{
				string value3 = null;
				bool flag3 = false;
				c = MMD4MecanimCommon.ToHalfLower(c);
				if (i + 1 < morphText.Length && MMD4MecanimCommon.IsAlphabet(morphText[i + 1]))
				{
					char value4 = MMD4MecanimCommon.ToHalfLower(morphText[i + 1]);
					if (i + 2 < morphText.Length && MMD4MecanimCommon.IsAlphabet(morphText[i + 2]))
					{
						char value5 = MMD4MecanimCommon.ToHalfLower(morphText[i + 2]);
						stringBuilder.Remove(0, stringBuilder.Length);
						stringBuilder.Append(c);
						stringBuilder.Append(value4);
						stringBuilder.Append(value5);
						if (englishPhraseDictionary.TryGetValue(stringBuilder.ToString(), out value3))
						{
							_AddMorphData(list, value3);
							flag3 = true;
							i += 3;
						}
					}
					if (!flag3)
					{
						stringBuilder.Remove(0, stringBuilder.Length);
						stringBuilder.Append(c);
						stringBuilder.Append(value4);
						if (englishPhraseDictionary.TryGetValue(stringBuilder.ToString(), out value3))
						{
							_AddMorphData(list, value3);
							flag3 = true;
							i += 2;
						}
					}
				}
				if (!flag3)
				{
					if (!flag && (morphText[i] == 'I' || morphText[i] == 'Ｉ'))
					{
						_AddMorphData(list, "ai");
					}
					else if (i > 0 && MMD4MecanimCommon.ToHalfLower(morphText[i - 1]) == 'e' && c == 'e')
					{
						_AddMorphData(list, "-");
					}
					else if (englishPronunDictionary.TryGetValue(c, out value3))
					{
						_AddMorphData(list, value3);
					}
					else
					{
						_AddMorphData(list, " ");
					}
					flag3 = true;
					i++;
				}
				flag = true;
				flag2 = true;
				continue;
			}
			if (japanesePronunDictionary.TryGetValue(c, out value))
			{
				string value6 = null;
				if (i + 1 < morphText.Length && japanesePostPhraseDictionary.TryGetValue(morphText[i + 1], out value6))
				{
					string value7 = null;
					if (japanesePhraseDictionary.TryGetValue(morphText.Substring(i, 2), out value7))
					{
						_AddMorphData(list, value7);
					}
					else
					{
						_AddMorphData(list, value);
						_AddMorphData(list, value6);
					}
					i += 2;
				}
				else
				{
					_AddMorphData(list, value);
					i++;
				}
				flag = false;
				flag2 = true;
				continue;
			}
			if (i + 1 < morphText.Length)
			{
				string value8 = null;
				if (punctuatDictionary.TryGetValue(c, out value8))
				{
					_AddMorphData(list, "   ");
				}
			}
			flag = false;
			flag2 = false;
			i++;
		}
		return list;
	}

	private void _AddMorphData(List<MorphData> morphDataList, string morphScript)
	{
		if (morphDataList == null || morphScript == null)
		{
			return;
		}
		MorphData item = default(MorphData);
		for (int i = 0; i < morphScript.Length; i++)
		{
			item.morphChar = morphScript[i];
			item.morphName = "";
			switch (item.morphChar)
			{
			case 'a':
				item.morphName = hiraMorphNameList[0];
				break;
			case 'i':
				item.morphName = hiraMorphNameList[1];
				break;
			case 'u':
				item.morphName = hiraMorphNameList[2];
				break;
			case 'e':
				item.morphName = hiraMorphNameList[3];
				break;
			case 'o':
				item.morphName = hiraMorphNameList[4];
				break;
			case 'n':
				item.morphName = hiraMorphNameList[2];
				break;
			case '^':
				item.morphName = hiraMorphNameList[2];
				break;
			case '`':
				item.morphName = "";
				break;
			case '-':
			case '/':
			case '~':
				if (morphDataList.Count > 0)
				{
					item.morphName = morphDataList[morphDataList.Count - 1].morphName;
				}
				break;
			}
			morphDataList.Add(item);
		}
	}
}
