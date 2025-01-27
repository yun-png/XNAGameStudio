#region File Description
//-----------------------------------------------------------------------------
// SoundComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Movipa.Components
{
    /// <summary>
    /// Component that plays sound. 
    /// If nothing is specified in the Constructor, it loads the file 
    /// created with the default name. 
    /// The PlaySoundEffect method should be used for SoundEffect playback, 
    /// and the PlayBackgroundMusic method for BackgroundMusic playback.
    /// Cue is specified as the return value for the PlayBackgroundMusic method.
    /// "Volume" and "Pitch" must be provided in the Cue variables on the 
    /// XACT side for the SetVolume and SetPitch methods.
    ///
    /// サウンドを鳴らすコンポーネントです。
    /// コンストラクタで特に何も指定しなければ、デフォルトで作成される名前の
    /// ファイルを読み込むようになっています。
    /// SoundEffectを再生するときはPlaySoundEffectメソッドを使用し、BackgroundMusic�
    /// �再生する時はPlayBackgroundMusicメソッドを
    /// 使用してください。
    /// PlayBackgroundMusicメソッドは戻り値にcueが指定されています。
    /// SetVolumeおよびSetPitchメソッドに関しては、XACT側でCueのVariableに
    /// "Volume"と"Pitch"が用意されている必要があります。
    /// </summary>
    public class SoundComponent : GameComponent
    {
        #region Fields
        private const string DefaultAudioEnginePath = "Content/Audio/Movipa.xgs";
        private const string DefaultWaveBankPath = "Content/Audio/Movipa.xwb";
        private const string DefaultSoundBankPath = "Content/Audio/Movipa.xsb";
        private const string VolumeVariableName = "Volume";
        private const string PitchVariableName = "Pitch";

        private string audioEnginePath;
        private string waveBankPath;
        private string soundBankPath;

        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        #endregion

        #region Properties
        /// <summary>
        /// Obtains the audio engine path.
        /// 
        /// オーディオエンジンのパスを取得します。
        /// </summary>
        public string AudioEnginePath
        {
            get { return audioEnginePath; }
        }


        /// <summary>
        /// Obtains the wave bank path.
        /// 
        /// ウェーブバンクのパスを取得します。
        /// </summary>
        public string WaveBankPath
        {
            get { return waveBankPath; }
        }


        /// <summary>
        /// Obtains the sound bank path.
        ///
        /// サウンドバンクのパスを取得します。
        /// </summary>
        public string SoundBankPath
        {
            get { return soundBankPath; }
        }


        /// <summary>
        /// Obtains the audio engine.
        ///
        /// オーディオエンジンを取得します。
        /// </summary>
        public AudioEngine AudioEngine
        {
            get { return audioEngine; }
        }


        /// <summary>
        /// Obtains the wave bank.
        ///
        /// ウェーブバンクを取得します。
        /// </summary>
        public WaveBank WaveBank
        {
            get { return waveBank; }
        }


        /// <summary>
        /// Obtains the sound bank.
        ///
        /// サウンドバンクを取得します。
        /// </summary>
        public SoundBank SoundBank
        {
            get { return soundBank; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the instance.
        /// The default audio file is used.
        ///
        /// インスタンスを初期化します。
        /// オーディオファイルはデフォルトのものを使用します。
        /// </summary>
        public SoundComponent(Game game)
            : this(game, 
            DefaultAudioEnginePath, DefaultWaveBankPath, DefaultSoundBankPath)
        {
        }


        /// <summary>
        /// Initializes the instance.
        /// 
        /// インスタンスを初期化します。
        /// </summary>
        public SoundComponent(Game game, 
            string audioEnginePath, string waveBankPath, string soundBankPath)
            : base(game)
        {
            this.audioEnginePath = audioEnginePath;
            this.waveBankPath = waveBankPath;
            this.soundBankPath = soundBankPath;
        }


        /// <summary>
        /// Initializes the sound.
        ///
        /// サウンドの初期化を行います。
        /// </summary>
        public override void Initialize()
        {
            audioEngine = new AudioEngine(AudioEnginePath);
            waveBank = new WaveBank(audioEngine, WaveBankPath);
            soundBank = new SoundBank(audioEngine, SoundBankPath);

            audioEngine.Update();

            base.Initialize();
        }


        /// <summary>
        /// Releases all sound resources.
        ///
        /// サウンドリソースを全て開放します。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (soundBank != null && !soundBank.IsDisposed)
                soundBank.Dispose();

            if (waveBank != null && !waveBank.IsDisposed)
                waveBank.Dispose();

            if (audioEngine != null && !audioEngine.IsDisposed)
                audioEngine.Dispose();

            base.Dispose(disposing);
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Updates the sound resource.
        /// 
        /// サウンドリソースの更新処理を行います。
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (audioEngine != null && !audioEngine.IsDisposed)
            {
                audioEngine.Update();
            }

            base.Update(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Obtains the Cue.
        ///
        /// Cueを取得します。
        /// </summary>
        /// <param name="sound">Sound name</param>
        ///  
        /// <param name="sound">サウンドの名前</param>
        /// <returns>サウンドのCue</returns>
        public Cue GetCue(string cueName)
        {
            return soundBank.GetCue(cueName);
        }


        /// <summary>
        /// Plays the sound.
        /// 
        /// サウンドの再生をします。
        /// </summary>
        /// <param name="sound">Sound for playback</param>
        ///  
        /// <param name="sound">再生するサウンド</param>
        ///
        /// <returns>Played Cue</returns>
        ///  
        /// <returns>再生されたCue</returns>
        public Cue PlayBackgroundMusic(string cueName)
        {
            Cue cue = GetCue(cueName);

            if (cue != null && !cue.IsDisposed)
                cue.Play();

            return cue;
        }


        /// <summary>
        /// Plays the sound.
        /// 
        /// サウンドの再生をします。
        /// </summary>
        /// <param name="sound">Sound for playback</param>
        ///  
        /// <param name="sound">再生するサウンド</param>
        public void PlaySoundEffect(string cueName)
        {
            if (soundBank != null && !soundBank.IsDisposed)
                soundBank.PlayCue(cueName);
        }


        /// <summary>
        /// Sets the Cue variable.
        /// 
        /// Cueの変数を設定します。
        /// </summary>
        /// <param name="cue">Cue to be changed</param>
        ///  
        /// <param name="cue">変更するキュー</param>
        ///
        /// <param name="value">Value to be changed</param>
        ///  
        /// <param name="value">変更する値</param>
        public static void SetVariable(Cue cue, string name, float value)
        {
            if (cue == null || cue.IsDisposed)
                return;

            cue.SetVariable(name, value);
        }


        /// <summary>
        /// Changes the Cue volume.
        /// Specified as float type between 0 and 1.
        /// 
        /// キューのボリュームを変更します。
        /// 0〜1までのfloat型で指定します。
        /// </summary>
        /// <param name="cue">Cue to be changed</param>
        ///  
        /// <param name="cue">変更するキュー</param>
        ///
        /// <param name="value">Value to be changed</param>
        ///  
        /// <param name="value">変更する値</param>
        public static void SetVolume(Cue cue, float value)
        {
            float volume = 100.0f * MathHelper.Clamp(value, 0.0f, 1.0f);
            SetVariable(cue, VolumeVariableName, volume);
        }


        /// <summary>
        /// Alters the Cue pitch.
        /// Alters the pitch by semitone in the range -12 to +12.
        /// 
        /// キューのピッチを変更します。
        /// -12〜+12まで、半音ずつ変化します。
        /// </summary>
        /// <param name="cue">Cue to be changed</param>
        ///  
        /// <param name="cue">変更するキュー</param>
       ///
        /// <param name="value">Value to be changed</param>
        ///  
        /// <param name="value">変更する値</param>
        public static void SetPitch(Cue cue, float value)
        {
            float pitch = 12.0f * MathHelper.Clamp(value, -1.0f, 1.0f);
            SetVariable(cue, PitchVariableName, pitch);
        }


        /// <summary>
        /// Stops the Cue.
        /// 
        /// Cueを停止します。
        /// </summary>
        /// <param name="cue">Cue to be stopped</param>
        ///  
        /// <param name="cue">停止するキュー</param>
        public static void Stop(Cue cue)
        {
            if (cue != null && !cue.IsDisposed && cue.IsPlaying)
                cue.Stop(AudioStopOptions.Immediate);
        }
        #endregion
    }
}
