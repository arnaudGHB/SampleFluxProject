# Impact du retrait de BankId

## Composants affectés

1. **Entités** :
   - `BaseEntity` (retrait de la propriété BankId)
   - `User` (hérite de BaseEntity)

2. **Migrations EF Core** :
   - Suppression de la colonne BankId dans la table Users
   - Nécessité de générer une nouvelle migration

3. **DTOs et Commandes** :
   - `UserDto`
   - `AddUserCommand`
   - `UpdateUserCommand`

4. **Mappings AutoMapper** :
   - `UserMappingProfile`
   - `MapperConfig`

5. **Validateurs FluentValidation** :
   - `AddUserCommandValidator`
   - `UpdateUserCommandValidator`

6. **Handlers MediatR** :
   - `AddUserCommandHandler`
   - `UpdateUserCommandHandler`

7. **Base de données** :
   - Suppression de la colonne BankId
   - Perte des données existantes dans cette colonne

## Étapes de mise en œuvre

1. Retirer BankId de toutes les entités
2. Générer une migration EF Core (`RemoveBankIdFromUser`)
3. Mettre à jour les DTOs et commandes
4. Mettre à jour les mappings AutoMapper
5. Mettre à jour les validateurs
6. Mettre à jour les handlers
7. Appliquer la migration
8. Tester les endpoints concernés

## Risques

- **Perte de données** : Les valeurs BankId existantes seront définitivement perdues
- **Compatibilité** : Aucun impact sur les autres microservices car BankId n'était pas encore utilisé
- **Temps d'arrêt** : Nécessaire pour appliquer la migration

## Recommandation

**Procéder au retrait** car BankId n'est pas utilisé dans les cas d'utilisation actuels.
