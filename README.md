# FinanceControl

Sistema web para controle de finanças pessoais, desenvolvido em ASP.NET MVC com .NET Framework 4.8.

O projeto foi construído com foco em organização por camadas, separação de responsabilidades e recursos comuns em aplicações corporativas, como autenticação, perfis, criptografia de senha, filtros, paginação, operações via AJAX e geração de relatórios.

## Visão geral

O FinanceControl permite que usuários cadastrem receitas e despesas, acompanhem o resultado financeiro do período, filtrem registros, gerem relatórios e gerenciem seu perfil. Também existe uma área administrativa para listagem e exclusão de usuários, acessivel apenas por perfil de administrador.

## Tecnologias utilizadas

- ASP.NET MVC 5
- .NET Framework 4.8
- C#
- SQL Server
- Entity Framework 6.5.1
- Razor Views
- Bootstrap 5.2.3
- jQuery 3.7.0
- jQuery Validation
- AJAX
- Font Awesome
- SMTP
- JWT

## Arquitetura

O projeto segue uma arquitetura em camadas, separando interface web, regras de negócio e acesso a dados.

```text
FinanceControl
+-- FinanceControl.Web
|   +-- Controllers
|   +-- Views
|   +-- Libs
|   +-- Content
|   +-- Scripts
+-- FinanceControl.Core
|   +-- Entities
|   +-- Services
|   +-- Interfaces
|   +-- DTOs
|   +-- ViewModels
|   +-- Enums
|   +-- Util
+-- FinanceControl.Infrastructure
    +-- Data
    +-- Repositories
    +-- Scripts
    +-- Reports
```
