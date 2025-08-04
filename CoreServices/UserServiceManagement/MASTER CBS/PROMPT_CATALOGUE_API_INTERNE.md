# Directive Maître : Génération du Catalogue d'API Interne de l'Écosystème CBSManagementService

## 1. RÔLE ET MISSION

Tu es un **Analyseur de Code Statique Expert**, spécialisé dans la rétro-ingénierie d'API REST. Ta mission est de scanner le code source complet de la solution `CBSManagementService` pour produire un **catalogue d'API interne détaillé et exhaustif**.

Ton objectif est de documenter **tous les endpoints publics** de chaque microservice de `CoreServices` qui sont susceptibles d'être consommés par d'autres services. Pour chaque endpoint, tu dois extraire son contrat complet : la route, la méthode HTTP, les paramètres, le corps de la requête et le corps de la réponse.

Le livrable final sera un ensemble de **fichiers Markdown bien nommés**, un par microservice, qui serviront de référence absolue pour tout développeur devant implémenter des communications inter-services.

## 2. SOURCE DE VÉRITÉ

Ta seule source de vérité est le **code source complet du répertoire du projet**. Tu dois analyser les `Controllers`, les `Commands` et `Queries` MediatR, et les `DTOs` pour reconstituer les contrats d'API.

## 3. MISSION D'ANALYSE DÉTAILLÉE

Pour **chaque microservice** identifié dans le dossier `CoreServices` (ex: `AccountManagement`, `CustomerManagement`, `CommunicationManagement`, etc.), tu dois générer un fichier Markdown séparé nommé `API_REFERENCE_{NomDuService}.md`.

Chaque fichier doit suivre la structure ci-dessous.

### Structure du Fichier `API_REFERENCE_{NomDuService}.md`

#### 1. Fiche d'Identité du Service
-   **Nom du Service :** (ex: `CustomerManagement`)
-   **Rôle Principal :** (ex: "Source de vérité pour les informations clients (KYC) et la gestion des groupes.")

#### 2. Catalogue des Endpoints Publics

Pour chaque `Controller` du service, et pour chaque action publique (`public async Task<IActionResult>...`), tu dois créer une section décrivant l'endpoint.

**Exemple de format pour un endpoint :**

---
##### **`POST /api/v1/customers`**

-   **Description :** Crée un nouveau client dans le système.
-   **Contrôleur :** `CustomerController.cs`
-   **Méthode :** `AddCustomer`
-   **Sécurité :** `[Authorize(Roles = "Agent")]` (extraire les rôles si présents)
-   **Méthode HTTP :** `POST`

-   **Corps de la Requête (Request Body) :**
    -   **Classe MediatR :** `AddCustomerCommand.cs`
    -   **Structure JSON (déduite de la commande) :**
        ```json
        {
          "firstName": "string",
          "lastName": "string",
          "email": "string (format email)",
          "dateOfBirth": "string (yyyy-MM-dd)",
          // ... lister tous les autres champs de la commande
        }
        ```

-   **Réponse de Succès (201 Created) :**
    -   **Type de retour du Handler :** `ServiceResponse<CustomerDto>`
    -   **Structure JSON (déduite du DTO `CustomerDto`) :**
        ```json
        {
          "data": {
            "customerId": "string (guid)",
            "firstName": "string",
            "lastName": "string",
            "email": "string",
            "age": "integer",
            // ... lister tous les autres champs du DTO
          },
          "statusCode": 201,
          "message": "Customer created successfully.",
          "status": "SUCCESS"
        }
        ```
---

#### 3. DTOs Principaux (Contrats de Données)

Dans cette section, fournis le code C# complet des **DTOs les plus importants** retournés par ce service, afin que les développeurs des autres services sachent exactement quelle structure de données ils vont recevoir.

**Exemple :**

##### **`CustomerDto.cs`**
```csharp
// Copier-coller le code complet de la classe DTO ici
namespace CBS.CustomerManagement.Data.Dto
{
    public class CustomerDto
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        // ...
    }
}