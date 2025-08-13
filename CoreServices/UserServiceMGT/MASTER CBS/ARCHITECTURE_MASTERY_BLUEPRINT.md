# Blueprint de Maîtrise de l'Architecture Microservices FLX/CBS

Ce document est le guide ultime pour tout architecte ou développeur senior souhaitant maîtriser notre écosystème. L'objectif n'est pas de tout lire d'un coup, mais de comprendre la hiérarchie de la connaissance.

### Étape 1 : Comprendre la Vision Globale (La Carte)
*Commencez par ces documents pour avoir une vue d'ensemble.*
1.  **La Carte du Monde :** `Dissection-Architecture-Globale.md`
2.  **Les Lois du Monde :** `Architecture-Reference.md`
3.  **Les 10 Commandements :** `PILIERS_ARCHITECTURE_FONDAMENTAUX.md`

> **À la fin de cette étape, vous devez être capable de dessiner l'architecture de haut niveau et d'expliquer le rôle de chaque couche.**

### Étape 2 : Disséquer un Spécimen Parfait (Le Modèle)
*La théorie prend vie dans le code. Étudiez notre microservice de référence.*
4.  **Le Code de Référence :** `etat stable UserServicemanagement.md`

> **À la fin de cette étape, vous devez comprendre comment les lois de l'architecture sont appliquées dans du code C# réel, y compris les versions exactes des dépendances.**

### Étape 3 : Zoom sur les Organes Vitaux (Les Mécanismes Clés)
*Certains composants sont plus complexes que d'autres. Étudiez-les en détail.*
5.  **Le Gardien :** `Dissection-API-Gateway.md`
6.  **Le Cerveau :** `Dissection-CustomerManagement-MediatR.md`
7.  **Le Bouclier :** `SECURITE_MICROSERVICES_EXPLICITE.md`

> **À la fin de cette étape, vous devez maîtriser la configuration Ocelot, le pipeline MediatR avec ses behaviors, et la chaîne de sécurité JWT de bout en bout.**

### Étape 4 : Apprendre à Construire (Le Guide de Montage)
*Maintenant que vous savez tout, voici comment en construire un nouveau.*
8.  **Le Plan de Montage :** `GUIDE_IMPLANTATION_MICROSERVICE.md`

> **À la fin de cette étape, vous devez être capable de créer un nouveau microservice 100% conforme à l'architecture, sans supervision.**

---
**Règle d'Or :** En cas de doute entre un document théorique et le code du "Golden Standard" (`etat stable UserServicemanagement.md`), **c'est toujours le code qui a raison.**