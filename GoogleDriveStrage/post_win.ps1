param($TargetDir, $ProjectDir, $TargetName)

$TargetDir = $($TargetDir).Replace("\","/");
$ProjectDir = $($ProjectDir).Replace("\","/");
$TargetName = $($TargetName).Replace("\","/");
$CopyTo = "$($ProjectDir)/../$($TargetName)"

echo "New-Item "$($CopyTo)" -ItemType Directory -ErrorAction SilentlyContinue"
New-Item "$($CopyTo)" -ItemType Directory -ErrorAction SilentlyContinue

echo "Copy-Item -Path '$($TargetDir)'/* -Destination '$($CopyTo)' -Force -Recurse"
Copy-Item -Path "$($TargetDir)/*" -Destination "$($CopyTo)" -Force -Recurse 