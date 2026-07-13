param(
    [string]$Root = $PSScriptRoot,
    [string]$OutputPath = (Join-Path $PSScriptRoot 'CARD_ART_CATALOG.md')
)

$ErrorActionPreference = 'Stop'

function Normalize-CardKey([string]$value) {
    return ($value -replace '[^A-Za-z0-9]', '').ToUpperInvariant()
}

function Collapse-Code([string]$value) {
    if ([string]::IsNullOrWhiteSpace($value)) { return '-' }
    return (($value -replace '\s+', ' ').Trim())
}

function Escape-Markdown([string]$value) {
    if ($null -eq $value) { return '-' }
    return ($value -replace '\|', '\|' -replace "`r?`n", '<br>')
}

function Get-BracedBlock([string]$text, [int]$openBraceIndex) {
    $depth = 0
    for ($i = $openBraceIndex; $i -lt $text.Length; $i++) {
        if ($text[$i] -eq '{') { $depth++ }
        elseif ($text[$i] -eq '}') {
            $depth--
            if ($depth -eq 0) {
                return $text.Substring($openBraceIndex, $i - $openBraceIndex + 1)
            }
        }
    }
    throw "Unclosed block at index $openBraceIndex"
}

function Get-MemberSource([string]$classBlock, [string]$memberName) {
    $match = [regex]::Match($classBlock, [regex]::Escape($memberName))
    if (-not $match.Success) { return '-' }

    $arrow = $classBlock.IndexOf('=>', $match.Index)
    $brace = $classBlock.IndexOf('{', $match.Index)
    if ($arrow -ge 0 -and ($brace -lt 0 -or $arrow -lt $brace)) {
        $end = $classBlock.IndexOf(';', $arrow)
        if ($end -ge 0) { return Collapse-Code $classBlock.Substring($match.Index, $end - $match.Index + 1) }
    }
    if ($brace -ge 0) {
        return Collapse-Code ($classBlock.Substring($match.Index, $brace - $match.Index) + (Get-BracedBlock $classBlock $brace))
    }
    return '-'
}

$localizationPath = Join-Path $Root 'Laughman\localization\eng\cards.json'
$cardsPath = Join-Path $Root 'LaughmanCode\Cards'
$localization = Get-Content -Raw -LiteralPath $localizationPath | ConvertFrom-Json

$locByNormalizedId = @{}
foreach ($property in $localization.PSObject.Properties) {
    if ($property.Name -match '^LAUGHMAN-(?!HOVER_)(.+)\.title$') {
        $id = $Matches[1]
        $descriptionProperty = $localization.PSObject.Properties["LAUGHMAN-$id.description"]
        $locByNormalizedId[(Normalize-CardKey $id)] = [pscustomobject]@{
            Id = $id
            Title = [string]$property.Value
            Description = if ($descriptionProperty) { [string]$descriptionProperty.Value } else { '-' }
        }
    }
}

