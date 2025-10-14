$password = 'password123'
$bytes = [System.Text.Encoding]::UTF8.GetBytes($password)
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$hash = $sha256.ComputeHash($bytes)
$hashString = [Convert]::ToBase64String($hash)

Write-Output "Password: $password"
Write-Output "Hash: $hashString"
