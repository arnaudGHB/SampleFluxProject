# ====================================================================================
# SCRIPT DE GÉNÉRATION ADAPTATIVE DE MICROSERVICE
# ====================================================================================
$ErrorActionPreference = "Stop"

# --- [ÉTAPE 0] Paramètres ---
$sourceServiceName = "UserServiceManagement"
$targetServiceName = "UserSkillManagement"
$solutionFile = "CBSManagementService.sln"

$sourcePath = "CoreServices\$sourceServiceName"
$targetPath = "CoreServices\$targetServiceName"

Write-Host "--- DÉBUT DE LA GÉNÉRATION ADAPTATIVE ---" -ForegroundColor Cyan
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

# --- [ÉTAPE 1] Duplication et Renommage ---
Write-Host "[1/5] Copie et renommage de la structure de base..."
Copy-Item -Path $sourcePath -Destination $targetPath -Recurse -Force

# Renommer les fichiers et dossiers
Get-ChildItem -Path $targetPath -Recurse | Where-Object { $_.Name -like "*$sourceServiceName*" } | ForEach-Object {
    $newName = $_.Name.Replace($sourceServiceName, $targetServiceName)
    Rename-Item -Path $_.FullName -NewName $newName -Force
}

# Mettre à jour le contenu des fichiers
Get-ChildItem -Path $targetPath -Recurse -Include "*.cs", "*.csproj", "*.json", "*.sln" | ForEach-Object {
    try {
        $content = [System.IO.File]::ReadAllText($_.FullName)
        $newContent = $content.Replace($sourceServiceName, $targetServiceName)
        if ($content -ne $newContent) {
            [System.IO.File]::WriteAllText($_.FullName, $newContent, [System.Text.Encoding]::UTF8)
        }
    } catch {
        Write-Warning "  - Impossible de traiter le fichier: $($_.FullName) - $($_.Exception.Message)"
    }
}

Write-Host "SUCCESS: Duplication et renommage terminés." -ForegroundColor Green

# --- [ÉTAPE 2] Nettoyage de la Logique Métier Spécifique "User" ---
Write-Host "[2/5] Nettoyage de la logique métier spécifique..."

# Supprimer les fichiers spécifiques à User
$itemsToRemove = @(
    "$targetPath\CBS.$targetServiceName.API\Controllers\UsersController.cs",
    "$targetPath\CBS.$targetServiceName.MediatR\User",
    "$targetPath\CBS.$targetServiceName.Data\Entity\User.cs",
    "$targetPath\CBS.$targetServiceName.Data\Entity\AuditLog.cs",
    "$targetPath\CBS.$targetServiceName.Data\Dto\*",
    "$targetPath\CBS.$targetServiceName.Repository\User",
    "$targetPath\CBS.$targetServiceName.API\Helpers\MappingProfile\UserMappingProfile.cs"
)

foreach ($item in $itemsToRemove) {
    Remove-Item -Path $item -Recurse -Force -ErrorAction SilentlyContinue
}

# Supprimer les migrations existantes
Remove-Item -Path "$targetPath\CBS.$targetServiceName.Domain\Migrations\*" -Recurse -Force -ErrorAction SilentlyContinue

# Créer les dossiers nécessaires
$dirsToCreate = @(
    "$targetPath\CBS.$targetServiceName.Data\Dto",
    "$targetPath\CBS.$targetServiceName.MediatR\Skill\Commands",
    "$targetPath\CBS.$targetServiceName.MediatR\Skill\Queries",
    "$targetPath\CBS.$targetServiceName.MediatR\Skill\Handlers",
    "$targetPath\CBS.$targetServiceName.MediatR\Skill\Validators"
)

foreach ($dir in $dirsToCreate) {
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
}

Write-Host "SUCCESS: Nettoyage terminé." -ForegroundColor Green

# --- [ÉTAPE 3] Injection des Squelettes Métier ---
Write-Host "[3/5] Injection des squelettes métier..."

