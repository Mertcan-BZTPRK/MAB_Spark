# MAB_Spark - Test Kısaltmaları Ekleme Betiği
# Bu betik SQLite veritabanına örnek kısaltmalar ekler

param(
	[string]$Action = "add"
)

# AppData yolunu belirle
$AppDataPath = Join-Path $env:LOCALAPPDATA "MAB_Spark"
$DbPath = Join-Path $AppDataPath "shortcuts.db"

# SQLite DLL yolunu belirle
$ProjectPath = Split-Path -Parent $MyInvocation.MyCommand.Definition
$SqliteDll = Join-Path $ProjectPath "packages\System.Data.SQLite*\lib\net45\System.Data.SQLite.dll"

# Eğer DLL bulunamazsa, GAC'den yükle
try {
	[System.Reflection.Assembly]::Load("System.Data.SQLite")
} catch {
	Write-Host "SQLite DLL yüklenemedi. Devam etmek için 'dotnet add package System.Data.SQLite' çalıştırın."
	exit 1
}

# Veritabanını kontrol et
if (-not (Test-Path $DbPath)) {
	Write-Host "Veritabanı bulunamadı: $DbPath"
	Write-Host "Lütfen önce uygulamayı çalıştırın."
	exit 1
}

# SQLite bağlantısını oluştur
$ConnectionString = "Data Source=$DbPath;Version=3;"
$Connection = New-Object System.Data.SQLite.SQLiteConnection $ConnectionString

try {
	$Connection.Open()

	if ($Action -eq "add") {
		# Örnek kısaltmalar
		$Shortcuts = @(
			@{short="btw"; expanded="by the way"},
			@{short="asap"; expanded="as soon as possible"},
			@{short="fyi"; expanded="for your information"},
			@{short="tbd"; expanded="to be determined"},
			@{short="imho"; expanded="in my humble opinion"},
			@{short="wrt"; expanded="with respect to"},
			@{short="etc"; expanded="et cetera"},
			@{short="pls"; expanded="please"},
			@{short="thx"; expanded="thanks"},
			@{short="tbh"; expanded="to be honest"}
		)

		foreach ($Shortcut in $Shortcuts) {
			$Command = $Connection.CreateCommand()
			$Command.CommandText = @"
				INSERT OR IGNORE INTO Shortcuts (ShortText, ExpandedText, IsEnabled, CreatedAt, UpdatedAt)
				VALUES (@short, @expanded, 1, datetime('now'), datetime('now'))
"@
			$Command.Parameters.AddWithValue("@short", $Shortcut.short) | Out-Null
			$Command.Parameters.AddWithValue("@expanded", $Shortcut.expanded) | Out-Null

			$Rows = $Command.ExecuteNonQuery()
			if ($Rows -gt 0) {
				Write-Host "✅ Eklendi: '$($Shortcut.short)' → '$($Shortcut.expanded)'"
			}
		}
	}
	elseif ($Action -eq "list") {
		# Tüm kısaltmaları listele
		$Command = $Connection.CreateCommand()
		$Command.CommandText = "SELECT ShortText, ExpandedText, IsEnabled FROM Shortcuts ORDER BY ShortText"

		$Reader = $Command.ExecuteReader()
		Write-Host "`n📋 Tüm Kısaltmalar:"
		Write-Host "─────────────────────────────────────────────"

		while ($Reader.Read()) {
			$Status = if ($Reader[2]) { "✅ Aktif" } else { "❌ Pasif" }
			Write-Host "$($Reader[0].PadRight(15)) → $($Reader[1]) [$Status]"
		}

		$Reader.Close()
	}
	elseif ($Action -eq "clear") {
		# Tüm kısaltmaları sil
		$Command = $Connection.CreateCommand()
		$Command.CommandText = "DELETE FROM Shortcuts"
		$Rows = $Command.ExecuteNonQuery()
		Write-Host "⚠️  $Rows kısaltma silindi."
	}

	Write-Host "`n✨ İşlem tamamlandı!"

} catch {
	Write-Host "❌ Hata: $_"
	exit 1
} finally {
	$Connection.Close()
}
