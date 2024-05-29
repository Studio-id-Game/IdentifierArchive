# 初期設定

1. プロジェクトフォルダをgitリポジトリとして初期化する

2. "IdentifierArchive.exe" si -sf="プロジェクトフォルダのパス" でIdentifierArchiveの初期化をする

3. このフォルダの中身をプロジェクトフォルダにコピーする

4. "IdentifierArchiveWork~\LocalKeys~\.localkey~"の、"%ProjectName%"（各プロジェクトと一対一対応する名前） を設定する。

5. 7zipをインストールする
    - https://7-zip.opensource.jp/
    
6. "IdentifierArchiveWork~\LocalKeys~\.localkey~"の、"%7z%"（7zipコマンドライン版のパス）と、"%7z.PassWord%"（圧縮暗号化に使うパスワード）を設定する。

7. GoogleDriveへのリモートアクセスキーを設定する
    - https://www.ipentec.com/document/software-google-cloud-platform-enable-google-drive-api
    - https://www.ipentec.com/document/software-google-cloud-platform-create-service-account
    - https://www.ipentec.com/document/google-drive-add-share-folder-for-service-account

8. "IdentifierArchiveWork~\LocalKeys~\.gdrivelocalkey~" ファイルを、あなたのアクセスキーファイルで上書きする

9. "IdentifierArchiveWork~\LocalKeys~\.localkey~"の、"%GoogleDriveStrage%"（GoogleDriveStrage.exeのパス）と"%GoogleDriveStrage.RootFolderID%"（アップロード先のGoogleDriveフォルダのID）を設定する。

10. "IdentifierArchive.exe" ti -sf="プロジェクトフォルダのパス" -tf="workspace" で、アーカイブとして初期化するフォルダを設定する

# 利用例

1. "workspace" フォルダ内で好きなファイルを作成したり、編集したりする。サブフォルダ内でもOK。（これらのファイルはgit管理から外れているはず）

2. "IdentifierArchive.exe" za ul -sf="プロジェクトフォルダのパス" -tf="workspace" で、現在の状態の7zファイルのキャッシュが作成され、gitで管理されているアーカイブ識別子が更新され、ドライブに7zファイルがアップロードされます。

3. "IdentifierArchive.exe" dl zx -sf="プロジェクトフォルダのパス" -tf="workspace" で、gitで管理されているアーカイブ識別子を使って、ドライブから7zファイルをダウンロードし、"workspace" フォルダ内に展開します。

4. "IdentifierArchive.exe" dl zx -sf="プロジェクトフォルダのパス" -tf="workspace" -id="アーカイブ識別子" で任意のアーカイブ識別子を指定して、DL＆展開する事もできます。

5. あとはいろいろ自分で試してみてね。
   - アーカイブのキャッシュは、デフォルトだと"D:\IdentifierArchiveCache~\%ProjectName%\%TARGET_FOLDER%\" にすべて保存されます。  
