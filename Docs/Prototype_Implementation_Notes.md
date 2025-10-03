# プロトタイプ実装メモ

このドキュメントでは、3Dローグライク「Chrono Depths」プロトタイプの初期実装状況をまとめます。設計プランで定義したマイルストーンのうち、コアシステム実装に向けて以下の項目を着手しました。

## 実装済みコンポーネント

### 1. システムブートストラップ
- `Assets/Scenes/PrototypeScene.unity` を追加し、プレイヤー、時間・カード・装備システム、ダンジョン生成を一括で配置。
- `GameBootstrap` がシーン開始時にダンジョン生成、時間制御、カードデッキ、装備反映を順に実行。
- シーンへ配置するだけで初期化が完結するよう警告ログを含めたガードを追加。

### 2. ダンジョン生成
- `DungeonConfig` ScriptableObject でルーム数、グリッドサイズ、使用するプレハブを管理。プレハブ未設定時は自動生成した仮ルームを利用。
- `DungeonGenerator` がランダムウォーク方式で 2D グリッド上にルームを配置し、生成結果の座標リストを他システムへ公開。

### 3. 時間制御（セミリアルタイム）
- `TimeController` がプレイヤーの行動通知に応じて `Time.timeScale` を補間し、アクション時と待機時のテンポ差を再現。
- `PlayerActionController` を組み合わせることで、入力に応じた時間制御フィーリングの検証が可能。
- `SimplePlayerController` を追加し、WASD とマウス操作で移動・旋回しつつ時間制御へ行動通知。

### 4. デッキ&装備ベース
- `CardData` と `EquipmentItem` の ScriptableObject でデータ定義を開始。
- プロトタイプ用のサンプルデータ（Strike/Guard/Focus カード、Chrono Blade など装備 3 種）を追加し、シーンの初期デッキ・装備構成に適用。
- `CardDeckController` がドロー/ディスカード/リシャッフルを担い、`EquipmentLoadout` が合算ステータスを算出。

## 次のステップ（提案）
1. プレイヤーキャラクターの移動・ステートマシンを追加し、`PlayerActionController` と統合してテンポを検証。
2. `DungeonGenerator` が配置した部屋へ NavMesh やスポーンポイントを自動設置するエクステンションを実装。
3. カードプレイ時のイベント（例: ダメージ、バフ）を ScriptableObject から呼び出せるようアクションインタフェースを設計。
4. UI プロトタイプ（ミニマップ、カードハンド）を仮実装し、システム同士の連携を可視化。

## 参考
- 設計プラン: `Docs/3D_Roguelike_Plan.md`

