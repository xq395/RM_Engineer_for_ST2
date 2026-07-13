param(
    [string]$ArtDir = "F:\st2\美工"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$projectDir = $PSScriptRoot
$cardDir = Join-Path $projectDir "Laughman\images\card_portraits"
$bigCardDir = Join-Path $cardDir "big"
$potionDir = Join-Path $projectDir "Laughman\images\potions"
$cardsJson = Join-Path $projectDir "Laughman\localization\eng\cards.json"

if (-not (Test-Path -LiteralPath $cardDir)) {
    throw "Card portrait directory does not exist: $cardDir"
}
if (-not (Test-Path -LiteralPath $bigCardDir)) {
    New-Item -ItemType Directory -Path $bigCardDir | Out-Null
}
if (-not (Test-Path -LiteralPath $potionDir)) {
    New-Item -ItemType Directory -Path $potionDir | Out-Null
}

$localized = Get-Content -LiteralPath $cardsJson -Raw | ConvertFrom-Json
$cardsByTitle = @{}
foreach ($property in $localized.PSObject.Properties) {
    if ($property.Name -match '^LAUGHMAN-(.+)\.title$' -and $property.Name -notmatch 'HOVER_') {
        $cardsByTitle[[string]$property.Value] = $matches[1].ToLowerInvariant()
    }
}

$titleAliases = @{
    "机器人大赛总决赛" = "机器人大赛决赛"
    "嵌入式大赛" = "嵌入式比赛"
    "浪潮形态" = "我即浪潮"
}

function Get-ArtCandidate([System.IO.FileInfo]$file) {
    $title = $file.BaseName
    $rank = 1
    if ($cardsByTitle.ContainsKey($title)) {
        return [PSCustomObject]@{ Title = $title; Rank = $rank; File = $file }
    }
    if ($title -match '^(.*?)[（(]\s*新?2\s*[）)]$') {
        $title = $matches[1].Trim()
        $rank = 3
    }
    elseif ($title -match '^(.*?)新2$') {
        $title = $matches[1].Trim()
        $rank = 3
    }
    elseif ($title -match '^(.*?)新$') {
        $title = $matches[1].Trim()
        $rank = 2
    }

    if ($titleAliases.ContainsKey($title)) {
        $title = $titleAliases[$title]
    }

    [PSCustomObject]@{ Title = $title; Rank = $rank; File = $file }
}

function Export-FittedImage([string]$source, [string]$target, [int]$width, [int]$height) {
    $inputImage = [Drawing.Bitmap]::FromFile($source)
    try {
        $corners = @(
            $inputImage.GetPixel(0, 0),
            $inputImage.GetPixel($inputImage.Width - 1, 0),
            $inputImage.GetPixel(0, $inputImage.Height - 1),
            $inputImage.GetPixel($inputImage.Width - 1, $inputImage.Height - 1)
        )
        $background = [Drawing.Color]::FromArgb(
            255,
            [int](($corners | Measure-Object R -Average).Average),
            [int](($corners | Measure-Object G -Average).Average),
            [int](($corners | Measure-Object B -Average).Average)
        )
        $outputImage = [Drawing.Bitmap]::new($width, $height, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [Drawing.Graphics]::FromImage($outputImage)
            try {
                $graphics.Clear($background)
                $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
                $graphics.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::HighQuality
                $scale = [Math]::Min($width / $inputImage.Width, $height / $inputImage.Height)
                $drawWidth = [int][Math]::Round($inputImage.Width * $scale)
                $drawHeight = [int][Math]::Round($inputImage.Height * $scale)
                $graphics.DrawImage(
                    $inputImage,
                    [int](($width - $drawWidth) / 2),
                    [int](($height - $drawHeight) / 2),
                    $drawWidth,
                    $drawHeight
                )
            }
            finally {
                $graphics.Dispose()
            }
            $outputImage.Save($target, [Drawing.Imaging.ImageFormat]::Png)
        }
        finally {
            $outputImage.Dispose()
        }
    }
    finally {
        $inputImage.Dispose()
    }
}

$selected = @{}
$ignoredPattern = 'image2|^局部截取_|^药水：'
$imageFiles = Get-ChildItem -LiteralPath $ArtDir -File | Where-Object {
    $_.Extension -match '^\.(png|jpe?g|webp)$' -and $_.BaseName -notmatch $ignoredPattern
}
foreach ($file in $imageFiles) {
    $candidate = Get-ArtCandidate $file
    if (-not $cardsByTitle.ContainsKey($candidate.Title)) {
        continue
    }
    $current = $selected[$candidate.Title]
    if ($null -eq $current -or
        $candidate.Rank -gt $current.Rank -or
        ($candidate.Rank -eq $current.Rank -and $candidate.File.LastWriteTime -gt $current.File.LastWriteTime)) {
        $selected[$candidate.Title] = $candidate
    }
}

foreach ($title in $selected.Keys) {
    if ($title -eq "防御") {
        continue
    }
    $id = $cardsByTitle[$title]
    Export-FittedImage $selected[$title].File.FullName (Join-Path $cardDir "$id.png") 250 190
    Export-FittedImage $selected[$title].File.FullName (Join-Path $bigCardDir "$id.png") 1000 760
}

$potions = @{
    "药水：美式咖啡" = "spare_battery"
    "药水：巧乐兹" = "coolant_flask"
    "药水：WD40" = "calibration_fluid"
}
foreach ($title in $potions.Keys) {
    $source = Get-ChildItem -LiteralPath $ArtDir -File | Where-Object BaseName -eq $title |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($null -ne $source) {
        Export-FittedImage $source.FullName (Join-Path $potionDir "$($potions[$title]).png") 64 64
    }
}

Write-Host "Imported $($selected.Count) card images and $($potions.Count) potion images."
Write-Host "Missing card images:"
foreach ($card in ($cardsByTitle.GetEnumerator() | Where-Object { -not $selected.ContainsKey($_.Key) } | Sort-Object Key)) {
    Write-Host "  $($card.Value) = $($card.Key)"
}
