Get-ChildItem -Include *.cs -Recurse -File | ForEach-Object {
    $file = $_.FullName
    $bytes = [System.IO.File]::ReadAllBytes($file)

    $encoding = ""

    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        $encoding = "UTF-8 with BOM"
    } elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
        $encoding = "UTF-16 LE"
    } elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
        $encoding = "UTF-16 BE"
    } else {
        # 尝试使用 UTF8 解析，看是否报错
        try {
            [System.Text.Encoding]::UTF8.GetString($bytes) | Out-Null
            $encoding = "UTF-8 without BOM or ANSI"
        } catch {
            $encoding = "Unknown or Binary"
        }
    }

    Write-Output "$file`t$encoding"
}
