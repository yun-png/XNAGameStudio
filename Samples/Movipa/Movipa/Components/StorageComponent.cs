#region File Description
//-----------------------------------------------------------------------------
// StorageComponent.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Movipa.Components
{
    /// <summary>
    /// Component that displays the storage selector.
    /// It automatically repeats until the storage is selected.
    /// GamerServicesComponent must be added in order to use
    /// this component.
    /// 
    /// ストレージセレクタを表示するコンポーネントです。
    /// ストレージが選択されるまで自動でリピートされます。
    /// このコンポーネントを使用するためには、GamerServicesComponentを
    /// 追加している必要があります。
    /// </summary>
    public class StorageComponent : GameComponent
    {
        #region Private Types
        /// <summary>
        /// Processing status
        /// 
        /// 処理状態
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// Initialization
            /// 
            /// 初期化
            /// </summary>
            BeginStorageSelect,

            /// <summary>
            /// Storage selection in progress
            /// 
            /// ストレージ選択中
            /// </summary>
            StorageSelect,

            /// <summary>
            /// Storage selection completed
            /// 
            /// ストレージ選択終了
            /// </summary>
            EndStorageSelect,
        }
        #endregion

        #region Fields
        // Processing status
        // 
        // 処理状態
        private Phase phase;

        // Container name used in storage
        // 
        // ストレージ上で使用するコンテナ名
        private string containerName;

        // Player number
        // 
        // プレイヤー番号
        private PlayerIndex playerIndex;

        // Asynchronous processing status
        // 
        // 非同期処理の状態
        private IAsyncResult asyncResult;

        // Storage container
        // 
        // ストレージコンテナ
        private StorageContainer storageContainer;

        // Storage device
        // 
        // ストレージデバイス
        private StorageDevice storageDevice;
        #endregion

        #region Properties
        /// <summary>
        /// Obtains the guide display status.
        /// 
        /// ガイドの表示状態を取得します。
        /// </summary>
        public bool IsVisible
        {
            get
            {
                if (phase != Phase.EndStorageSelect)
                    return true;

                return Guide.IsVisible;
            }
        }


        /// <summary>
        /// Obtains the storage connection status.
        ///
        /// ストレージの接続状態を取得します。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (storageDevice == null)
                    return false;

                return storageDevice.IsConnected;
            }
        }


        /// <summary>
        /// Obtains or sets the player index.
        ///
        /// プレイヤーインデックスを取得または設定します。
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the instance.
        /// 
        /// インスタンスを初期化します。
        /// </summary>
        public StorageComponent(Game game)
            : base(game)
        {
            phase = Phase.EndStorageSelect;
            playerIndex = PlayerIndex.One;
        }


        /// <summary>
        /// Initializes the storage selector.
        ///
        /// ストレージセレクタを初期化します。
        /// </summary>
        private bool InitializeStorageDeviceSelector(PlayerIndex player)
        {
            // Checks if the guide is currently displayed; 
            // if so, the following processing is not performed.
            // 
            // ガイドが表示中かチェックし、表示中なら以下の処理を行いません。
            if (Guide.IsVisible)
            {
                return false;
            }

            // Releases the storage.
            // 
            // ストレージの開放処理を行います。
            DisposeStorage();

            // Displays the storage selector screen.
            // 
            // ストレージ選択画面を表示します。
            asyncResult = Guide.BeginShowStorageDeviceSelector(player, null, null);

            // Returns true if the normal screen is displayed successfully.
            // 
            // 正常に画面が表示出来たならtrueを返します。
            return (asyncResult != null);
        }


        /// <summary>
        /// Releases all resources.
        /// 
        /// 全てのリソースを開放します。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            DisposeStorage();
            base.Dispose(disposing);
        }


        /// <summary>
        /// Releases the storage.
        /// 
        /// ストレージの開放処理を行います。
        /// </summary>
        private void DisposeStorage()
        {
            if (storageContainer != null)
            {
                if (!storageContainer.IsDisposed)
                {
                    storageContainer.Dispose();
                }
                storageContainer = null;
            }

            if (storageDevice != null)
            {
                storageDevice = null;
            }
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Performs update processing.
        /// 
        /// 更新処理を行います。
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (phase == Phase.BeginStorageSelect)
            {
                if (InitializeStorageDeviceSelector(playerIndex))
                {
                    // Sets to storage select.
                    // 
                    // ストレージ選択に設定します。
                    phase = Phase.StorageSelect;
                }
            }
            else if (phase == Phase.StorageSelect)
            {
                if (UpdateStorageSelect())
                {
                    // Sets to storage select end.
                    // 
                    // ストレージ選択終了に設定します。
                    phase = Phase.EndStorageSelect;
                }
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// Performs update processing during storage selection.
        /// 
        /// ストレージ選択中の更新処理を行います。
        /// </summary>
        private bool UpdateStorageSelect()
        {
            // Processing is not performed if storage selection is not completed.
            // 
            // ストレージ選択が完了していない場合は処理を行いません。
            if (asyncResult == null || !asyncResult.IsCompleted)
            {
                return false;
            }

            // Obtains the device.
            // 
            // デバイスを取得します。
            storageDevice = Guide.EndShowStorageDeviceSelector(asyncResult);

            // Sets the blade status to Null.
            // 
            // ブレード状態を null にセットします。
            asyncResult = null;

            // Checks if the device has been obtained successfully.
            // 
            // デバイスが取得できたかチェックします。
            if (storageDevice == null || !storageDevice.IsConnected)
            {
                // If not connected (cancelled).
                // 
                // 接続されていない（キャンセルされた）

                // Re-displays the storage selection blade.
                // 
                // 再度ストレージ選択のブレードを表示します。
                phase = Phase.BeginStorageSelect;

                // Returns fail.
                // 
                // 失敗を返します。
                return false;
            }

            // If connected to storage successfully.
            // 
            // ストレージに接続成功

            // Obtains the container.
            // 
            // コンテナを取得します。
            storageContainer = storageDevice.OpenContainer(containerName);

            return true;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Displays the storage selector.
        /// 
        /// ストレージセレクタを表示させます。
        /// </summary>
        /// <param name="container">Container name used in storage</param>
        ///  
        /// <param name="container">ストレージで使用するコンテナ名</param>

        /// <param name="player">Player number</param>
        ///  
        /// <param name="player">プレイヤー番号</param>

        /// <returns>Displayed successfully status</returns>
        ///  
        /// <returns>表示成功状態</returns>
        public bool ShowStorageDeviceSelector(string container, PlayerIndex player)
        {
            // Checks if currently displayed; if so, the processing is not performed.
            // 
            // 表示中かチェックし、表示中なら処理を行いません。
            if (IsVisible)
            {
                return false;
            }

            phase = Phase.BeginStorageSelect;

            playerIndex = player;

            containerName = container;

            return true;
        }

        /// <summary>
        /// Connects the storage container path and obtains the file name.
        /// 
        /// ストレージコンテナのパスを繋げてファイル名を取得します。
        /// </summary>
        /// <param name="fileName">File name</param>
        ///  
        /// <param name="fileName">ファイル名</param>
        /// <returns>Full path to container</returns>
        ///  
        /// <returns>コンテナへのフルパス</returns>
        public string GetStoragePath(string fileName)
        {
            return Path.Combine(storageContainer.Path, fileName);
        }
        #endregion
    }
}
