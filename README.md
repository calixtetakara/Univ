# UnivSystem

Plateforme de gestion universitaire couvrant l'intégralité du cycle de vie étudiant (de l'inscription au paiement) et l'administration académique, avec un système de contrôle d'accès basé sur les rôles (RBAC) avancé.

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blueviolet?style=flat-square)
![MariaDB 10.4.32](https://img.shields.io/badge/MariaDB-10.4.32-blue?style=flat-square)
![Architecture N-Tier](https://img.shields.io/badge/Architecture-N--Tier-green?style=flat-square)
![Sécurité OWASP Top 10](https://img.shields.io/badge/S%C3%A9curit%C3%A9-OWASP%20Top%2010-red?style=flat-square)

## Table des matières

1. [Présentation & Fonctionnalités](#1-présentation--fonctionnalités)
2. [Architecture & Stack Technique](#2-architecture--stack-technique)
3. [Prérequis](#3-prérequis)
4. [Installation & Configuration](#4-installation--configuration)
5. [Lancement de l'application](#5-lancement-de-lapplication)
6. [Identifiants de connexion (Comptes de test)](#6-identifiants-de-connexion)
7. [Tests & Qualité](#7-tests--qualité)
8. [Structure du dépôt](#8-structure-du-dépôt)
9. [Documentation](#9-documentation)

---

## 1. Présentation & Fonctionnalités

UnivSystem est une application web développée avec **ASP.NET Core 8 (MVC)**. Elle centralise les processus métiers d'une université et est divisée en **6 espaces distincts** :

- **Candidat** : Inscription en 3 étapes (Wizard) avec validation stricte des fichiers (MIME, taille).
- **Étudiant** : Suivi de dossier, paiement en ligne (via Webhook), téléchargement du reçu PDF.
- **Scolarité** : Traitement des dossiers (Approuver/Rejeter), configuration académique (Facultés, Filières, UE).
- **Pédagogie** : Création des enseignants (génération de mot de passe temporaire) et planification des cours.
- **Enseignant** : Consultation du planning personnel et des effectifs (avec configuration forcée du mot de passe).
- **Super-Admin** : Matrice RBAC dynamique (protection anti-lockout), pilotage financier et KPI en temps réel.

## 2. Architecture & Stack Technique

Le projet respecte une architecture **N-Tier** stricte avec une séparation claire des responsabilités (Clean Architecture simplifiée) :

```text
Domain ← Application ← Infrastructure ← Web
```

| Composant | Technologie |
|---|---|
| Framework | ASP.NET Core 8.0 (MVC) |
| Base de données | MariaDB 10.4.32 (via Pomelo / EF Core) |
| Authentification | Cookie Authentication (Claims-based) |
| Tâches de fond | Hangfire (stockage MySQL/MariaDB) |
| Génération de documents | QuestPDF (PDF), ClosedXML (Excel), CsvHelper |
| Sécurité | BCrypt.Net-Next, Mime-Detective, Rate Limiting |

## 3. Prérequis

Avant de cloner et lancer le projet, assurez-vous d'avoir installé :

- **.NET SDK** (version utilisée : 10.0.403 ; le projet cible .NET 8.0)
- **MariaDB 10.4.32** (Installation native requise, pas de Docker), administrable via phpMyAdmin
- Visual Studio 2022 ou VS Code

## 4. Installation & Configuration

### 4.1. Cloner le dépôt

```bash
git clone https://github.com/calixtetakara/Univ.git
cd Univ
```

### 4.2. Configurer la base de données

Connectez-vous à votre serveur MariaDB et créez les bases (encodage `utf8mb4` obligatoire) :

```sql
CREATE DATABASE univ_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE univ_test CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Configurez les secrets locaux (`user-secrets`) depuis le projet Web :

```bash
cd src/UniversitySystem.Web
dotnet user-secrets set "ConnectionStrings:Default" "Server=127.0.0.1;Database=univ_db;User=root;Password=VotreMotDePasse;"
dotnet user-secrets set "ConnectionStrings:Test" "Server=127.0.0.1;Database=univ_test;User=root;Password=VotreMotDePasse;"
dotnet user-secrets set "Storage:RootPath" "C:/univ-storage"
dotnet user-secrets set "Webhook:Secret" "UneCleSecreteDe32CaracteresMinimum!"
```

### 4.3. Appliquer les migrations

```bash
cd src/UniversitySystem.Web
dotnet ef database update -p ../UniversitySystem.Infrastructure
```

## 5. Lancement de l'application

```bash
dotnet run
```

L'application sera accessible sur `https://localhost:5074` (ou le port indiqué dans votre terminal).

## 6. Identifiants de connexion

> **Information importante** : Au premier démarrage, le `DataSeeder` crée le compte Super-Admin. Pour les autres rôles, vous devez les créer manuellement (via l'interface ou les requêtes SQL ci-dessous).

**Mot de passe universel pour tous les comptes de test :** `Admin@2026!`

| Rôle | Email | Création |
|---|---|---|
| Super-Admin | `admin@univ.local` | Créé automatiquement par le Seeder |
| Scolarité | `scolarite.test@univ.local` | À créer via SQL |
| Pédagogie | `pedagogie.test@univ.local` | À créer via SQL |
| Directeur | `directeur.test@univ.local` | À créer via SQL |
| Enseignant | `enseignant.test@univ.local` | À créer via SQL |
| Étudiant | (Créé via le formulaire d'inscription) | Route `/Registration/Step1` |

### Requêtes SQL d'initialisation

Ouvrez phpMyAdmin ou votre terminal MariaDB, sélectionnez la base `univ_db` et exécutez ce script. Il copie le hash sécurisé du mot de passe de l'administrateur pour créer les autres comptes.

```sql
-- 1. Créer l'agent de Scolarité
INSERT INTO utilisateurs (email, mot_de_passe_hash, nom, prenom, statut, doit_changer_mot_de_passe, date_creation, id_role)
SELECT 'scolarite.test@univ.local', mot_de_passe_hash, 'Diallo', 'Aminata', 1, 0, NOW(), (SELECT id_role FROM roles WHERE code = 'scolarite')
FROM utilisateurs WHERE email = 'admin@univ.local';

-- 2. Créer l'agent de Pédagogie
INSERT INTO utilisateurs (email, mot_de_passe_hash, nom, prenom, statut, doit_changer_mot_de_passe, date_creation, id_role)
SELECT 'pedagogie.test@univ.local', mot_de_passe_hash, 'Durand', 'Sophie', 1, 0, NOW(), (SELECT id_role FROM roles WHERE code = 'pedagogie')
FROM utilisateurs WHERE email = 'admin@univ.local';

-- 3. Créer un Directeur
INSERT INTO utilisateurs (email, mot_de_passe_hash, nom, prenom, statut, doit_changer_mot_de_passe, date_creation, id_role)
SELECT 'directeur.test@univ.local', mot_de_passe_hash, 'Martin', 'Paul', 1, 0, NOW(), (SELECT id_role FROM roles WHERE code = 'directeur')
FROM utilisateurs WHERE email = 'admin@univ.local';

-- 4. Créer un Enseignant (Utilisateur + Profil Enseignant)
INSERT INTO utilisateurs (email, mot_de_passe_hash, nom, prenom, statut, doit_changer_mot_de_passe, date_creation, id_role)
SELECT 'enseignant.test@univ.local', mot_de_passe_hash, 'Koffi', 'Jean', 1, 0, NOW(), (SELECT id_role FROM roles WHERE code = 'enseignant')
FROM utilisateurs WHERE email = 'admin@univ.local';

INSERT INTO enseignants (id_utilisateur, matricule_enseignant, grade)
SELECT id_utilisateur, 'ENS-2026-001', 'Assistant' FROM utilisateurs WHERE email = 'enseignant.test@univ.local';
```

## 7. Tests & Qualité

### 7.1. Lancer les tests (Unitaires & Intégration)

```bash
dotnet test
```

> **Note** : Les tests d'intégration utilisent MariaDB/MySQL et l'outil Respawn pour réinitialiser la base de données entre chaque test. Assurez-vous que la chaîne `ConnectionStrings:Test` est configurée.

### 7.2. Vérifier les vulnérabilités NuGet

```bash
dotnet list package --vulnerable --include-transitive
```

## 8. Structure du dépôt

```text
├── src/
│   ├── UniversitySystem.Domain/          # Entités, Enums, Interfaces de base
│   ├── UniversitySystem.Application/     # Services métier, DTOs, Validators
│   ├── UniversitySystem.Infrastructure/  # EF Core, Hangfire, Storage, PDF
│   └── UniversitySystem.Web/             # Contrôleurs, Vues Razor, Areas, Middlewares
├── tests/
│   ├── UniversitySystem.UnitTests/       # Tests unitaires
│   ├── UniversitySystem.IntegrationTests/ # Tests EF Core + MariaDB/MySQL (Respawn)
│   └── UniversitySystem.E2ETests/        # Tests HTTP bout-en-bout
├── docs/                                 # Documentation technique
├── .editorconfig                         # Règles de formatage du code
├── Directory.Build.props                 # Propriétés MSBuild communes
└── README.md
```

## 9. Documentation

| Document | Description |
|---|---|
| Manuel d'utilisation | Guide complet pas à pas pour les 6 profils utilisateurs. |
| Architecture technique | Schémas des couches, des Areas et des flux RBAC. |
| Configuration MariaDB | Guide d'installation OS par OS et optimisation. |
| Revue de sécurité | Rapport OWASP Top 10 et mesures de protection. |
| Guide de déploiement | Procédure de mise en production (IIS, Kestrel, secrets). |
| Flux de paiement | Fonctionnement du webhook HMAC et du FakeGateway en dev. |

---

<div align="center">
  <i>Développé par <b>TAKARA Kondéabalo Calixte</b>, étudiant en 3<sup>e</sup> année Développement Web et Mobile </i>
  <br>
  <i>Assistant : <b>PAKPALI Gilles</b></i>
</div>