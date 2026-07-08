# Plan: 26 — Register NpgSql and Redis Health Check Probes

**Spec:** `.harness/specs/features/26-register-health-check-probes.md`  
**Status:** Ready  
**Created:** 2026-07-08  

> **Nota:** Este issue deve ser implementado no mesmo branch que #27. Probes registrados sem rotas mapeadas (e vice-versa) não são verificáveis. Fechar com um único PR cobrindo ambos.

## Tasks

- [ ] **task-1** · agent: `engineer`  
  Adicionar `AspNetCore.HealthChecks.NpgSql` e `AspNetCore.HealthChecks.Redis` ao `Directory.Packages.props`, referenciar em `Tasks.DependencyInjection.csproj`, e encadear `.AddNpgSql` (duas vezes, para event store e projections) e `.AddRedis` dentro de `AddDependencies()`, lendo as connection strings de `IConfiguration` com as mesmas chaves já usadas no registro dos DbContexts.  
  _depends on: —_  
  _covers: AC-1, AC-2, AC-3, AC-4, AC-5_
