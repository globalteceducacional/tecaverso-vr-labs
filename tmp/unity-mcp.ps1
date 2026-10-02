#Requires -Version 5.1
<#
  unity-mcp.ps1 - minimal MCP JSON-RPC client for the Unity MCP server (Streamable HTTP).

  Usage:
    pwsh -File tmp/unity-mcp.ps1 -Action list
    pwsh -File tmp/unity-mcp.ps1 -Action call -Tool manage_gameobject -ArgsJson '{"action":"get","target":"Main Camera"}'
    pwsh -File tmp/unity-mcp.ps1 -Action call -Tool read_console -ArgsFile tmp/args.json
    pwsh -File tmp/unity-mcp.ps1 -Action resources
    pwsh -File tmp/unity-mcp.ps1 -Action read -Uri 'mcpforunity://instances'
    pwsh -File tmp/unity-mcp.ps1 -Action raw -Method tools/list -ParamsJson '{}'

  On this machine .ps1 execution is blocked by policy, so invoke via:
    powershell -NoProfile -ExecutionPolicy Bypass -File tmp\unity-mcp.ps1 ...
  Prefer -ArgsFile over -ArgsJson when the JSON contains quotes that the caller may mangle.
#>
[CmdletBinding()]
param(
  [string]$Endpoint = "http://127.0.0.1:8080/mcp",
  [ValidateSet("list", "call", "resources", "prompts", "read", "raw")]
  [string]$Action = "list",
  [string]$Tool,
  [string]$ArgsJson = "{}",
  [string]$ArgsFile,
  [string]$Method,
  [string]$ParamsJson = "{}",
  [string]$Uri,
  [int]$TimeoutSec = 120,
  [switch]$Raw
)

function Resolve-ArgsJson {
  param([string]$Inline, [string]$File)
  if (-not [string]::IsNullOrWhiteSpace($File)) {
    if (-not (Test-Path -LiteralPath $File)) { throw "Args file not found: $File" }
    return (Get-Content -LiteralPath $File -Raw -Encoding UTF8)
  }
  if ([string]::IsNullOrWhiteSpace($Inline)) { return "{}" }
  return $Inline
}

$ErrorActionPreference = "Stop"
$script:SessionId = $null
$script:NextId = 0

function Parse-SseBody {
  param([string]$Content)
  if ([string]::IsNullOrWhiteSpace($Content)) { return $null }
  $out = New-Object System.Collections.Generic.List[object]
  foreach ($line in ($Content -split "`r?`n")) {
    if ($line.StartsWith("data:")) {
      $json = $line.Substring(5).Trim()
      if ($json) { $out.Add(($json | ConvertFrom-Json)) }
    }
  }
  if ($out.Count -eq 0) {
    try { return ($Content | ConvertFrom-Json) } catch { return $null }
  }
  if ($out.Count -eq 1) { return $out[0] }
  return $out
}

function Invoke-Mcp {
  param(
    [string]$Method,
    $Params,
    [switch]$Notification
  )
  $script:NextId++
  $payload = [ordered]@{ jsonrpc = "2.0"; method = $Method }
  if (-not $Notification) { $payload.id = $script:NextId }
  if ($null -ne $Params) { $payload.params = $Params }

  $headers = @{
    "Accept" = "application/json, text/event-stream"
    "Content-Type" = "application/json"
  }
  if ($script:SessionId) { $headers["Mcp-Session-Id"] = $script:SessionId }

  $body = $payload | ConvertTo-Json -Depth 30 -Compress
  try {
    $resp = Invoke-WebRequest -Uri $Endpoint -Method POST -Body $body -Headers $headers -TimeoutSec $TimeoutSec -UseBasicParsing
  } catch {
    $r = $_.Exception.Response
    if ($r) {
      $sr = New-Object System.IO.StreamReader($r.GetResponseStream())
      $detail = $sr.ReadToEnd()
      throw "MCP HTTP $([int]$r.StatusCode) on '$Method': $detail"
    }
    throw
  }

  if (-not $script:SessionId) {
    $sid = $resp.Headers["Mcp-Session-Id"]
    if ($sid) { $script:SessionId = if ($sid -is [array]) { $sid[0] } else { $sid } }
  }
  if ($Notification) { return $null }
  return (Parse-SseBody -Content $resp.Content)
}

