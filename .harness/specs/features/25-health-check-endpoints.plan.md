# Plan: 25 — Add Health Check Endpoints

**Spec:** `.harness/specs/features/25-health-check-endpoints.md`  
**Status:** Ready  
**Created:** 2026-07-08  

## Dependency order

```
task-1 (#26 + #27)  →  task-2 (#28)
                    →  task-3 (#29)
```

## Tasks

- [ ] **task-1** · agent: `engineer`  
  Implement #26 e #27 em um único branch: registrar os três probes em `DependencyExtensions` e mapear `/health/live` e `/health/ready` em `Program.cs`. Os dois issues são inseparáveis — probes sem rotas ou rotas sem probes não permitem validação.  
  _depends on: —_  
  _covers: AC-1, AC-2, AC-5, AC-6_  
  _tracked in: #26, #27_

- [ ] **task-2** · agent: `engineer`  
  Escrever os quatro testes de integração em `HealthCheckEndpointsTests` (ver #28).  
  _depends on: task-1_  
  _covers: AC-1, AC-2, AC-3, AC-4, AC-5, AC-6_  
  _tracked in: #28_

- [ ] **task-3** · agent: `engineer`  
  Adicionar smoke gate step em `.github/workflows/ci.yml` (ver #29).  
  _depends on: task-1_  
  _covers: AC-2, AC-3, AC-4_  
  _tracked in: #29_
