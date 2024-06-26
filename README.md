# IdentifierArchive
履歴を持たず、Version識別子でのみ管理するクラウドデータアーカイブシステム

## プロジェクトの目的
このシステムは、Gitと併用して、GitLFSの代替手段の一つとして利用される事を想定しています。 

GitLFSの課題として、履歴上の古いバージョンを削除する機能が正式にサポートされていないことが挙げられます。 

これは、クラウドストレージのコストを下げるうえで大きな問題になるため、シンプルな代替手段が必要だと考えました。

## 基本機能
- IdentifierArchiveSettings.ini で、作業フォルダ、暗号圧縮コマンド、復号解凍コマンド、ULコマンド、DLコマンドを設定
- archiveIdentifier.txt(及び比較のための archiveIdentifier.current.txt) というバージョン情報ファイルを作成（archiveIdentifier.current.txt はgitignore）
- `IdentifierArchive push settingsFilePath targetPath (identifier)` コマンドで、設定された暗号圧縮コマンドを実行、圧縮ファイルを作業フォルダに配置、ULコマンドを実行、バージョン情報ファイルの自動更新
- `IdentifierArchive pull settingsFilePath targetPath (identifier)` コマンドで、 archiveIdentifier.txt と archiveIdentifier.current.txt(または identifier 引数) が違っていれば、DLコマンドと復号解凍コマンドを実行、ターゲットパス内のコンテンツを削除したあと、解凍したファイルを配置
- カスタムコマンド内では、`%INPUT_PATH%` `%OUTPUT_PATH%` `%INPUT_FILENAME%` `%OUTPUT_FILENAME%` などの環境変数が利用可能