function Connect-Mcp {
  Invoke-Mcp -Method "initialize" -Params @{
    protocolVersion = "2024-11-05"
    capabilities    = @{}
    clientInfo      = @{ name = "dsh-unity-mcp"; version = "1.0" }
  } | Out-Null
  Invoke-Mcp -Method "notifications/initialized" -Params @{} -Notification | Out-Null
}

function Write-Result {
  param($Result)
  if ($Raw) { $Result | ConvertTo-Json -Depth 40; return }
  if ($Result -is [System.Collections.IEnumerable] -and $Result -isnot [string] -and $Result -isnot [pscustomobject]) {
    $Result | ConvertTo-Json -Depth 40
  } else {
    $Result | ConvertTo-Json -Depth 40
  }
}

Connect-Mcp

switch ($Action) {
  "list" {
    $res = Invoke-Mcp -Method "tools/list" -Params @{}
    $tools = $res.result.tools
    if ($Raw) { Write-Result $res; break }
    "TOOLS: $($tools.Count)"
    $tools | ForEach-Object {
      $desc = if ($_.description) { ($_.description -split "`n")[0].Trim() } else { "" }
      "  {0,-34} {1}" -f $_.name, $desc
    }
  }
  "resources" {
    $res = Invoke-Mcp -Method "resources/list" -Params @{}
    if ($Raw) { Write-Result $res; break }
    $items = $res.result.resources
    "RESOURCES: $($items.Count)"
    $items | ForEach-Object { "  {0,-52} {1}" -f $_.uri, $_.name }
    $tpl = Invoke-Mcp -Method "resources/templates/list" -Params @{}
    if ($tpl.result.resourceTemplates) {
      "TEMPLATES: $($tpl.result.resourceTemplates.Count)"
      $tpl.result.resourceTemplates | ForEach-Object { "  {0,-52} {1}" -f $_.uriTemplate, $_.name }
    }
  }
  "prompts" {
    $res = Invoke-Mcp -Method "prompts/list" -Params @{}
    if ($Raw) { Write-Result $res; break }
    "PROMPTS: $($res.result.prompts.Count)"
    $res.result.prompts | ForEach-Object { "  {0,-34} {1}" -f $_.name, $_.description }
  }
  "read" {
    if (-not $Uri) { throw "-Uri is required for -Action read" }
    $res = Invoke-Mcp -Method "resources/read" -Params @{ uri = $Uri }
    if ($Raw) { Write-Result $res; break }
    foreach ($c in $res.result.contents) {
      "--- $($c.uri) ($($c.mimeType)) ---"
      if ($c.text) { $c.text } else { "(binary blob $($c.blob.Length) chars base64)" }
    }
  }
  "call" {
    if (-not $Tool) { throw "-Tool is required for -Action call" }
    $argJson = Resolve-ArgsJson -Inline $ArgsJson -File $ArgsFile
    $argObj = if ([string]::IsNullOrWhiteSpace($argJson)) { @{} } else { $argJson | ConvertFrom-Json }
    $res = Invoke-Mcp -Method "tools/call" -Params @{ name = $Tool; arguments = $argObj }
    if ($Raw) { Write-Result $res; break }
    if ($res.error) { "MCP ERROR: $($res.error.message)"; break }
    $r = $res.result
    if ($r.isError) { "TOOL ERROR:" }
    foreach ($c in $r.content) {
      if ($c.type -eq "text") { $c.text }
      elseif ($c.type -eq "image") { "(image content: $($c.mimeType), $($c.data.Length) chars base64)" }
      else { $c | ConvertTo-Json -Depth 20 }
    }
    if ($r.structuredContent) {
      "--- structuredContent ---"
      $r.structuredContent | ConvertTo-Json -Depth 40
    }
  }
  "raw" {
    if (-not $Method) { throw "-Method is required for -Action raw" }
    $p = if ([string]::IsNullOrWhiteSpace($ParamsJson)) { @{} } else { $ParamsJson | ConvertFrom-Json }
    $res = Invoke-Mcp -Method $Method -Params $p
    Write-Result $res
  }
}
