# ==============================================================================
# SCRIPT DE DUPLICATION ET RENOMMAGE INTELLIGENT DE MICROSERVICE
# ==============================================================================
$ErrorActionPreference = "Stop" # Arrête le script en cas d'erreur

# --- [ÉTAPE 0] Paramètres de l'opération ---
$sourceServiceName = "UserServiceManagement"
$targetServiceName = "UserServiceMGT"
$solutionFile = "CBSManagementService.sln" # Adaptez si nécessaire

$sourcePath = "CoreServices\$sourceServiceName"
$targetPath = "CoreServices\$targetServiceName"

Write-Host "--- DÉBUT DE LA DUPLICATION ---" -ForegroundColor Cyan
Write-Host "Source: $sourceServiceName"
Write-Host "Cible:  $targetServiceName"
Write-Host "----------------------------------"

# Vérification de sécurité
if (-not (Test-Path -Path $sourcePath)) {
    Write-Error "ERREUR: Le répertoire source '$sourcePath' n'existe pas. Annulation."
    exit 1
}
if (Test-Path -Path $targetPath) {
    Write-Error "ERREUR: Le répertoire cible '$targetPath' existe déjà. Annulation."
    exit 1
}

# --- [ÉTAPE 1] Copie de l'intégralité de la structure ---
Write-Host "[1/4] Copie de l'arborescence complète de $sourceServiceName vers $targetServiceName..."
Copy-Item -Path $sourcePath -Destination $targetPath -Recurse -Force
Write-Host "SUCCESS: Copie terminée." -ForegroundColor Green

# --- [ÉTAPE 2] Renommage des fichiers et des dossiers ---
Write-Host "[2/6] Renommage des fichiers et dossiers contenant '$sourceServiceName'..."

# D'abord, traiter les dossiers (en ordre de profondeur décroissante)
$folders = Get-ChildItem -Path $targetPath -Recurse -Directory | 
    Where-Object { $_.Name -like "*$sourceServiceName*" } | 
    Sort-Object -Property FullName -Descending

foreach ($folder in $folders) {
    try {
        $newName = $folder.Name.Replace($sourceServiceName, $targetServiceName)
        $newPath = Join-Path -Path $folder.Parent.FullName -ChildPath $newName
        
        if (-not (Test-Path -Path $newPath)) {
            Rename-Item -Path $folder.FullName -NewName $newName -Force -ErrorAction Stop
            Write-Host "  - Dossier renommé: $($folder.Name) -> $newName"
        }
    }
    catch {
        Write-Warning "  - Impossible de renommer le dossier: $($folder.Name) - $($_.Exception.Message)"
    }
}

# Ensuite, traiter les fichiers
$files = Get-ChildItem -Path $targetPath -Recurse -File | 
    Where-Object { $_.Name -like "*$sourceServiceName*" }

foreach ($file in $files) {
    try {
        $newName = $file.Name.Replace($sourceServiceName, $targetServiceName)
        $newPath = Join-Path -Path $file.Directory.FullName -ChildPath $newName
        
        # Vérifier si le fichier de destination existe déjà
        if (Test-Path -Path $newPath) {
            Remove-Item -Path $file.FullName -Force -ErrorAction Stop
            Write-Host "  - Fichier existant supprimé: $($file.Name)"
        }
        else {
            Rename-Item -Path $file.FullName -NewName $newName -Force -ErrorAction Stop
            Write-Host "  - Fichier renommé: $($file.Name) -> $newName"
        }
    }
    catch {
        Write-Warning "  - Impossible de renommer le fichier: $($file.Name) - $($_.Exception.Message)"
    }
}
Write-Host "SUCCESS: Renommage des fichiers et dossiers terminé." -ForegroundColor Green

# --- [ÉTAPE 3] Remplacement du contenu (Namespaces et références) ---
Write-Host "[3/4] Adaptation du contenu des fichiers (namespaces, références de projet)..."
$filesToProcess = Get-ChildItem -Path $targetPath -Recurse -Include "*.cs", "*.csproj", "*.json", "*.sln", "*.cshtml", "*.config", "*.xml" -Exclude "bin", "obj", "packages"

