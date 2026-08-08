<#
.SYNOPSIS
    Instala la coleccion ESAPI-Snippets en la carpeta de snippets de usuario de
    Visual Studio.

.DESCRIPTION
    Visual Studio carga automaticamente los snippets que encuentra en
    Documents\Visual Studio <version>\Code Snippets\Visual C#\My Code Snippets,
    sin necesidad de registrarlos por el Administrador de fragmentos de codigo.

    Este script detecta las versiones de Visual Studio instaladas para el
    usuario actual y copia ahi el contenido de snippets\, conservando las
    subcarpetas por categoria.

    La copia se hace con robocopy en vez de Copy-Item: es mas robusto ante
    carpetas de destino recien creadas y ante retrasos de sincronizacion
    cuando "Documentos" vive dentro de OneDrive.

.PARAMETER VisualStudioVersion
    Version concreta a la que instalar (por ejemplo 2022). Si se omite, se
    instala en todas las versiones detectadas.

.PARAMETER SubFolder
    Nombre de la carpeta destino dentro de My Code Snippets.
    Por defecto, ESAPI-Snippets.

.PARAMETER Force
    Sobrescribe una instalacion previa sin preguntar.

.EXAMPLE
    .\tools\Install-Snippets.ps1

.EXAMPLE
    .\tools\Install-Snippets.ps1 -VisualStudioVersion 2022 -Force

.NOTES
    Cierra Visual Studio antes de ejecutarlo: los snippets se indexan al
    arrancar el IDE.
#>

[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string] $VisualStudioVersion,
    [string] $SubFolder = 'ESAPI-Snippets',
    [switch] $Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$sourceDir = Join-Path $repoRoot 'snippets'

if (-not (Test-Path -LiteralPath $sourceDir)) {
    throw "No se encontro la carpeta de snippets en '$sourceDir'. Ejecuta el script desde el repositorio clonado."
}

$documents = [Environment]::GetFolderPath('MyDocuments')
if ([string]::IsNullOrWhiteSpace($documents)) {
    throw 'No se pudo resolver la carpeta Documentos del usuario actual.'
}

# Las carpetas se llaman "Visual Studio 2019", "Visual Studio 2022", etc.
$candidates = Get-ChildItem -LiteralPath $documents -Directory -Filter 'Visual Studio *' -ErrorAction SilentlyContinue

if ($VisualStudioVersion) {
    $candidates = $candidates | Where-Object { $_.Name -eq "Visual Studio $VisualStudioVersion" }

    if (-not $candidates) {
        throw "No se encontro 'Visual Studio $VisualStudioVersion' en '$documents'."
    }
}

if (-not $candidates) {
    throw "No se detecto ninguna instalacion de Visual Studio en '$documents'. Usa -VisualStudioVersion o instala manualmente (ver docs/installation-guide.md)."
}

$installedCount = 0

foreach ($candidate in $candidates) {
    $target = Join-Path $candidate.FullName 'Code Snippets\Visual C#\My Code Snippets'
    $destination = Join-Path $target $SubFolder

    if (-not (Test-Path -LiteralPath $target)) {
        Write-Verbose "Creando '$target'."
        New-Item -ItemType Directory -Path $target -Force | Out-Null
    }

    if ((Test-Path -LiteralPath $destination) -and -not $Force) {
        $answer = Read-Host "Ya existe '$destination'. Sobrescribir? (s/N)"
        if ($answer -notmatch '^[sSyY]$') {
            Write-Host "Omitido: $($candidate.Name)" -ForegroundColor Yellow
            continue
        }
    }

    if ($PSCmdlet.ShouldProcess($destination, 'Instalar ESAPI-Snippets')) {
        if (Test-Path -LiteralPath $destination) {
            Remove-Item -LiteralPath $destination -Recurse -Force
        }

        # Copy-Item -Recurse copiando muchos archivos a una carpeta recien
        # creada puede fallar con FileNotFoundException de forma intermitente
        # (bug conocido de Windows PowerShell, agravado si el destino esta
        # dentro de una carpeta sincronizada por OneDrive, que puede
        # bloquear/retrasar los archivos mientras se escriben). robocopy es
        # la herramienta estandar de Windows para copias recursivas y trae
        # reintentos automaticos, por lo que resuelve el problema de raiz.
        New-Item -ItemType Directory -Path $destination -Force | Out-Null

        $roboOutput = & robocopy $sourceDir $destination /E /R:3 /W:1 /NFL /NDL /NJH /NJS /NC /NP
        if ($LASTEXITCODE -ge 8) {
            throw "robocopy fallo copiando a '$destination' (codigo de salida $LASTEXITCODE).`n$roboOutput"
        }

        $count = (Get-ChildItem -LiteralPath $destination -Recurse -Filter '*.snippet').Count
        Write-Host "Instalados $count snippets en $($candidate.Name)." -ForegroundColor Green
        Write-Host "  $destination" -ForegroundColor DarkGray
        $installedCount++
    }
}

if ($installedCount -gt 0) {
    Write-Host ''
    Write-Host 'Reinicia Visual Studio para que indexe los snippets.' -ForegroundColor Cyan
}
