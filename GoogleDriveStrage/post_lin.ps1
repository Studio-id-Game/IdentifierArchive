param($TargetDir, $ProjectDir, $TargetName)

$TargetDir = $($TargetDir).Replace("\","/");
$ProjectDir = $($ProjectDir).Replace("\","/");
$TargetName = $($TargetName).Replace("\","/");
$CopyTo = "$($ProjectDir)/../$($TargetName)"

echo "mkdir -p $($CopyTo)"
mkdir -p $($CopyTo)
echo "rsync -rv --delete "$($TargetDir)" "$($CopyTo)""
rsync -rv --delete "$($TargetDir)" "$($CopyTo)"