foreach ($file in $filesToProcess) {
    try {
        # Ignorer les fichiers binaires
        if ($file.Extension -match '\.(dll|exe|pdb|nupkg|snk|resources|resx|cache|cache$)' -or 
            $file.DirectoryName -match '\\obj\\|\\bin\\|\\packages\\|\\.vs\\') {
            continue
        }
        
        # Lire le contenu du fichier
        $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
        
        # Effectuer les remplacements
        $newContent = $content.Replace($sourceServiceName, $targetServiceName)
        
        # Si le contenu a changé, écrire le nouveau contenu
        if ($content -ne $newContent) {
            [System.IO.File]::WriteAllText($file.FullName, $newContent, [System.Text.Encoding]::UTF8)
            Write-Host "  - Modifié: $($file.FullName.Substring($PWD.Path.Length + 1))"
        }
    }
    catch {
        Write-Warning "  - Impossible de traiter le fichier: $($file.FullName) - $($_.Exception.Message)"
    }
}
Write-Host "SUCCESS: Contenu des fichiers adapté." -ForegroundColor Green

# --- [ÉTAPE 4] Nettoyage des dossiers de sortie ---
Write-Host "[4/6] Nettoyage des dossiers bin et obj..."
Get-ChildItem -Path $targetPath -Recurse -Directory | 
    Where-Object { $_.Name -match '^(bin|obj|TestResults|_ReSharper\.*|.vs)$' } | 
    ForEach-Object {
        try {
            Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction Stop
            Write-Host "  - Supprimé: $($_.FullName.Substring($PWD.Path.Length + 1))"
        }
        catch {
            Write-Warning "  - Impossible de supprimer: $($_.FullName) - $($_.Exception.Message)"
        }
    }

# --- [ÉTAPE 5] Mise à jour des GUIDs de projet ---
Write-Host "[5/6] Génération de nouveaux GUIDs pour les projets..."
$csprojFiles = Get-ChildItem -Path $targetPath -Filter "*.csproj" -Recurse
foreach ($csproj in $csprojFiles) {
    try {
        $content = [System.IO.File]::ReadAllText($csproj.FullName)
        $newGuid = [System.Guid]::NewGuid().ToString("B").ToUpper()
        
        # Mettre à jour le ProjectGuid
        $newContent = [System.Text.RegularExpressions.Regex]::Replace(
            $content, 
            '<ProjectGuid>\{?[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\}?<\/ProjectGuid>',
            "<ProjectGuid>{$newGuid}</ProjectGuid>"
        )
        
        if ($content -ne $newContent) {
            [System.IO.File]::WriteAllText($csproj.FullName, $newContent, [System.Text.Encoding]::UTF8)
            Write-Host "  - Nouveau GUID pour: $($csproj.Name)"
        }
    }
    catch {
        Write-Warning "  - Impossible de mettre à jour le GUID pour: $($csproj.FullName) - $($_.Exception.Message)"
    }
}

# --- [ÉTAPE 6] Intégration du nouveau microservice à la solution ---
Write-Host "[6/6] Ajout des nouveaux projets au fichier de solution '$solutionFile'..."
$newProjects = Get-ChildItem -Path $targetPath -Filter "*.csproj" -Recurse
foreach ($project in $newProjects) {
    try {
        $relativePath = $project.FullName.Substring($PWD.Path.Length + 1)
        & dotnet sln $solutionFile add $relativePath | Out-Null
        Write-Host "  - Ajouté: $($project.Name) à la solution."
    }
    catch {
        Write-Warning "  - Impossible d'ajouter le projet à la solution: $($project.Name) - $($_.Exception.Message)"
    }
}

# --- FIN ---
Write-Host "========================================================" -ForegroundColor Green
Write-Host "--- OPÉRATION TERMINÉE AVEC SUCCÈS ---" -ForegroundColor Green
Write-Host "Le microservice '$targetServiceName' a été créé avec succès." -ForegroundColor Green
Write-Host "Emplacement: $((Resolve-Path $targetPath).Path)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Prochaines étapes recommandées:" -ForegroundColor Yellow
Write-Host "1. Ouvrez la solution dans Visual Studio"
Write-Host "2. Rechargez la solution pour voir les changements"
Write-Host "3. Vérifiez les références de projet"
Write-Host "4. Générez la solution pour vérifier les erreurs"
Write-Host "5. Mettez à jour les chaînes de connexion et configurations spécifiques"
Write-Host "========================================================" -ForegroundColor Green