$classes = @{}
foreach ($file in Get-ChildItem -LiteralPath $cardsPath -Filter '*.cs' -File) {
    $text = Get-Content -Raw -LiteralPath $file.FullName
    $matches = [regex]::Matches($text, 'public\s+(?<abstract>abstract\s+)?(?:sealed\s+)?class\s+(?<name>[A-Za-z0-9_]+)\s*:\s*(?<base>[A-Za-z0-9_]+)')
    foreach ($match in $matches) {
        $openBrace = $text.IndexOf('{', $match.Index)
        $block = Get-BracedBlock $text $openBrace
        $line = ($text.Substring(0, $match.Index) -split "`n").Count
        $classes[$match.Groups['name'].Value] = [pscustomobject]@{
            Name = $match.Groups['name'].Value
            Base = $match.Groups['base'].Value
            Abstract = $match.Groups['abstract'].Success
            File = $file.FullName
            RelativeFile = $file.FullName.Substring($Root.Length + 1).Replace('\', '/')
            Line = $line
            Block = $block
        }
    }
}

function Inherits-LaughmanCard([object]$classInfo) {
    $base = $classInfo.Base
    while ($base) {
        if ($base -eq 'LaughmanCard') { return $true }
        if (-not $classes.ContainsKey($base)) { return $false }
        $base = $classes[$base].Base
    }
    return $false
}

function Get-CardMetadata([object]$classInfo) {
    $current = $classInfo
    while ($current) {
        $pattern = '(?:public|protected)\s+' + [regex]::Escape($current.Name) + '\s*\([^)]*\)\s*:\s*base\(\s*(?<cost>[^,]+),\s*CardType\.(?<type>[A-Za-z]+),\s*CardRarity\.(?<rarity>[A-Za-z]+),\s*TargetType\.(?<target>[A-Za-z]+)\s*\)'
        $ctor = [regex]::Match($current.Block, $pattern)
        if ($ctor.Success) {
            $cost = Collapse-Code $ctor.Groups['cost'].Value
            if ($classInfo.Block -match 'HasEnergyCostX\s*=>\s*true') { $cost = 'X' }
            return [pscustomobject]@{
                Cost = $cost
                Type = $ctor.Groups['type'].Value
                Rarity = $ctor.Groups['rarity'].Value
                Target = $ctor.Groups['target'].Value
            }
        }
        if (-not $classes.ContainsKey($current.Base)) { break }
        $current = $classes[$current.Base]
    }
    return [pscustomobject]@{ Cost = '?'; Type = '?'; Rarity = '?'; Target = '?' }
}

function Get-InheritedMemberSource([object]$classInfo, [string]$memberName) {
    $current = $classInfo
    while ($current) {
        $source = Get-MemberSource $current.Block $memberName
        if ($source -ne '-') { return $source }
        if (-not $classes.ContainsKey($current.Base)) { break }
        $current = $classes[$current.Base]
    }
    return '-'
}

$cards = foreach ($classInfo in $classes.Values) {
    if ($classInfo.Abstract -or -not (Inherits-LaughmanCard $classInfo)) { continue }
    $normalized = Normalize-CardKey $classInfo.Name
    if (-not $locByNormalizedId.ContainsKey($normalized)) {
        Write-Warning "No localization match for $($classInfo.Name)"
        continue
    }
    $loc = $locByNormalizedId[$normalized]
    $meta = Get-CardMetadata $classInfo
    [pscustomobject]@{
        Name = $loc.Title
        Id = $loc.Id
        Class = $classInfo.Name
        Rarity = $meta.Rarity
        Type = $meta.Type
        Cost = $meta.Cost
        Target = $meta.Target
        Description = $loc.Description
        DynamicVars = Get-InheritedMemberSource $classInfo 'CanonicalVars'
        Keywords = Get-InheritedMemberSource $classInfo 'CanonicalKeywords'
        Upgrade = Get-InheritedMemberSource $classInfo 'OnUpgrade'
        Source = "$($classInfo.RelativeFile):$($classInfo.Line)"
        Image = "Laughman/images/card_portraits/$($loc.Id.ToLowerInvariant()).png"
        BigImage = "Laughman/images/card_portraits/big/$($loc.Id.ToLowerInvariant()).png"
    }
}

$rarityOrder = @('Basic', 'Common', 'Uncommon', 'Rare', 'Ancient', 'Token', 'Quest')
$typeNames = @{ Attack = '攻击'; Skill = '技能'; Power = '能力' }
$rarityNames = @{ Basic = '基础'; Common = '普通'; Uncommon = '罕见'; Rare = '稀有'; Ancient = '先古'; Token = '衍生'; Quest = '任务' }
$targetNames = @{ Self = '自身'; AnyEnemy = '单个敌人'; AllEnemies = '所有敌人'; AnyAlly = '单个友方'; AllAllies = '所有友方' }

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# RM工程师卡牌美工总表')
$lines.Add('')
$lines.Add('> 自动生成文件。数据来源：`LaughmanCode/Cards/*.cs` 与 `Laughman/localization/eng/cards.json`。')
$lines.Add('> 重新生成：在仓库根目录执行 `powershell -ExecutionPolicy Bypass -File .\export-card-art-catalog.ps1`。')
$lines.Add('')
$lines.Add("- 卡牌总数：$($cards.Count)")
$lines.Add('- 普通卡图建议尺寸：`1000x760`（可用 `500x380` 测试）')
$lines.Add('- 大图建议尺寸：`1000x760`（与普通卡图保持相同构图比例）')
$lines.Add('- 图片中不要生成卡名、数值或说明文字，游戏会自行渲染。')
$lines.Add('- `Token`、`Quest`、`Ancient` 虽不一定进入普通奖励池，仍需要卡图。')
$lines.Add('')

foreach ($rarity in $rarityOrder) {
    $group = @($cards | Where-Object Rarity -eq $rarity | Sort-Object Id)
    if ($group.Count -eq 0) { continue }
    $displayRarity = if ($rarityNames.ContainsKey($rarity)) { $rarityNames[$rarity] } else { $rarity }
    $lines.Add("## $displayRarity / $rarity（$($group.Count) 张）")
    $lines.Add('')
    foreach ($card in $group) {
        $displayType = if ($typeNames.ContainsKey($card.Type)) { $typeNames[$card.Type] } else { $card.Type }
        $displayTarget = if ($targetNames.ContainsKey($card.Target)) { $targetNames[$card.Target] } else { $card.Target }
        $lines.Add("### $($card.Name)（$($card.Id)）")
        $lines.Add('')
        $lines.Add("- 类名：``$($card.Class)``")
        $lines.Add("- 类型：$displayType / ``$($card.Type)``")
        $lines.Add("- 费用：``$($card.Cost)``")
        $lines.Add("- 目标：$displayTarget / ``$($card.Target)``")
        $lines.Add("- 效果：$(Escape-Markdown $card.Description)")
        $lines.Add("- 数值定义：``$(Escape-Markdown $card.DynamicVars)``")
        $lines.Add("- 关键词：``$(Escape-Markdown $card.Keywords)``")
        $lines.Add("- 升级实现：``$(Escape-Markdown $card.Upgrade)``")
        $lines.Add("- 代码：``$($card.Source)``")
        $lines.Add("- 普通卡图：``$($card.Image)``")
        $lines.Add("- 大图：``$($card.BigImage)``")
        $lines.Add('- 美术构图备注：待填写')
        $lines.Add('')
    }
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[void]$lines.RemoveAt($lines.Count - 1)
[System.IO.File]::WriteAllLines($OutputPath, $lines, $utf8NoBom)
Write-Output "Generated $OutputPath with $($cards.Count) cards."
