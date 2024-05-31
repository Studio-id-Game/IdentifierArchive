param($TargetDir, $ProjectDir, $TargetName)

$TargetDir = $($TargetDir).Replace("\","/");
$ProjectDir = $($ProjectDir).Replace("\","/");
$TargetName = $($TargetName).Replace("\","/");
$CopyTo = "$($ProjectDir)/../$($TargetName)"

echo "copy $($TargetDir) to $($CopyTo)"
mkdir -p $($CopyTo)
cp -r -v -force "$($TargetDir)/*" $($CopyTo)