# Créer les entités
$entityPath = "$targetPath\CBS.$targetServiceName.Data\Entity"

# Skill.cs
@"
using System;
using CBS.$targetServiceName.Data.Interfaces;

namespace CBS.$targetServiceName.Data.Entity
{
    public class Skill : BaseEntity, IAuditable
    {
        public Guid SkillId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
"@ | Out-File -FilePath "$entityPath\Skill.cs" -Encoding UTF8

# UserSkill.cs
@"
using System;
using CBS.$targetServiceName.Data.Interfaces;

namespace CBS.$targetServiceName.Data.Entity
{
    public class UserSkill : BaseEntity, IAuditable
    {
        public Guid UserSkillId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid SkillId { get; set; }
        public string Level { get; set; }
        public string Experience { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual Skill Skill { get; set; }
    }
}
"@ | Out-File -FilePath "$entityPath\UserSkill.cs" -Encoding UTF8

# Créer le DbContext
$contextContent = @"
using Microsoft.EntityFrameworkCore;
using CBS.$targetServiceName.Data.Entity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.$targetServiceName.Domain.Context
{
    public class UserSkillContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public UserSkillContext(DbContextOptions<UserSkillContext> options, ICurrentUserService currentUserService) 
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(e => e.SkillId);
                entity.Property(e => e.SkillId).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<UserSkill>(entity =>
            {
                entity.HasKey(e => e.UserSkillId);
                entity.Property(e => e.UserSkillId).ValueGeneratedNever();
                entity.Property(e => e.Level).HasMaxLength(50);
                entity.Property(e => e.Experience).HasMaxLength(1000);
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.HasOne(us => us.Skill)
                    .WithMany()
                    .HasForeignKey(us => us.SkillId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
"@

$contextPath = "$targetPath\CBS.$targetServiceName.Domain\Context\UserSkillContext.cs"
$contextContent | Out-File -FilePath $contextPath -Encoding UTF8

Write-Host "SUCCESS: Squelettes métier injectés." -ForegroundColor Green

# --- [ÉTAPE 4] Configuration et DI ---
Write-Host "[4/5] Adaptation de la configuration..."

# Adapter Startup.cs
$startupFile = "$targetPath\CBS.$targetServiceName.API\Startup.cs"
$startupContent = Get-Content $startupFile -Raw
$startupContent = $startupContent -replace "UserContext", "UserSkillContext"
$startupContent = $startupContent -replace "IUserRepository", "// TODO: IYourRepository"
$startupContent = $startupContent -replace "UserRepository", "// TODO: YourRepository"
$startupContent | Set-Content $startupFile -Encoding UTF8

# --- [ÉTAPE 5] Intégration à la Solution ---
Write-Host "[5/5] Ajout des projets à la solution..."
$newProjects = Get-ChildItem -Path $targetPath -Filter "*.csproj" -Recurse
foreach ($project in $newProjects) {
    dotnet sln $solutionFile add $project.FullName | Out-Null
    Write-Host "  - Ajouté: $($project.Name) à la solution."
}

Write-Host "========================================================" -ForegroundColor Green
Write-Host "--- OPÉRATION TERMINÉE AVEC SUCCÈS ---" -ForegroundColor Green
Write-Host "Le microservice '$targetServiceName' a été généré." -ForegroundColor Green
Write-Host "Emplacement: $((Resolve-Path $targetPath).Path)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Prochaines étapes recommandées:" -ForegroundColor Yellow
Write-Host "1. Ouvrez la solution dans Visual Studio"
Write-Host "2. Générez la solution pour vérifier les erreurs"
Write-Host "3. Créez et appliquez une migration initiale"
Write-Host "4. Implémentez les contrôleurs et services manquants"
Write-Host "5. Testez les points de terminaison de l'API"
Write-Host "========================================================" -ForegroundColor Green
